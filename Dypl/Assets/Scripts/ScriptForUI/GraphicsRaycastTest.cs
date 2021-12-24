using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphicsRaycastTest : MonoBehaviour
{
    //stats

    public bool pointerOnReadySpell = false;


    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    private Spells.Spell spell;

    //references
    public GameObject buttonWithSpell;
    public GameObject player;

    public GraphicsRaycastTest spellCreationCanvas = null;

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
    }

    public void SetSpellName(int slot, string text)
    {
        transform.GetChild(0).GetChild(slot).Find("SpellName").GetComponent<Text>().text = text;
    }

    /// <summary>
    /// Check if mouse is above slots button
    /// </summary>
    /// <returns>Slot id</returns>
    public int CheckRaycastOnSlot() // return slot number
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach(RaycastResult result in results)
        {
            if(result.gameObject.tag == "QuickSlot")
            {
                switch (result.gameObject.name)
                {
                    case "Slot1":
                        return 0;
                    case "Slot2":
                        return 1;
                    case "Slot3":
                        return 2;
                    case "Slot4":
                        return 3;
                    case "Slot5":
                        return 4;
                }
            }
        }
        return -1;
    }

    //void sUpdate()
    //{
    //    if(pointerOnReadySpell == spellCreationCanvas.pointerOnReadySpell)
    //    pointerOnReadySpell = spellCreationCanvas.pointerOnReadySpell;

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        //Debug.Log("Try to raycast");
    //        //Set up the new Pointer Event
    //        m_PointerEventData = new PointerEventData(m_EventSystem);
    //        //Set the Pointer Event Position to that of the mouse position
    //        m_PointerEventData.position = Input.mousePosition;

    //        //Create a list of Raycast Results
    //        List<RaycastResult> results = new List<RaycastResult>();

    //        //Raycast using the Graphics Raycaster and mouse click position
    //        m_Raycaster.Raycast(m_PointerEventData, results);

    //        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
    //        foreach (RaycastResult result in results)
    //        {
    //            //Debug.Log("Ray hit:" + result.gameObject.name);
    //            if(result.gameObject.tag=="QuickSlot") Debug.Log("Ray hit:" + result.gameObject.name);

    //            if (result.gameObject.name == "Slot1" && buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().dragged && pointerOnReadySpell)
    //            {
    //                Debug.Log("Spell saved in slot1");
    //                spell = buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().spell;
    //                player.GetComponent<Spells>().spellslist[0] = spell;
    //                result.gameObject.transform.GetChild(0).GetComponent<Text>().text = spell.currentType.ToString() + spell.currentForm.ToString();
    //                pointerOnReadySpell = false;
    //            }
    //            if (result.gameObject.name == "Slot2" && buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().dragged)
    //            {
    //                spell = buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().spell;
    //                player.GetComponent<Spells>().spellslist[1] = spell;
    //                result.gameObject.transform.GetChild(0).GetComponent<Text>().text = spell.currentType.ToString() + spell.currentForm.ToString();
    //            }
    //            if (result.gameObject.name == "Slot3" && buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().dragged)
    //            {
    //                spell = buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().spell;
    //                player.GetComponent<Spells>().spellslist[2] = spell;
    //                result.gameObject.transform.GetChild(0).GetComponent<Text>().text = spell.currentType.ToString() + spell.currentForm.ToString();
    //            }
    //            if (result.gameObject.name == "Slot4" && buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().dragged)
    //            {
    //                spell = buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().spell;
    //                player.GetComponent<Spells>().spellslist[3] = spell;
    //                result.gameObject.transform.GetChild(0).GetComponent<Text>().text = spell.currentType.ToString() + spell.currentForm.ToString();
    //            }
    //            if (result.gameObject.name == "Slot5" && buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().dragged)
    //            {
    //                spell = buttonWithSpell.GetComponent<ReadySpellToQuickSlots>().spell;
    //                player.GetComponent<Spells>().spellslist[4] = spell;
    //                result.gameObject.transform.GetChild(0).GetComponent<Text>().text = spell.currentType.ToString() + spell.currentForm.ToString();
    //            }
    //            if (result.gameObject.name == "ReadySpell")
    //            {
    //                Debug.Log("Reycast hit ReadySpellButton");
    //                result.gameObject.GetComponent<ReadySpellToQuickSlots>().dragged = false;
    //                result.gameObject.transform.position = result.gameObject.GetComponent<ReadySpellToQuickSlots>().startPosition;
    //                pointerOnReadySpell = true;
    //            }

    //        }
    //        pointerOnReadySpell = false;
    //    }
    //}
}
