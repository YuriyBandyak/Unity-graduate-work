using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingForm : MonoBehaviour
{
    //stats
    private Spells.Spell spell;
    private GameObject magicType;

    private float distance = 0;
    public float power;

    public void SetStats(Spells.Spell spell, GameObject magicType)
    {
        power = MagicBalance.Ring.power;
        this.spell = spell;
        this.magicType = magicType;
        this.magicType = Instantiate(magicType, gameObject.transform);

        distance = MagicBalance.Ring.radius;
        var emission = GetComponentInChildren<ParticleSystem>().emission;
        emission.rateOverTime = Mathf.Lerp(MagicBalance.Ring.minParticleAmount, MagicBalance.Ring.maxParticleAmount, MagicBalance.Ring.interpolateVal);

        GetComponentInChildren<SphereCollider>().radius = distance;
        Transform[] transforms;
        transforms = GetComponentsInChildren<Transform>();
        foreach (Transform i in transforms)
        {
            i.localScale = new Vector3(distance, distance, distance);
        }
    }

    private void LateUpdate()
    {
        if (!magicType.transform.GetChild(0).GetComponent<ParticleSystem>().IsAlive()) Destroy(gameObject);
    }
}
