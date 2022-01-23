using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemy : MonoBehaviour
{
    //enum
    public enum EnemyState { idle, patrol, chase, seeButCannotReach, lostEnemy, dead}
    public enum EnemyType { simple, jumper, magician}

    //stats
    [Header("Stats")]
    public EnemyType type = EnemyType.simple;
    private float damage = 10;
    private float startHealthPoints = 100;
    public float currentHealthPoints = 100;

    private float visionAngle = 70;
    private float visionDistance = 20; // difault is 13

    public EnemyState startState = EnemyState.idle;
    public EnemyState state = EnemyState.idle;

    private float distanceForCommunication = 20f;


    public float distanceToPlayer;

    [SerializeField]
    private float waitInOnePlace = 0;
    [SerializeField]
    private float pauseBetweenAttacks = 0;

    [SerializeField]
    private bool canSetNewPath = true;

    private Vector3 startPoint;

    private NavMeshPath path;

    private Vector3 previousPosition;
    public float currentSpeed;
    private float maxSpeed = 3.5f;
    private float walkSpeed = 1.9f;

    public float currentSpeedOfPlayer;
    private Vector3 previousPositionOfPlayer;

    private float simpleStoppingDistance = 1f;
    private float patrolStoppingDistance = 0f;
    private float magicianStoppingDistance = 9f;

    [SerializeField]
    private bool walkBack = false;
    [SerializeField]
    private bool isGrounded;

    [SerializeField]
    private float distanceToAttack = 0;

    [Header("For magician")]
    public Spells.MagicTypes spellType = Spells.MagicTypes.Fire;
    private Spells.Spell attackSpell;
    private float power = 10;
    private float requiredMana = 10;

    private float MaxManaPoints = 100;
    [SerializeField]
    private float currentManaPoints = 100;
    private float manaRegenPerSec = 1 / 50f; // fixedUpdate have 50 fps

    private Vector3 predictPoint;

    private float distanceToAttackForMagician = 13;

    [Header("For jumper")]
    public bool isAttacking = false;
    [SerializeField]
    private float timerForAttack = 0;
    private Transform startTransform;
    private Rigidbody rb;

    private float damageFromJumpAttack = 15f;

    private float distanceToAttackForJumper = 7f;

    private float powerOfJump = 10;
    private float miniJumpPower = 5;
    public bool wasFlying = false;

    [Header("Debug")]
    public bool visionLines = true;
    public bool pathLines = true;

    [Header("Patrolling")]
    public GameObject parentOfPathPoints;       // it and waitAtPoints should be set in unity 
    public List<float> waitAtPointsForSeconds;  //
    private int currentPointOfPath = 0;

    //references
    [Header("References")]
    public GameObject player;

    private NavMeshAgent agent;

    private GameObject weapon;

    private Animator myAnimator;

    public GameObject hand;

    public GameObject spellStartPoint;


    //const strings
    private const string EnemyBody = "enemy_character";
    private const string Sword_Path = "Weapons/Sword_1";
    private const string Staff_Path = "Weapons/Staff_1";
    private const string AnimatorIsAttacking = "attack";
    private const string AnimatorIsDead1 = "death1";
    private const string AnimatorIsWalking = "walk";
    private const string AnimatorIsRunnnigSlow = "runSlow";
    private const string AnimatorIsWalkingBack = "walkBack";
    private const string AnimatorIsGettingDamage = "getDamage";
    private const string AnimatorIsJumping = "jumpUp";
    private const string AnimatorIsAttackingDown = "attackDown";
    private const string DirectFormPrefabPath = "MagicForms/DirectedFormPrefab";
    private const string FireDirectedPath = "MagicType/FireBall2";
    private const string WaterDirectedPath = "MagicType/Waterball";
    private const string EarthDirectedPath = "MagicType/Earth";
    private const string WindDirectedPath = "MagicType/WindDirected";
    private const string PlayerBodyColliderName = "Body";


    public void Start()
    {
        transform.Rotate(new Vector3(0, transform.rotation.y, 0));

        agent = GetComponent<NavMeshAgent>();
        myAnimator = transform.Find(EnemyBody).GetComponent<Animator>();

        startPoint = transform.position;
        agent.speed = maxSpeed;
        currentHealthPoints = startHealthPoints;
        agent.stoppingDistance = simpleStoppingDistance; //////////


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
        
        switch(type)
        {
            case EnemyType.simple:
                weapon = Resources.Load<GameObject>(Sword_Path);
                break;
            case EnemyType.magician:
                weapon = Resources.Load<GameObject>(Staff_Path);
                attackSpell = new Spells.Spell(spellType, Spells.MagicForms.Directed, power, requiredMana);
                attackSpell.SetOwner(gameObject);
                //agent.stoppingDistance = magicianStoppingDistance;
                distanceToAttack = distanceToAttackForMagician;
                break;

            case EnemyType.jumper:
                weapon = Resources.Load<GameObject>(Sword_Path);
                startTransform = transform;

                rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;

                distanceToAttack = distanceToAttackForJumper;
                break;
        }

        Instantiate(weapon, hand.transform);
    }

    private void FixedUpdate()
    {
        if (type == EnemyType.magician)
        {
            if (currentManaPoints < MaxManaPoints) currentManaPoints += manaRegenPerSec;
        }
        if (type == EnemyType.jumper) timerForAttack -= Time.deltaTime;
        if (state != EnemyState.dead)
        {
            CalculateSpeed();
            CalculatePlayerSpeed();
        }
        waitInOnePlace -= Time.deltaTime;
        pauseBetweenAttacks -= Time.deltaTime;
    }

    Vector3 newPos = Vector3.zero;

    private float height = 0.9f;
    AnimatorClipInfo[] m_CurrentClipInfo;

    private void LateUpdate()
    {
        //Those line used for jumperEnemy attack
        GameObject character = transform.Find("enemy_character").gameObject;
        character.transform.position = transform.position;
        m_CurrentClipInfo = this.myAnimator.GetCurrentAnimatorClipInfo(0);

        if (m_CurrentClipInfo[0].clip.name != "jumpDown")
            character.transform.Find("mixamorig:Hips").position = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);

        if (m_CurrentClipInfo[0].clip.name == "Attack down" || m_CurrentClipInfo[0].clip.name == "Jump")
        {
            character.transform.Find("mixamorig:Hips").rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        }
    }

    public virtual void Update()
    {
        distanceToPlayer = (Vector3.Distance(transform.position, player.transform.position));

        if (currentHealthPoints <= 0)
        {
            if (rb != null) rb.freezeRotation = true;
            myAnimator.SetBool(AnimatorIsDead1, true);
            state = EnemyState.dead;
            agent.speed = 0;
            agent.angularSpeed = 0;
        }
        else
        {
            AnimationState();
            CanAttack();
            if (type == EnemyType.jumper) IsJumperAttackPass();
            IsGrounded();
            CalculatePositionForSteppingBack();

            switch (type)
            {
                case EnemyType.simple:
                    break;

                case EnemyType.magician:
                    PredictPlayerNextPosition();

                    if (DistanceAndObstacles() && 
                        Vector3.Distance(transform.position, player.transform.position) <= distanceToAttackForMagician &&
                        //Vector3.Distance(transform.position, player.transform.position) > 5 &&
                        AngleOfVision(player.transform.position) < visionAngle)
                    {
                        transform.LookAt(player.transform.position);
                    }

                    if (myAnimator.GetBool(AnimatorIsAttacking) && distanceToPlayer > 5) //where enemy going to shoot
                    {
                        transform.LookAt(predictPoint);
                        
                    }
                    else if (myAnimator.GetBool(AnimatorIsAttacking))
                    {
                        transform.LookAt(player.transform.position);
                    }
                    if (AngleOfVision(player.transform.position) - AngleOfVision(predictPoint) > 90)
                    {
                        transform.LookAt(player.transform.position);
                    }

                    if (myAnimator.GetBool(AnimatorIsAttacking))
                    {
                        agent.speed = 0;
                    }
                    else if (!walkBack)
                    {
                        agent.speed = maxSpeed;
                    }
                    else
                    {
                        agent.speed = walkSpeed;
                    }
                    break;

                case EnemyType.jumper:
                    break;
            }
        }

        switch (state)
        {
            case EnemyState.chase:
                if (!myAnimator.GetBool(AnimatorIsAttacking)) agent.speed = maxSpeed;

                WarnAboutPlayer(player);
                Debug.DrawLine(transform.position + new Vector3(0, 1, 0), player.transform.position + Vector3.up, Color.red);
                if (type == EnemyType.magician)
                {
                    agent.stoppingDistance = magicianStoppingDistance;
                    if (distanceToPlayer < 7)
                    {
                        agent.stoppingDistance = 0;
                        walkBack = true;
                        if (!myAnimator.GetBool(AnimatorIsAttacking)) agent.speed = 2;

                        //agent.angularSpeed = 0;
                        GoFor(newPos);
                        Debug.Log("Magician enemy stepping back");
                    }
                    else
                    {
                        walkBack = false;
                        if (!myAnimator.GetBool(AnimatorIsAttacking)) agent.speed = maxSpeed;
                        //agent.angularSpeed = 120;
                        GoFor(player.transform);
                    }
                }
                else
                {
                    agent.stoppingDistance = simpleStoppingDistance;
                    GoFor(player.transform);
                }
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
                if (waitInOnePlace > 0)
                {
                    visionAngle = 180;
                }
                break;

            case EnemyState.patrol:
                agent.speed = walkSpeed;
                agent.stoppingDistance = patrolStoppingDistance;
                //Debug.Log("t/AI: patrol()");
                if (currentPointOfPath == parentOfPathPoints.transform.childCount) currentPointOfPath = 0;
                GoFor(parentOfPathPoints.transform.GetChild(currentPointOfPath));
                if (Vector3.Distance(parentOfPathPoints.transform.GetChild(currentPointOfPath).transform.position, transform.position) < 2f)
                {
                    if (waitAtPointsForSeconds[currentPointOfPath] > 0) WaitFor(waitAtPointsForSeconds[currentPointOfPath]);
                    currentPointOfPath++;
                }
                break;

            case EnemyState.dead:
                
                break;

            default:
                
                //Debug.Log("t/AI: default()");
                break;
        }
        if (state != EnemyState.dead) ChooseState();
    }

    private void CalculateSpeed()
    {
        Vector3 curMove = transform.position - previousPosition;
        currentSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;
    }

    private void CalculatePlayerSpeed()
    {
        Vector3 curMovePlayer = player.transform.position - previousPositionOfPlayer;
        currentSpeedOfPlayer = curMovePlayer.magnitude / Time.deltaTime;
        previousPositionOfPlayer = player.transform.position;
    }

    /// <summary>
    /// Calculating position for stepping back
    /// </summary>
    private void CalculatePositionForSteppingBack()
    {
        Vector3 dirToPlayer = transform.position - player.transform.position;
        newPos = transform.position + dirToPlayer;
        newPos.y = 0.1f;
    }

    public virtual void AnimationState()
    {
        if (currentSpeed <=0.1f)
        {
            myAnimator.SetBool(AnimatorIsWalking, false);
            myAnimator.SetBool(AnimatorIsRunnnigSlow, false);
        }
        if (currentSpeed > 0.1f && currentSpeed < 1.5f && !walkBack)
        {
            myAnimator.SetBool(AnimatorIsWalking, true);
            myAnimator.SetBool(AnimatorIsRunnnigSlow, false);
        }
        if (currentSpeed > 0.1f && currentSpeed < 2.5f && walkBack)
        {
            myAnimator.SetBool(AnimatorIsWalking, false);
            myAnimator.SetBool(AnimatorIsRunnnigSlow, false);
            myAnimator.SetBool(AnimatorIsWalkingBack, true);
        }
        if (currentSpeed >= 1.5f && currentSpeed < 6f)
        {
            myAnimator.SetBool(AnimatorIsWalking, false);
            myAnimator.SetBool(AnimatorIsRunnnigSlow, true);
        }
    }

    private void PredictPlayerNextPosition()
    {
        Vector3 nextPlace = (Quaternion.AngleAxis(player.transform.rotation.eulerAngles.y, Vector3.up) *
                new Vector3(0, 0, 1)) * (Vector3.Distance(transform.position, player.transform.position) *
                currentSpeedOfPlayer / 5f);
        predictPoint = player.transform.position + nextPlace;
    }


    public virtual void Attack()
    {
        //agent.updatePosition = false;
        
        switch (type)
        {
            case EnemyType.simple:
                agent.speed = 0;
                pauseBetweenAttacks = 1;
                myAnimator.SetBool(AnimatorIsAttacking, true);
                Debug.Log("AI: Enemy attacking");
                break;
            case EnemyType.magician:
                agent.speed = 0;
                transform.LookAt(player.transform.position);
                myAnimator.SetBool(AnimatorIsAttacking, true);
                pauseBetweenAttacks = 2;

                break;
            case EnemyType.jumper:
                Debug.Log("Jumper is attacking");
                myAnimator.SetBool(AnimatorIsJumping, true);

                break;
        }
    }

    public void JumpForAttack()
    {
        canSetNewPath = false;
        agent.enabled = false;

        rb.isKinematic = false;
        rb.useGravity = true;

        //Vector3 playerHead = player.transform.position + new Vector3(0, 3, 0);
        Vector3 directionAndPower = (player.transform.position - transform.position).normalized * powerOfJump;
        rb.AddForce(new Vector3(0, miniJumpPower, 0), ForceMode.Impulse);
        rb.AddForce(directionAndPower, ForceMode.Impulse);

        pauseBetweenAttacks = 2;
        timerForAttack = 0.7f;

        isAttacking = true;
    }

    private void IsJumperAttackPass()
    {
        if (myAnimator.GetBool(AnimatorIsJumping) &&
            !isGrounded &&
            distanceToPlayer < 1.5f &&
            isAttacking)
        {
            Debug.Log("Jumper attack passed");
            myAnimator.SetBool(AnimatorIsAttackingDown, true);
            myAnimator.SetBool(AnimatorIsJumping, false);

            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            transform.LookAt(player.transform);
        }
        else if ((myAnimator.GetBool(AnimatorIsJumping) || (myAnimator.GetBool(AnimatorIsAttackingDown))) &&
            distanceToPlayer > 1.5f &&
            isGrounded &&
            isAttacking &&
            timerForAttack <= 0)
        {
            Debug.Log("Jumper attack didnt pass");
            myAnimator.SetBool(AnimatorIsAttackingDown, false);
            myAnimator.SetBool(AnimatorIsJumping, false);

            rb.isKinematic = true;
            rb.useGravity = false;

            agent.enabled = true;
            canSetNewPath = true;

            isAttacking = false;
        }
        else if(isGrounded &&
            !isAttacking &&
            timerForAttack <= 0 && 
            !rb.isKinematic)
        {
            Debug.Log("Jumper attack didnt pass");
            myAnimator.SetBool(AnimatorIsAttackingDown, false);
            myAnimator.SetBool(AnimatorIsJumping, false);

            rb.isKinematic = true;
            rb.useGravity = false;

            agent.enabled = true;
            canSetNewPath = true;
        }
    }

    private void SimpleMeleeAttack()
    {
        pauseBetweenAttacks = 1;
        myAnimator.SetBool(AnimatorIsAttacking, true);
        Debug.Log("AI: Enemy attacking");
    }

    public void AnimationPointOfAttack()
    {
        switch (type)
        {
            case EnemyType.simple:
                player.GetComponent<PlayerStatsAndFunction>().GetDamage(damage);  // should be smoothier
                break;
            case EnemyType.magician:
                GameObject spell;
                GameObject prefabType;
                GameObject directedFormPrefab = Resources.Load<GameObject>(DirectFormPrefabPath);

                currentManaPoints -= requiredMana;

                switch (spellType)
                {
                    case Spells.MagicTypes.Fire:
                        prefabType = Resources.Load<GameObject>(FireDirectedPath);
                        spell = Instantiate(directedFormPrefab, spellStartPoint.transform.position
                            , Quaternion.Euler(transform.rotation.eulerAngles));
                        spell.GetComponent<DirectedForm>().setStats(prefabType, gameObject);
                        break;

                    case Spells.MagicTypes.Water:
                        prefabType = Resources.Load<GameObject>(WaterDirectedPath);
                        spell = Instantiate(directedFormPrefab, spellStartPoint.transform.position
                            , Quaternion.Euler(transform.rotation.eulerAngles));
                        spell.GetComponent<DirectedForm>().setStats(prefabType, gameObject);
                        break;

                    case Spells.MagicTypes.Earth:
                        prefabType = Resources.Load<GameObject>(EarthDirectedPath);
                        spell = Instantiate(directedFormPrefab, spellStartPoint.transform.position
                            , Quaternion.Euler(transform.rotation.eulerAngles));
                        spell.GetComponent<DirectedForm>().setStats(prefabType, gameObject);
                        break;

                    case Spells.MagicTypes.Wind:
                        prefabType = Resources.Load<GameObject>(WindDirectedPath);
                        spell = Instantiate(directedFormPrefab, spellStartPoint.transform.position
                            , Quaternion.Euler(transform.rotation.eulerAngles));
                        spell.GetComponent<DirectedForm>().setStats(prefabType, gameObject);
                        break;
                }
                break;
            case EnemyType.jumper:
                //player.GetComponent<PlayerStatsAndFunction>().GetDamage(damage);  // should be smoothier
                if (!rb.isKinematic)
                {
                    player.GetComponent<PlayerStatsAndFunction>().GetDamage(damageFromJumpAttack);
                }
                else
                {
                    player.GetComponent<PlayerStatsAndFunction>().GetDamage(damage);
                }
                
                break;
        }
    }

    public virtual void CanAttack()
    {
        switch (type)
        {
            case EnemyType.simple:
                if (pauseBetweenAttacks <= 0 &&
                    distanceToPlayer <= 2f &&
                    AngleOfVision(player.transform.position) < visionAngle)
                {
                    Attack();
                    Debug.Log(AnimatorIsAttacking);
                }
                break;
            case EnemyType.magician:
                if (Vector3.Distance(transform.position, player.transform.position) < distanceToAttackForMagician &&
                    DistanceAndObstacles() &&
                    currentManaPoints > attackSpell.requiredMana &&
                    pauseBetweenAttacks <= 0 &&
                    AngleOfVision(player.transform.position) < visionAngle)
                {
                    Debug.Log("Magic attack");
                    Attack();
                    pauseBetweenAttacks = 2;
                }
                    break;
            case EnemyType.jumper:
                //if (distanceToPlayer < 3f &&
                //    pauseBetweenAttacks <= 0 &&
                //    AngleOfVision(player.transform.position) < visionAngle &&
                //    DistanceAndObstacles())
                //{
                //    SimpleMeleeAttack();
                //}
                //else
                //if (distanceToPlayer < distanceToAttack &&
                //    pauseBetweenAttacks <= 0 &&
                //    AngleOfVision(player.transform.position) < visionAngle &&
                //    DistanceAndObstacles())
                //{
                //    Attack();
                //}
                //else if (distanceToPlayer < distanceToAttack &&
                //    DistanceAndObstacles())
                //{
                //    GoFor(player.transform);
                //}
                if (distanceToPlayer < 2f &&
                    pauseBetweenAttacks <= 0 &&
                    AngleOfVision(player.transform.position) < visionAngle &&
                    DistanceAndObstacles())
                {
                    SimpleMeleeAttack();
                }
                else if (distanceToPlayer < distanceToAttackForJumper &&
                    distanceToPlayer >=2f &&
                    pauseBetweenAttacks <= 0 &&
                    AngleOfVision(player.transform.position) < visionAngle &&
                    DistanceAndObstacles())
                {
                    Attack();
                }

                break;
        }
    }

    public void IsAnimationOfAttackEnd()
    {
        agent.speed = maxSpeed;
        switch (type)
        {
            case EnemyType.simple:
                myAnimator.SetBool(AnimatorIsAttacking, false);

                break;
            case EnemyType.magician:
                myAnimator.SetBool(AnimatorIsAttacking, false);

                break;
            case EnemyType.jumper:
                if (!rb.isKinematic)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;

                    agent.enabled = true;
                    canSetNewPath = true;
                }
                else
                {
                    myAnimator.SetBool(AnimatorIsAttacking, false);
                }

                isAttacking = false;

                break;
        }
    }


    protected void IsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + new Vector3(0,.2f,0), new Vector3(0, -1, 0), out hit, .3f);

        if (hit.collider != null && hit.collider.gameObject.layer == 9)
        {
            if (hit.distance < 0.5f)
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
            if (type == EnemyType.jumper) isAttacking = true;
        }
    }

    public void GetDamage(float dmg)
    {
        currentHealthPoints -= dmg;
        myAnimator.SetBool(AnimatorIsGettingDamage, true);
        Debug.Log("enemy get damage: " + dmg);

        if (currentHealthPoints <= 0)
        {
            if (rb != null) rb.freezeRotation = true;
            state = EnemyState.dead;
            agent.speed = 0;
            agent.angularSpeed = 0;
        }
    }

    private void ChooseState()
    {
        if (currentHealthPoints > 0)
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
                if (canSetNewPath && !walkBack)
                {
                    path = new NavMeshPath();

                    if (!agent.CalculatePath(player.transform.position, path))
                    {
                        Debug.Log("t/AI: Path is not calculated");
                    }
                    //GoFor(player.transform);

                    if (Vector3.Distance(agent.destination, player.transform.position) > 2)
                    {
                        if (state != EnemyState.chase && state != EnemyState.seeButCannotReach) WarnAboutPlayer(player);
                        state = EnemyState.seeButCannotReach;
                    }
                }
            }
            else if (state == EnemyState.chase && waitInOnePlace <= 0f)
            {
                state = EnemyState.lostEnemy;
                WaitFor(5);
            }
            else if (state == EnemyState.lostEnemy && waitInOnePlace > 0f)
            {
                state = EnemyState.lostEnemy;
            }
            else if (waitInOnePlace <= 0f)
            {
                state = startState;
                visionAngle = 45;
            }
            else
            {
                state = EnemyState.idle;
                path = new NavMeshPath();
                if (waitInOnePlace <= -20 && canSetNewPath)
                {
                    GoFor(startPoint);
                    //waitInOnePlace = -1;
                    WaitFor(-1);
                }
            }
        }
    }


    private void WaitFor(float seconds)
    {
        waitInOnePlace = seconds;
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
            if (hit.collider.gameObject.layer == 6) continue; // enemy
            if (hit.collider.gameObject.layer == 7) continue; // spell
            if (hit.collider.gameObject.layer == 8) continue; //nothing

            if (hit.collider.gameObject.layer != 3) break; // non player

            

            if (hit.collider.gameObject.name == PlayerBodyColliderName) return true;
        }
        return false;
    }

    public void GoFor(Transform target)
    {
        if (canSetNewPath)
        {
            path = new NavMeshPath();

            if (!agent.CalculatePath(target.position, path))
            {
                Debug.Log("t/AI: Path is not calculated");
            }
            agent.SetPath(path);
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

    private void WalkAroung() // not sure if need this one
    {

    }

    private void WarnAboutPlayer(GameObject target)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0), distanceForCommunication);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == 6 && hitCollider.gameObject.name == PlayerBodyColliderName && 
                hitCollider.transform.parent != transform)
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
        GoFor(target.transform);
        state = EnemyState.chase;
    }
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
                Gizmos.DrawLine(parentOfPathPoints.transform.GetChild(0).position, parentOfPathPoints.transform
                    .GetChild(parentOfPathPoints.transform.childCount - 1).position);
                if (currentPointOfPath < parentOfPathPoints.transform.childCount && state == EnemyState.patrol)
                {
                    Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0), parentOfPathPoints.transform
                        .GetChild(currentPointOfPath).position);
                }
                else if (state == EnemyState.patrol)
                {
                    Gizmos.DrawLine(transform.position + new Vector3(0, 1, 0), parentOfPathPoints.transform.GetChild(0).position);
                }
            }
        }

        if (type == EnemyType.magician)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(predictPoint, Vector3.one);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(newPos, Vector3.one);
            Gizmos.color = Color.red;
            if (agent != null)
            {
                Gizmos.DrawCube(agent.destination, Vector3.one);
            }
            
        }
    }
}
