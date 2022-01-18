using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
    public void EndOfCasting()
    {
        transform.parent.GetComponent<PlayerStatsAndFunction>().EndOfCasting();
    }

    public void CastMoment()
    {
        transform.parent.GetComponent<PlayerStatsAndFunction>().WaitForAnimation();
    }
}
