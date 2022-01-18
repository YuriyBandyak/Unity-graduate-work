using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JumperEnemy : SimpleEnemy
{
    [Header("Jumper stats")]
    public float distanceToAttack = 7f;
    public float distanceToPlayer;

    public float powerOfJump = 10;
    public float miniJumpPower = 5;

    public bool isAttacking = false;
    private bool isAttackPass = false;

    private Rigidbody rb;

    private Transform startTransform;

    public GameObject testObject;

    private new void Start()
    {
        base.Start();

        startTransform = transform;
        
        if (GetComponent<Rigidbody>())
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
        else
        {
            Debug.LogError("Add rigidbody component to enemy");
        }
    }

    public void GetDamage(int damageAmount)
    {
        currentHealthPoints -= damageAmount;
    }

    private void PlayAttackAnimation() { }

    private void IfAttackPass()
    {
        if(Vector3.Distance(transform.position,player.transform.position) < 2 && 
            isAttacking &&
            !isAttackPass)
        {
            PlayAttackAnimation();
            Debug.Log("PlayerAttackAnimation");
            player.GetComponent<PlayerStatsAndFunction>().GetDamage(damage);
            isAttackPass = true;
        }
    }

    public override void Attack()
    {
        Vector3 playerHead = player.transform.position + new Vector3(0, 3, 0);
        Vector3 directionAndPower = (player.transform.position - transform.position).normalized * powerOfJump;
        rb.AddForce(new Vector3(0, miniJumpPower, 0), ForceMode.Impulse);
        rb.AddForce(directionAndPower, ForceMode.Impulse);
        Debug.Log("Jumper Attack");
        pauseBetweenAttacks = 1;
        isAttacking = true;

        timer = 1;
    }


    public override void CanAttack()
    {
        distanceToPlayer = (Vector3.Distance(transform.position, player.transform.position));

        if (distanceToPlayer < distanceToAttack && 
            pauseBetweenAttacks<=0 && 
            AngleOfVision(player.transform.position) < visionAngle && 
            DistanceAndObstacles())
        {
            Debug.Log("CanAttack = true");
            canSetNewPath = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;

            Attack();
            
        }
        else if (distanceToPlayer < distanceToAttack &&
            DistanceAndObstacles())
        {
            GoFor(player.transform);
        }
    }

    //test
    public override void Update()
    {
        base.Update();

        timer -= Time.deltaTime;
        //if (Physics.Raycast(new Ray(transform.position, transform.position - new Vector3(0,-1,0)), out hit, 10f, 9) && Vector3.Distance(transform.position, hit.point) > 0.5f)
        //{
        //    wasFlying = true;
        //}

        RaycastHit hit;

        Ray ray = new Ray(transform.position, -Vector3.up);

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.layer == 9)
        {
            //testObject.transform.position = hit.point - transform.position;
            //Debug.Log("name: " + hit.collider.name);
            //Debug.Log("distanceFromRayStart: " + hit.distance);
            //Debug.Log("distanceFromEnemy: " + Vector3.Distance(transform.position, hit.point));
        }
        if (hit.distance < 0.5)
        {
            isGrounded = true;
        }

        IfAttackPass();

    }
    public float timer = 0;
    public bool wasFlying = false;
    
    public override void IsAttackEnd()
    {
        if (isGrounded && isAttacking && timer <= 0)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            //agent.enabled = true;
            canSetNewPath = true;
            
            isAttacking = false;
            wasFlying = false;
            transform.position = startTransform.position;
            transform.rotation = startTransform.rotation;
            transform.localScale = startTransform.localScale;

            ////temp 
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
            Debug.Log("AttackEnd");

            GetComponent<NavMeshAgent>().enabled = true;
            isAttackPass = false;
        }
    }
}
