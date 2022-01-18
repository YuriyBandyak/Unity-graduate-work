using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemy : MonoBehaviour
{
    //enum
    public enum EnemyState { idle, patrol, chase, seeButCannotReach, lostEnemy }

    //stats
    [Header("Debug")]
    public bool visionLines = true;
    public bool pathLines = true;

    [Header("Patrolling")]
    public GameObject parentOfPathPoints;       // it and waitAtPoints should be set in unity 
    public List<float> waitAtPointsForSeconds;  //
    protected int currentPointOfPath = 0;

    //private float speed = 5;
    [Header("Stats")]
    public float damage = 10;
    protected float startHealthPoints = 100;
    public float currentHealthPoints = 100;

    protected float visionAngle = 70;
    protected float visionDistance = 13;

    public EnemyState startState = EnemyState.idle;
    public EnemyState state = EnemyState.idle;

    protected float distanceForCommunication = 20f;

    public float wait = 0;
    public float pauseBetweenAttacks = 0;

    protected bool canSetNewPath = true;

    protected Vector3 startPoint;

    protected NavMeshPath path;

    //references
    public GameObject player;

    protected NavMeshAgent agent;

    public GameObject weapon;
    public bool forAttack = false;

    public Animation anim;

    //test 
    //public string testString;
    public bool isGrounded;

    public void Start()
    {
        startPoint = transform.position;
        agent = GetComponent<NavMeshAgent>();
        if (parentOfPathPoints != null)
        {
            startState = EnemyState.patrol;
            state = startState;
            parentOfPathPoints.SetActive(false);
            int n = parentOfPathPoints.transform.childCount;
            if (waitAtPointsForSeconds.Count != n)
            {
                for (int i = waitAtPointsForSeconds.Count; i < n; i++)
                {
                    waitAtPointsForSeconds.Add(0);
                }
            }
        }
        if (gameObject.transform.Find("Sword"))
        {
            weapon = transform.Find("Sword").gameObject;
        }
        if (weapon != null && weapon.transform.GetComponent<Animation>())
        {
            anim = weapon.transform.GetComponent<Animation>();
            anim.playAutomatically = false;

        }
        currentHealthPoints = startHealthPoints;

        agent.stoppingDistance = 2f; //////////
    }


    public virtual void Update()
    {
        CanAttack();
        IsAttackEnd();
        //isGrounded = false;
        IsGrounded();


        wait -= Time.deltaTime;
        pauseBetweenAttacks -= Time.deltaTime;

        switch (state)
        {
            case EnemyState.chase:
                //Debug.Log("t/AI: goFor() in update");
                GoFor(player.transform);
                WarnAboutPlayer(player);
                Debug.DrawLine(transform.position + new Vector3(0, 1, 0), player.transform.position + Vector3.up, Color.red);
                break;

            case EnemyState.seeButCannotReach:
                Debug.Log("t/AI: The path cannot reach the destination");
                GoFor(player.transform);
                transform.LookAt(player.transform, Vector3.up);  /* for now its ok, 
                                                                  * but shoud change in way that enemy 
                                                                  * get closer and still watch at player 
                                                                  * if possible*/
                WarnAboutPlayer(player);
                break;

            case EnemyState.lostEnemy: /* for now its not working properly, cause its just call LookAround() 
                                        * and change state to idle*/
                //Debug.LogError("lost enemy");
                //Debug.Log("t/AI: LookAround()");
                if (wait > 0)
                {
                    visionAngle = 180;
                }
                LookAround();
                break;

            case EnemyState.patrol:
                //Debug.Log("t/AI: patrol()");
                if (currentPointOfPath == parentOfPathPoints.transform.childCount) currentPointOfPath = 0;
                GoFor(parentOfPathPoints.transform.GetChild(currentPointOfPath));
                if (Vector3.Distance(parentOfPathPoints.transform.GetChild(currentPointOfPath).transform.position, transform.position) < 1f)
                {
                    if (waitAtPointsForSeconds[currentPointOfPath] != 0) WaitFor(waitAtPointsForSeconds[currentPointOfPath]);
                    currentPointOfPath++;
                }
                break;

            default:
                //Debug.Log("t/AI: default()");
                break;
        }

        ChooseState();
    }


    public virtual void Attack()
    {
        forAttack = false;
        //anim["SwordAnim"].wrapMode = WrapMode.Once;
        anim["SwordAnim"].wrapMode = WrapMode.Once;
        //anim.Play();
        anim.Play("SwordAnim");
        pauseBetweenAttacks = 1;
        player.GetComponent<PlayerStatsAndFunction>().GetDamage(damage);  // should be smoothier
    }

    public virtual void CanAttack()
    {
        if (forAttack && pauseBetweenAttacks <= 0)
        {
            Attack();
            Debug.Log("attack");
        }
    }

    public virtual void IsAttackEnd()
    {

    }

    protected void IsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, .5f);
        //return Physics.Raycast(transform.position, -Vector3.up, 0.3f, 9); // 9 - ground

        if (hit.collider != null && hit.collider.gameObject.layer == 9)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }

    public void GetDamage(float dmg)
    {
        currentHealthPoints -= dmg;
    }

    private void ChooseState()
    {

        path = new NavMeshPath();
        if (AngleOfVision(player.transform.position) < visionAngle &&
               DistanceAndObstacles())
        {
            WarnAboutPlayer(player);
            state = EnemyState.chase;
            visionAngle = 45;
            WaitFor(-1);
            ///////////////////
            if (canSetNewPath)
            {
                path = new NavMeshPath();

                if (!agent.CalculatePath(player.transform.position, path))
                {
                    Debug.Log("t/AI: Path is not calculated");
                }
                agent.SetPath(path);

                if (Vector3.Distance(agent.destination, player.transform.position) > 2)
                {
                    if (state != EnemyState.chase && state != EnemyState.seeButCannotReach) WarnAboutPlayer(player);
                    state = EnemyState.seeButCannotReach;
                }
            }
            ///////////////////
            
            //Debug.LogError("DistanceAndObstacles: " + DistanceAndObstacles());
        }
        else if (state == EnemyState.chase && wait <= 0f)
        {
            //Debug.LogError("angleofvision= " + AngleOfVision(player.transform.position));

            state = EnemyState.lostEnemy;
            WaitFor(5);
        }
        else if (state == EnemyState.lostEnemy && wait > 0f)
        {
            state = EnemyState.lostEnemy;
        }
        else if (wait <= 0f)
        {
            state = startState;
            visionAngle = 45;
            if (startState == EnemyState.idle && canSetNewPath)
            {
                if (!agent.CalculatePath(startPoint, path))
                {
                    Debug.Log("t/AI: Path is not calculated");
                }
                agent.SetPath(path);
            }
        }
        else
        {
            //agent.SetDestination(startPoint);
            state = EnemyState.idle;

            path = new NavMeshPath();
            Debug.Log("t/AI: IDLE!");

            if (wait <= -20 && canSetNewPath)
            {
                if (!agent.CalculatePath(startPoint, path))
                {
                    Debug.Log("t/AI: Path is not calculated");
                }
                agent.SetPath(path);
                wait = -1;
            }

        }
    }


    private void WaitFor(float seconds)
    {
        wait = seconds;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point1">target</param>
    /// <returns></returns>
    protected float AngleOfVision(Vector3 point1)
    {
        return Vector3.Angle(point1 - transform.position, transform.forward);
    }

    protected bool DistanceAndObstacles()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > visionDistance) return false;

        RaycastHit[] hits;

        Vector3 centerOfPlayer = player.transform.position;
        Vector3 centerOfEnemy = transform.position;
        Vector3 direction = (centerOfPlayer - centerOfEnemy).normalized;// * visionDistance;

        hits = Physics.RaycastAll(transform.position + new Vector3(0, 1, 0), direction, visionDistance);
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), direction, Color.blue);

        //RaycastHit[] sortedHits = new RaycastHit[hits.Length];

        for (int j = 0; j < hits.Length; j++)
        {
            for (int i = 0; i < hits.Length - 1; i++)
            {
                if (hits[i].distance > hits[i + 1].distance)
                {
                    RaycastHit temp = hits[i + 1];
                    hits[i + 1] = hits[i];
                    hits[i] = temp;
                }
            }
        }
        foreach (RaycastHit hit in hits)
        {
            //Debug.Log("name: " + hit.collider.name);
            if (hit.collider.gameObject.layer == 6) continue; // enemy
            if (hit.collider.gameObject.layer == 8) continue; //nothing

            if (hit.collider.gameObject.layer != 3) break; // non player
            //if (hit.collider.gameObject.layer == 3) return true; //player

            if (hit.collider.gameObject.name == "Body") return true;

        }
        return false;
    }

    public void GoFor(Transform target)
    {
        //Debug.Log("t/AI: GoFor");
        if (canSetNewPath)
        {
            path = new NavMeshPath();

            if (!agent.CalculatePath(target.position, path))
            {
                Debug.Log("t/AI: Path is not calculated");
            }
            agent.SetPath(path);
            //Debug.Log("t/AI: Path.status: " + path.status.ToString());
        }

    }

    private void GoFor(Vector3 position)
    {
        if (canSetNewPath)
        {
            path = new NavMeshPath();
            if (!agent.CalculatePath(position, path))
            {
                Debug.Log("t/AI: Path is not calculated");
            }
            agent.SetPath(path);
        }
        
    }

    private void LookAround()
    {
        //in future there should be code that start animation of enemy searching for player or whatever
        //WaitFor(5);
    }

    private void WalkAroung() // not sure if need this one
    {

    }

    private void WarnAboutPlayer(GameObject target)
    {
        //Debug.Log("WarnAboutPlayer");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0), distanceForCommunication);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == 6 && hitCollider.gameObject.name == "Body" && hitCollider.transform.parent != transform)
            {
                SimpleEnemy otherEnemy = hitCollider.transform.parent.GetComponent<SimpleEnemy>();
                otherEnemy.GetWarned(player, gameObject);
            }
        }
    }

    /// <summary>
    /// Method in enemy that take info about player or enemy target
    /// </summary>
    /// <param name="target">Point of destination</param>
    /// <param name="enemyThatCalled">Object that called this method</param>
    public void GetWarned(GameObject target, GameObject enemyThatCalled)
    {
        Debug.Log("t/AI: enemy was warned about player");
        //testString = "get warned";
        GoFor(target.transform);
        state = EnemyState.chase;
        //WarnAboutPlayer(target, enemyThatCalled);
    }

    //public void GetWarned(Vector3 position, GameObject enemyThatCalled)
    //{
    //    Debug.Log("t/AI: enemy was warned about player");
    //    GoFor(position);
    //    //WarnAboutPlayer(target, enemyThatCalled);
    //}

    public void OnDrawGizmos()
    {
        if (visionLines)
        {
            Gizmos.color = Color.green;
            Vector3 rightLine = Vector3.Lerp(new Vector3(0, 0, 1), new Vector3(1, 0, 0), (float)visionAngle / 90).normalized * visionDistance;
            Vector3 correctRightLine = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * rightLine;
            Gizmos.DrawRay(transform.position, correctRightLine);

            Vector3 leftLine = Vector3.Lerp(new Vector3(0, 0, 1), new Vector3(-1, 0, 0), (float)visionAngle / 90).normalized * visionDistance;
            Vector3 correctLeftLine = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * leftLine;
            Gizmos.DrawRay(transform.position, correctLeftLine);

            for (float i = 0.1f; i < 1f; i += 0.1f)
            {
                Vector3 vector = Vector3.Lerp(leftLine, rightLine, i).normalized * visionDistance;
                Vector3 correctVector = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * vector;
                Gizmos.DrawRay(transform.position, correctVector);
            }
        }

        if (pathLines && parentOfPathPoints != null)
        {
            Gizmos.color = Color.gray;

            for (int i = 0; i < parentOfPathPoints.transform.childCount - 1; i++)
            {
                Gizmos.DrawLine(parentOfPathPoints.transform.GetChild(i).position, parentOfPathPoints.transform.GetChild(i + 1).position);
                Gizmos.DrawLine(parentOfPathPoints.transform.GetChild(0).position, parentOfPathPoints.transform.GetChild(parentOfPathPoints.transform.childCount - 1).position);
                if (currentPointOfPath < parentOfPathPoints.transform.childCount && state == EnemyState.patrol)
                {
                    Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0), parentOfPathPoints.transform.GetChild(currentPointOfPath).position);
                }
                else if (state == EnemyState.patrol)
                {
                    Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0), parentOfPathPoints.transform.GetChild(0).position);
                }
            }


        }


        //Gizmos.color = Color.white;

        //Vector3 centerOfPlayer = player.transform.position;
        //Vector3 centerOfEnemy = transform.position;
        //Vector3 direction = (centerOfPlayer - centerOfEnemy).normalized * visionDistance;
        //Gizmos.DrawRay(transform.position + new Vector3(0, 1, 0), direction);

    }
}
