using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Should write it in PlayerStatsAndFunction class (thats not possible)
/// </summary>
public class TriggerForActivableItems : MonoBehaviour
{
    //stats
    //public GameObject[] listOfActivableObject;
    public List<GameObject> listOfActivableObject;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.GetComponent<ActivibleItems>())
        {
            other.gameObject.GetComponent<ActivibleItems>().ShowActivible();
            Debug.Log("t/world:Activable item founded");
            //listOfActivableObject[listOfActivableObject.Length] = other.gameObject;
            listOfActivableObject.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ActivibleItems>())
        {
            other.GetComponent<ActivibleItems>().ShowActivible();
            for (int i = 0; i < listOfActivableObject.Count; i++)
            {
                if (listOfActivableObject[i] == other.gameObject)
                {
                    listOfActivableObject.Remove(other.gameObject);
                }
            }
        }
    }


}
