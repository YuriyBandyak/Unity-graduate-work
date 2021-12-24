using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSpellButton : MonoBehaviour
{
    //Stats
    public Spells.MagicTypes currentType = Spells.MagicTypes.Default;
    public Spells.MagicForms currentForm = Spells.MagicForms.Default;
    public int requiredMana = 0;


    //References
    public Slider powerSlider;
    public Component spel;
    public GameObject buttonToDrag;
    public GameObject player;

    public float ManaAmount()
    {
        float requiredManaF = 0;
        if (currentForm != Spells.MagicForms.Default && currentType != Spells.MagicTypes.Default)
        {
            requiredManaF = player.GetComponent<PlayerStatsAndFunction>().playerMaxMP / 10;
        }
        return requiredManaF;
    }

    public float DamageAmount()
    {
        float spellDamage = player.GetComponent<PlayerStatsAndFunction>().playerMaxMP / 10;
        return spellDamage;
    }
    public void SaveSpellFunc()
    {
        if(currentForm != Spells.MagicForms.Default && currentType != Spells.MagicTypes.Default)
        {
            buttonToDrag.GetComponent<ReadySpellToQuickSlots>().spell = new Spells.Spell(
               currentType,
               currentForm,
               DamageAmount(),
               10,
               ManaAmount(),
               1
               );
            Debug.Log("t/Spell: " + currentForm.ToString() + currentType.ToString() + " spell saved");
            buttonToDrag.GetComponent<ReadySpellToQuickSlots>().SetSpellName();
            buttonToDrag.SetActive(true);
        }
        else
        {
            Debug.Log("Dont use default form or type");
        }
    }
    
    
    public void ChangeType(int id) // enum doesnt show up in button options
    {
        switch(id){
            case 0:
                currentType = Spells.MagicTypes.Fire;
                break;
            case 1:
                currentType = Spells.MagicTypes.Wind;
                break;
            case 2:
                currentType = Spells.MagicTypes.Water;
                break;
            case 3:
                currentType = Spells.MagicTypes.Earth;
                break;
        }
    }

    public void ChangeForm(int id)
    {
        switch (id)
        {
            case 0:
                currentForm = Spells.MagicForms.Directed;
                break;
            case 1:
                currentForm = Spells.MagicForms.Cone;
                break;
            case 2:
                currentForm = Spells.MagicForms.Wall;
                break;
            case 3:
                currentForm = Spells.MagicForms.SelfCast;
                break;
            case 4:
                currentForm = Spells.MagicForms.Ring;
                break;
            case 5:
                currentForm = Spells.MagicForms.Ray;
                break;
        }
    }
}
