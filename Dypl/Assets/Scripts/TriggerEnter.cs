using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnter : MonoBehaviour
{
    //stats
    public string myTag;
    public Object myObject;

    //references
    public GameObject enemy;

    //private void OnTriggerStay(Collider other)
    //{
    //    if(myObject!=null&& other.gameObject == myObject)
    //    {
    //        Debug.Log("Thats it");
    //        if (enemy != null)
    //        {
    //            enemy.transform.GetComponent<SimpleEnemy>().forAttack = true;
    //        }
    //        else
    //        {
    //            Debug.Log("Something wrong");
    //        }
    //    }
    //}

}
