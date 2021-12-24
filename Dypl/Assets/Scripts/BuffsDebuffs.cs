using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For player and other charters
/// </summary>
public class BuffsDebuffs : MonoBehaviour
{
    //enum
    public enum buffsDebuffs { none, clearFromAll, fired, wet, poisoned}

    //struct
    private struct effect
    {
        buffsDebuffs effectName;
        public float effectTime;

        public effect(buffsDebuffs effectName, float effectTime)
        {
            this.effectName = effectName;
            this.effectTime = effectTime; // seconds
        }
    }

    //stats
    private effect[] effectList = {
        new effect(buffsDebuffs.fired,0),
        new effect(buffsDebuffs.wet,0),
        new effect(buffsDebuffs.poisoned,0)
    };

    private bool isEffected = false;

    //references
    //public GameObject player;

    public void AddEffect(buffsDebuffs effect, float timeForEffect)
    {
        switch (effect)
        {
            case buffsDebuffs.fired:
                effectList[0].effectTime += timeForEffect;
                isEffected = true;
                break;
            case buffsDebuffs.wet:
                effectList[1].effectTime += timeForEffect;
                isEffected = true;
                break;
            case buffsDebuffs.poisoned:
                effectList[2].effectTime += timeForEffect;
                isEffected = true;
                break;
            case buffsDebuffs.clearFromAll:
                effectList[0].effectTime = 0;
                effectList[1].effectTime = 0;
                effectList[2].effectTime = 0;
                isEffected = false;
                break;
        }
    }

    private void FixedUpdate() // add combos    example: fired + wet = nothing
    {
        if (isEffected)
        {
            if (effectList[0].effectTime > 0)
            {
                ShowEffect(buffsDebuffs.fired, effectList[0].effectTime);
                if (GetComponent<PlayerStatsAndFunction>()) GetComponent<PlayerStatsAndFunction>().currentHP -= 5 / 50f;
                effectList[0].effectTime -= 1/50f;
                //if (effectObj.transform.GetChild(0).GetComponent<ParticleSystem>().particleCount <= 0)
                //    //Object.Destroy(effectObj, 1);
                //    //Debug.Log("ParticleCount: " + effectObj.transform.GetChild(0).GetComponent<ParticleSystem>().particleCount);
                //    Debug.Log("ChildrenCount: " + effectObj.transform.childCount);
                if (effectList[0].effectTime <= 0)
                {
                    
                    Object.Destroy(effectObj, 5);
                    effectObj = null;
                }

                //Debug.Log("Fire effect duration: " + effectList[0].effectTime);
            }
            if (effectList[1].effectTime > 0)
            {
                ShowEffect(buffsDebuffs.wet, effectList[1].effectTime);
                //if (GetComponent<PlayerStatsAndFunction>()) GetComponent<PlayerStatsAndFunction>().currentHP -= 1 / 50;
                effectList[1].effectTime -= 1 / 50f;
                //if (effectList[1].effectTime <= 0) Object.Destroy(effectObj);
            }
            if (effectList[2].effectTime > 0)
            {
                ShowEffect(buffsDebuffs.poisoned, effectList[2].effectTime);
                if (GetComponent<PlayerStatsAndFunction>()) GetComponent<PlayerStatsAndFunction>().currentHP -= 2 / 50f;
                effectList[2].effectTime -= 1 / 50f;
                //if (effectList[2].effectTime <= 0) Object.Destroy(effectObj);
            }
        }
    }



    private GameObject effectPref = null;
    private GameObject effectObj = null;
    private void ShowEffect(buffsDebuffs effectName, float timeForEffect)
    {
        
        if (effectName == buffsDebuffs.fired)
        {
            effectPref = Resources.Load("Effects/FireLong") as GameObject;
        }
        if (effectObj == null)
        {
            effectObj = Instantiate(effectPref, gameObject.transform.Find("Body").transform.position + new Vector3(0,1,0), 
                Quaternion.identity);

            //official manual way to change duration for partical system
            ParticleSystem ps = effectObj.transform.GetChild(0).GetComponent<ParticleSystem>();
            ps.Stop();
            ParticleSystem.MainModule main = ps.main;
            main.duration = 5;
            ps.Play();
            //

            effectPref = null;
            Debug.Log("t/buffDebuff: Effect is instantiated");
        }
        else if (effectObj != null)
        {
            effectObj.transform.position = gameObject.transform.position;
        }
    }


}
