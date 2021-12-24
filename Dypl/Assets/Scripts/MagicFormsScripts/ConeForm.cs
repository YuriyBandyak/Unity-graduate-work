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
        gameObject.transform.rotation = playerTransform.rotation;
        this.magicType = Instantiate(magicType, gameObject.transform);

        distance = MagicBalance.Cone.distance;
        if (GetComponentInChildren<MeshCollider>())
        {
            GetComponentInChildren<MeshCollider>().transform.localScale = new Vector3(distance, distance, distance);
        }
        Transform[] transforms;
        transforms = GetComponentsInChildren<Transform>();
        foreach(Transform i in transforms)
        {
            i.localScale = new Vector3(distance, distance, distance);
        }
    }

    private void LateUpdate()
    {
        if (GetComponentInChildren<ParticleSystem>().particleCount == 0)
        {
            Object.Destroy(gameObject,5);
        }
    }

    //private void LateUpdate()
    //{
    //    //if (magicType.transform.GetChild(0).GetComponent<ParticleSystem>().particleCount == 0) Destroy(gameObject);
    //}
}
