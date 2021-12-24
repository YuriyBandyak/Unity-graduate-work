using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It should looks like Spell class. Paste enums in Spell.
/// And array of spells add to PlayerStatsAndFucntion
/// </summary>
public class Spells : MonoBehaviour
{
    public enum MagicTypes { Default, Fire, Wind, Water, Earth };
    public enum MagicForms { Default, Directed, Cone, Wall, SelfCast, Ring, Ray};

    public class Spell
    {
        //Stats
        public MagicTypes currentType = MagicTypes.Default;
        public MagicForms currentForm = MagicForms.Default;

        public float power = 0;
        public float size = 0;
        public float requiredMana = 0;

        public float speed = 0;
        //public float coneRadius = 45; left from first concept
        public float distance = 5;
        public Vector3 firstPoint;
        public Vector3 lastPoint;

        public GameObject owner = null;

        public Spell(MagicTypes currentType, MagicForms currentForm, float power, float size, float requiredMana, float speed, float distance, Vector3 firstPoint, Vector3 lastPoint)
        {
            this.currentType = currentType;
            this.currentForm = currentForm;
            this.power = power;
            this.size = size;
            this.requiredMana = requiredMana;
            this.speed = speed;
            this.distance = distance;
            this.firstPoint = firstPoint;
            this.lastPoint = lastPoint;
        }

        public Spell(MagicTypes currentType, MagicForms currentForm, float power, float size, float requiredMana, float speed) : this(currentType, currentForm, power, size)
        {
            this.requiredMana = requiredMana;
            this.speed = speed;
        }

        public Spell(MagicTypes currentType, MagicForms currentForm, float power, float requiredMana)
        {
            this.currentType = currentType;
            this.currentForm = currentForm;
            this.power = power;
            this.requiredMana = requiredMana;
        }

        public Spell() { }

        

        public void SetOwner(GameObject ownerObject)
        {
            owner = ownerObject;
        }
    }

    public Spell[] spellslist;

    public bool IsSpellEnable(int n)
    {
        if (n <= spellslist.Length)
        {
            return true;
        }
        else return false;
    }

    public void SaveSpell(MagicTypes currentType, MagicForms currentForm, float power, float size, float requiredMana, float speed, float coneRadius, float distance, Vector3 firstPoint, Vector3 lastPoint)
    {
        spellslist[0] = new Spell(currentType, currentForm, power, size, requiredMana, speed, distance, firstPoint, lastPoint);
    }

    private void Start()
    {
        spellslist = new Spell[10];
    }
}
