using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for difference items to active some component or smth like that
/// </summary>
public class ActivibleItems : MonoBehaviour
{
    //references
    public GameObject objectToDisactivate;
    public GameObject messege;

    public void Action()
    {
        if (objectToDisactivate != null) objectToDisactivate.SetActive(!objectToDisactivate.activeInHierarchy);
        //other action
        //ShowActivible();
    }

    public void ShowActivible()
    {
        messege.SetActive(!messege.activeInHierarchy);
        
        //Debug.Log("Object has been activated");
    }
}
