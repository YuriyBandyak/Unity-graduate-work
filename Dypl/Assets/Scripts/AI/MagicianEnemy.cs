using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicianEnemy : SimpleEnemy
{
    //enums

    //stats
    public Spells.MagicTypes type = Spells.MagicTypes.Fire;
    private float MaxManaPoints = 100;
    public float currentManaPoints = 100;
    private float manaRegenPerSec = 1 / 50f; // fixedUpdate have 50 fps

    private bool isAttackPossible = false;

    private Spells.Spell attackSpell;

    private float power = 10;
    private float requiredMana = 10;

    private float pauseBetweenAttacksSec = 2 / 50f; // fixedUpdate have 50 fps
    public float currentPause = 0;

    private Vector3 predictPoint;
    private Vector3 lastNextPlace = Vector3.zero; // for more comfortable testing

    //references
    public GameObject spellStartPoint;
    
    private new void Start()
    {
        base.Start();

        visionDistance = 30;
        attackSpell = new Spells.Spell(type, Spells.MagicForms.Directed, power, requiredMana);

        agent.stoppingDistance = 17;

        //agent.angularSpeed = 0;  // stop agent from rotating
    }

    private void FixedUpdate()
    {
        if (currentManaPoints < MaxManaPoints) currentManaPoints += manaRegenPerSec;
        currentPause -= Time.deltaTime;

        if (DistanceAndObstacles() && Vector3.Distance(transform.position, player.transform.position) <= 20 && AngleOfVision(player.transform.position) < visionAngle)
        {
            transform.LookAt(predictPoint);
            Debug.Log("I can see you");
        }
    }

    private new void Update()
    {
        base.Update();

        if (lastNextPlace == Vector3.zero)  // should change formula to include spell speed
        {
            lastNextPlace = (Quaternion.AngleAxis(player.transform.rotation.eulerAngles.y, Vector3.up) * new Vector3(0, 0, 1)) * Vector3.Distance(transform.position, player.transform.position) * player.GetComponent<PlayerMovement>().playerSpeed / 5f;
        }
        Vector3 nextPlace = Vector3.Lerp((Quaternion.AngleAxis(player.transform.rotation.eulerAngles.y, Vector3.up) * new Vector3(0, 0, 1)) * Vector3.Distance(transform.position, player.transform.position) * player.GetComponent<PlayerMovement>().playerSpeed / 5f, lastNextPlace, 0.5f);
        lastNextPlace = nextPlace;
        predictPoint = player.transform.position + nextPlace;
    }

    public override void CanAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 20 &&
            DistanceAndObstacles() &&
            currentManaPoints > attackSpell.requiredMana &&
            currentPause <= 0 &&
            AngleOfVision(player.transform.position) < visionAngle)
        {
            Attack();
            currentPause = 2;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) < 20 &&
           DistanceAndObstacles())
        {
            transform.LookAt(predictPoint);
        }
    }

    public override void Attack()
    {
        GameObject spell;
        GameObject prefabType;
        GameObject directedFormPrefab = Resources.Load<GameObject>("MagicForms/DirectedFormPrefab");

        currentManaPoints -= requiredMana;

        switch (type)
        {
            case Spells.MagicTypes.Fire:
                prefabType = Resources.Load<GameObject>("MagicType/FireBall2");
                spell = Instantiate(directedFormPrefab, spellStartPoint.transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
                spell.GetComponent<DirectedForm>().setStats(prefabType, gameObject);
                break;

            case Spells.MagicTypes.Water:
                prefabType = Resources.Load<GameObject>("MagicType/Waterball");
                spell = Instantiate(directedFormPrefab, spellStartPoint.transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
                spell.GetComponent<DirectedForm>().setStats(prefabType, gameObject);
                break;

            case Spells.MagicTypes.Earth:
                prefabType = Resources.Load<GameObject>("MagicType/Earth");
                spell = Instantiate(directedFormPrefab, spellStartPoint.transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
                spell.GetComponent<DirectedForm>().setStats(prefabType, gameObject);
                break;

            case Spells.MagicTypes.Wind:
                prefabType = Resources.Load<GameObject>("MagicType/WindDirected");
                spell = Instantiate(directedFormPrefab, spellStartPoint.transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
                spell.GetComponent<DirectedForm>().setStats(prefabType, gameObject);
                break;
        }
    }



    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawCube(predictPoint, Vector3.one);
    }
}
