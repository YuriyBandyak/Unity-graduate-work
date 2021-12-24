using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //references
    public GameObject player = null;

    //stats
    private Vector3 simpleCameraPos;
    private Vector3 fightCameraPos;

    //private bool viewChanging = false; // create camera switching system in future (between inFight mode and idle mode)

    private void Start()
    {
        simpleCameraPos = new Vector3(0, 15, -6);
        fightCameraPos = new Vector3(0,8,-4);
    }

    private void Update()
    {
        Vector3 addedVector;
        if(player.GetComponent<PlayerStatsAndFunction>().state == PlayerStatsAndFunction.PlayerStates.inFight)
        {
            addedVector = player.transform.position + simpleCameraPos;
        }
        else
        {
            addedVector = player.transform.position + fightCameraPos;
        }

        transform.position = addedVector;
        transform.LookAt(player.transform);
    }

}
