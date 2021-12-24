using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add this script to buttons parent
/// </summary>
public class ChooseOneFromAllButtons : MonoBehaviour
{
    /// <summary>
    /// Set button activated||pressed||selected, and turn on others buttons 
    /// </summary>
    /// <param name="id">Button position in hierarchy</param>
    public void SetSelectable(int id)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Button but = transform.GetChild(i).GetComponent<Button>();
            but.interactable = true;
            if (i == id) but.interactable = false;
        }
    }
}
