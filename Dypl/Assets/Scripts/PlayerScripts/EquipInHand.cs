using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipInHand : MonoBehaviour
{
    //stats
    public Transform hand;
    private bool visibility = true;

    //references
    public GameObject equipment = null;


    private void Start()
    {
        equipment = (GameObject)Instantiate(equipment, hand);
        ChangeVisibility();
    }

    private void ChangeVisibility()
    {
        if (visibility)
        {
            equipment.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            equipment.transform.GetChild(1).GetComponent<Renderer>().enabled = false;
            visibility = false;

            transform.Find("Pointer").GetComponent<Renderer>().enabled = false;
        }
        else
        {
            equipment.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            equipment.transform.GetChild(1).GetComponent<Renderer>().enabled = true;
            visibility = true;

            transform.Find("Pointer").GetComponent<Renderer>().enabled = true;
        }
        
    }

    private void Update()
    {
        equipment.transform.position = hand.transform.position;
        equipment.transform.rotation = hand.transform.rotation;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameObject.GetComponent<PlayerStatsAndFunction>().state == PlayerStatsAndFunction.PlayerStates.idle)
            {
                gameObject.GetComponent<PlayerStatsAndFunction>().state = PlayerStatsAndFunction.PlayerStates.inFight;
                ChangeVisibility();
            }
            else if (gameObject.GetComponent<PlayerStatsAndFunction>().state == PlayerStatsAndFunction.PlayerStates.inFight)
            {
                gameObject.GetComponent<PlayerStatsAndFunction>().state = PlayerStatsAndFunction.PlayerStates.idle;
                ChangeVisibility();
            }
        }
    }
}
