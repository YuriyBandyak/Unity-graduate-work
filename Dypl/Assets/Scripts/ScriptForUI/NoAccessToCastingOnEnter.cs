using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoAccessToCastingOnEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject playerObj;
    public bool pointerOnUI = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOnUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOnUI = false;
    }
}
