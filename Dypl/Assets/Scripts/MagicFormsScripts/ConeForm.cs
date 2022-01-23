using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeForm : MonoBehaviour
{
    //stats
    private Spells.Spell spell;
    private GameObject magicType;

    public float distance = 0;

    public float power;

    public void SetStats(Spells.Spell spell, GameObject magicType, Transform playerTransform)
    {
        power = MagicBalance.Cone.power;
        this.spell = spell;
        this.magicType = magicType;
        //gameObject.transform.Rotate(new Vector3(0, playerTransform.rotation.y, 0));
        this.magicType = Instantiate(magicType, gameObject.transform);

        distance = MagicBalance.Cone.distance;

        Transform[] transforms;
        transforms = GetComponentsInChildren<Transform>();
        foreach (Transform i in transforms)
        {
            i.localScale = i.localScale * distance;
        }

        transform.GetChild(0).gameObject.AddComponent<ColliderReactor>();
    }

    private void LateUpdate()
    {
        if (GetComponentInChildren<ParticleSystem>().particleCount == 0)
        {
            Object.Destroy(gameObject,1);
        }
    }

    

    private class ColliderReactor : MonoBehaviour
    {

        private float power = 0;

        private void Start()
        {
            power = MagicBalance.Cone.power;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("name: " + other.name);
            if (other.gameObject.layer == 6) // enemy
            {
                if (other.transform.parent.GetComponent<SimpleEnemy>())
                {
                    other.transform.parent.GetComponent<SimpleEnemy>().GetDamage(power);

                    /*
                     * should add type of spell to constructor and if its fire then add effect on enemy or player
                     */

                    Destroy(gameObject);
                }
            }
            else if (other.gameObject.layer == 3) //player
            {
                if (other.transform.parent.GetComponent<PlayerStatsAndFunction>())
                {
                    other.transform.parent.GetComponent<PlayerStatsAndFunction>().GetDamage(power);
                    Destroy(gameObject);
                }
            }
        }
    }

}
