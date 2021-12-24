using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffCastPoint : MonoBehaviour
{
    //references
    public GameObject player;

    private void Start()
    {
        player = transform.parent.parent.gameObject;
    }
    public void CallPlayerWaitOnAnimationFunc()
    {
        player.GetComponent<PlayerStatsAndFunction>().WaitForAnimation();
    }
}
