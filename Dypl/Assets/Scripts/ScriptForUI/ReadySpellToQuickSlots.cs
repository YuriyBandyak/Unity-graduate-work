using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReadySpellToQuickSlots : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //stats
    public Spells.Spell spell;
    public string text;

    private Vector3 startPosition;

    //references
    public GameObject quickSlotsCanvas;
    public GameObject player;

    private void Start()
    {
        startPosition = transform.position;
        gameObject.SetActive(false);
    }

    public void SetSpellName()
    {
        text = spell.currentType.ToString() + " " + spell.currentForm.ToString();
        transform.Find("Text").GetComponent<Text>().text = text;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPosition;
        switch (quickSlotsCanvas.GetComponent<GraphicsRaycastTest>().CheckRaycastOnSlot())
        {
            case -1:
                break;
            case 0:
                player.GetComponent<PlayerStatsAndFunction>().AddSpell(spell,0);
                quickSlotsCanvas.GetComponent<GraphicsRaycastTest>().SetSpellName(0, text);
                break;
            case 1:
                player.GetComponent<PlayerStatsAndFunction>().AddSpell(spell, 1);
                quickSlotsCanvas.GetComponent<GraphicsRaycastTest>().SetSpellName(1, text);
                break;
            case 2:
                player.GetComponent<PlayerStatsAndFunction>().AddSpell(spell, 2);
                quickSlotsCanvas.GetComponent<GraphicsRaycastTest>().SetSpellName(2, text);
                break;
            case 3:
                player.GetComponent<PlayerStatsAndFunction>().AddSpell(spell, 3);
                quickSlotsCanvas.GetComponent<GraphicsRaycastTest>().SetSpellName(3, text);
                break;
            case 4:
                player.GetComponent<PlayerStatsAndFunction>().AddSpell(spell, 4);
                quickSlotsCanvas.GetComponent<GraphicsRaycastTest>().SetSpellName(4, text);
                break;
        }
        if (quickSlotsCanvas.GetComponent<GraphicsRaycastTest>().CheckRaycastOnSlot() != -1) 
            Debug.Log("t/Spell: Spell saved to slot" + quickSlotsCanvas.GetComponent<GraphicsRaycastTest>().CheckRaycastOnSlot());
    }
}
