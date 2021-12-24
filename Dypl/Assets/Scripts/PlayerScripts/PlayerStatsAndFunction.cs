using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStatsAndFunction : MonoBehaviour
{
    //enums
    public enum PlayerStates { idle, inFight};
    public enum CastType { none, quick, wall, ray, concentrated };

    //Player stats
    public float currentHP = 100;
    public float currentMP = 0;
    public int manaChannel = 10;

    public int playerMaxHP = 100;
    public int playerMaxMP = 100;
    public float manaRegenPerSec = 3;
    public float healthRegenPerSec = 0.5f;

    public PlayerStates state = PlayerStates.idle;
    private CastType castType = CastType.none;

    private int choosedSpellNumber = -1;
    private int lastChoosedSpell = -2;
    public bool pointerOnUI = false;

    private GameObject raySpellObject = null;
    private GameObject wallSpellObject = null;

    private List<GameObject> listOfActivableObjects;

    //References
    public GameObject spellCreationWindows = null;
    public GameObject playerStatsWindow = null;
    public GameObject spellStartPoint = null;
    public GameObject quickCastCanvas = null;

    public GameObject spellPointer;

    private GameObject spellCreationPanel = null;

    private void Start()
    {
        spellCreationPanel = spellCreationWindows.transform.GetChild(0).gameObject;
        //AddEffect(BuffsDebuffs.buffsDebuffs.fired, 5);
    }

    private void FixedUpdate() // default its 50/sec
    {
        if (currentMP < playerMaxMP) 
        {
            currentMP += (manaRegenPerSec / 50); 
        }
        if (currentHP < playerMaxHP)
        {
            currentHP += (healthRegenPerSec / 50);
        }
    }
    public void Update()
    {
        // showing pointer only during fight time
        if (state == PlayerStates.inFight) GetComponent<MouseController>().ShowPointer(); 

        // checking if mouse is above menus
        SetCurrentPointerStateOnUI();

        // Active/disactive interactable item
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject nearestObject = null;
            listOfActivableObjects = gameObject.transform.Find("RangeForAction").GetComponent<TriggerForActivableItems>().listOfActivableObject;
            if (listOfActivableObjects.Count > 1)
            {
                nearestObject = listOfActivableObjects[0];
                for (int i = 1; i < listOfActivableObjects.Count; i++)
                {
                    if(Vector3.Distance(gameObject.transform.position, listOfActivableObjects[i].transform.position) 
                        < Vector3.Distance(gameObject.transform.position, nearestObject.transform.position))
                    {
                        nearestObject = listOfActivableObjects[i];
                    }
                }
            }else if(listOfActivableObjects.Count == 1)
            {
                nearestObject = listOfActivableObjects[0];
            }
            if(nearestObject != null)
            {
                nearestObject.GetComponent<ActivibleItems>().Action();
            }
        }

        // Spell Creation Window
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!spellCreationWindows.activeInHierarchy)
            {
                spellCreationWindows.SetActive(true);
            }
            else
            {
                spellCreationWindows.SetActive(false);
            }
        }

        // Player Stats Window
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!playerStatsWindow.activeInHierarchy)
            {
                playerStatsWindow.SetActive(true);
            }
            else
            {
                playerStatsWindow.SetActive(false);
            }
        }


        // QuickSlots
        QuickSlotsKeys();

        //Casts
        Cast();

    }

    public void GetDamage(float dmg)
    {
        currentHP -= dmg;
    }

    private void QuickSlotsKeys()
    {
        bool isAlphaPressed = false;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            choosedSpellNumber = 0;
            isAlphaPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            choosedSpellNumber = 1;
            isAlphaPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            choosedSpellNumber = 2;
            isAlphaPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            choosedSpellNumber = 3;
            isAlphaPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            choosedSpellNumber = 4;
            isAlphaPressed = true;
        }

        if (isAlphaPressed)
        {
            if (lastChoosedSpell == choosedSpellNumber)
            {
                SetQuickSlot(choosedSpellNumber);
                choosedSpellNumber = -1;
                lastChoosedSpell = -2;
            }
            else
            {
                lastChoosedSpell = choosedSpellNumber;
                SetQuickSlot(choosedSpellNumber);
            }
        }
    }

    private void Cast()
    {
        if (choosedSpellNumber != -1 
            && GetComponent<Spells>().spellslist[choosedSpellNumber] != null 
            && pointerOnUI == false
            && state == PlayerStates.inFight)
        {
            if ( ( GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.Directed 
                || GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.Ring 
                || GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.SelfCast 
                || GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.Cone) 
                && Input.GetMouseButtonDown(0))
            {
                //QuickCast(choosedSpellNumber);
                QuickCast();
            }
            else if (GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.Wall && Input.GetMouseButtonDown(0))
            {
                CastSpell(choosedSpellNumber);
                WallCast();
                
            }
            if (GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.Wall && Input.GetMouseButtonUp(0) && wallSpellObject.GetComponent<WallForm>().preparingModeState==0)
            {
                GetComponent<PlayerMovement>().isPlayerCastSpell = true;
                transform.Find("Hand").GetChild(0).GetComponent<Animator>().SetBool("Casting", true);
                //GetComponent<MouseController>().lookAt();
            }
            else if(GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.Ray && Input.GetMouseButtonDown(0))
            {
                RayCast();
                GetComponent<MouseController>().lookAt();
            }
            else if(GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.Ray && Input.GetMouseButtonUp(0) && transform.Find("Hand").GetChild(0).GetComponent<Animator>().speed == 0)
            {
                transform.Find("Hand").GetChild(0).GetComponent<Animator>().speed = 1;
                transform.Find("Hand").GetChild(0).GetComponent<Animator>().SetBool("Casting", false);
                GetComponent<PlayerMovement>().isPlayerCastSpell = false;
                raySpellObject.GetComponent<RayForm>().Stop();
            }

            if (GetComponent<Spells>().spellslist[choosedSpellNumber].currentForm == Spells.MagicForms.Ray && Input.GetMouseButton(0))
            {
                GetComponent<MouseController>().lookAt();
            }
        }
    }

    private void QuickCast()
    {
        if(transform.Find("Hand").GetChild(0))// staff instantiated here
        {
            castType = CastType.quick;

            GetComponent<PlayerMovement>().isPlayerCastSpell = true;
            transform.Find("Hand").GetChild(0).GetComponent<Animator>().SetBool("Casting", true);
            GetComponent<MouseController>().lookAt();
        }
        
    }

    private void WallCast()
    {
        castType = CastType.wall;

    }

    private void RayCast()
    {
        castType = CastType.ray;

        GetComponent<PlayerMovement>().isPlayerCastSpell = true;
        transform.Find("Hand").GetChild(0).GetComponent<Animator>().SetBool("Casting", true);
        //GetComponent<MouseController>().lookAt();
    }

    //private GameObject raySpellObject = null;


    public void WaitForAnimation() // called by stuff animation event
    {
        //transform.Find("Hand").GetChild(0).GetComponent<Animator>().SetBool("Casting", false);
        //GetComponent<PlayerMovement>().playerCastSpell = false;
        Debug.Log("t/spell: Casting...");
        if (castType == CastType.quick)
        {
            CastSpell(choosedSpellNumber);

            transform.Find("Hand").GetChild(0).GetComponent<Animator>().SetBool("Casting", false);
            GetComponent<PlayerMovement>().isPlayerCastSpell = false;
        }
        else if (castType == CastType.wall)
        {
            wallSpellObject.GetComponent<WallForm>().preparingModeState = 1;

            transform.Find("Hand").GetChild(0).GetComponent<Animator>().SetBool("Casting", false);
            GetComponent<PlayerMovement>().isPlayerCastSpell = false;
        }
        else if (castType == CastType.ray)
        {
            CastSpell(choosedSpellNumber);

            transform.Find("Hand").GetChild(0).GetComponent<Animator>().speed = 0;
        }
    }

    /// <summary>
    /// Set current state for PointerOnUI - so spell cant be casted when you choose smth from menus
    /// </summary>
    private void SetCurrentPointerStateOnUI()
    {
        if (spellCreationWindows.activeInHierarchy)
        {
            if (!spellCreationPanel.GetComponent<NoAccessToCastingOnEnter>().pointerOnUI) { pointerOnUI = false; }
            else { pointerOnUI = true; }
        }
        else if (playerStatsWindow.activeInHierarchy)
        {
            if (!playerStatsWindow.transform.GetChild(0).GetComponent<NoAccessToCastingOnEnter>().pointerOnUI) { pointerOnUI = false; }
            else { pointerOnUI = true; }
        }
        else
        {
            pointerOnUI = false;
        }
    }

    

    /// <summary>
    /// Adding spell to spell list array
    /// </summary>
    public void AddSpell(Spells.Spell spell, int slot)
    {
        GetComponent<Spells>().spellslist[slot] = spell;
    }

    /// <summary>
    /// Take spell from spell list and instantiate it
    /// </summary>
    /// <param name="n">ID from spell list</param>
    private void CastSpell(int n)
    {
        Debug.Log("t/Spell: Trying to cast spell in slot" + n);
        Spells.Spell spell = new Spells.Spell();
        spell = GetComponent<Spells>().spellslist[n];
        if (spell != null && currentMP > spell.requiredMana)
        {
            GameObject typePrefab = null;
            GameObject spellObj = null;

            currentMP -= spell.requiredMana;

            switch (spell.currentForm)
            {
                case Spells.MagicForms.Directed:
                    GameObject directedFormPrefab = Resources.Load<GameObject>("MagicForms/DirectedFormPrefab");
                    Vector3 direction = spellPointer.transform.position;
                    switch (spell.currentType)
                    {
                        case Spells.MagicTypes.Fire:
                            typePrefab = Resources.Load<GameObject>("MagicType/FireBall2");
                            break;
                        case Spells.MagicTypes.Earth:
                            typePrefab = Resources.Load<GameObject>("MagicType/Earth");
                            break;
                        case Spells.MagicTypes.Water:
                            typePrefab = Resources.Load<GameObject>("MagicType/Waterball"); 
                            break;
                        case Spells.MagicTypes.Wind:
                            typePrefab = Resources.Load<GameObject>("MagicType/WindDirected");
                            break;
                    }
                    spellObj = Instantiate(directedFormPrefab, spellStartPoint.transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
                    spellObj.GetComponent<DirectedForm>().setStats(typePrefab, direction, gameObject);
                    break;

                case Spells.MagicForms.Cone:
                    GameObject coneFormPrefab = Resources.Load<GameObject>("MagicForms/ConeFormPrefab");
                    switch (spell.currentType)
                    {
                        case Spells.MagicTypes.Fire:
                            typePrefab = Resources.Load<GameObject>("MagicType/FireCone3");
                            break;
                        case Spells.MagicTypes.Earth:
                            typePrefab = Resources.Load<GameObject>("MagicType/EarthWall");
                            break;
                        case Spells.MagicTypes.Water:
                            typePrefab = Resources.Load<GameObject>("MagicType/Waterball");
                            break;
                        case Spells.MagicTypes.Wind:
                            typePrefab = Resources.Load<GameObject>("MagicType/WindDirected");
                            break;
                    }
                    spellObj = Instantiate(coneFormPrefab, spellStartPoint.transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
                    spellObj.GetComponent<ConeForm>().SetStats(spell, typePrefab, spellStartPoint.transform);
                    Debug.Log("t/spell: ConeSPell");
                    break;

                case Spells.MagicForms.Wall:
                    GameObject wallFormPrefab = Resources.Load<GameObject>("MagicForms/WallFormPrefab");
                    switch (spell.currentType)
                    {
                        case Spells.MagicTypes.Fire:
                            typePrefab = Resources.Load<GameObject>("MagicType/FireWall");
                            break;
                        case Spells.MagicTypes.Earth:
                            typePrefab = Resources.Load<GameObject>("MagicType/EarthWall");
                            break;
                        case Spells.MagicTypes.Water:
                            typePrefab = Resources.Load<GameObject>("MagicType/Waterball");
                            break;
                        case Spells.MagicTypes.Wind:
                            typePrefab = Resources.Load<GameObject>("MagicType/WindDirected");
                            break;
                    }
                    wallSpellObject = Instantiate(wallFormPrefab, spellPointer.transform.position, Quaternion.Euler(0, 0, 0));
                    wallSpellObject.GetComponent<WallForm>().SetStats(spell, typePrefab, spellPointer);
                    break;

                case Spells.MagicForms.SelfCast:
                    switch (spell.currentType)
                    {
                        case Spells.MagicTypes.Fire:
                            AddEffect(BuffsDebuffs.buffsDebuffs.fired, MagicBalance.SelfCast.duration); // change time to spell.time
                            break;
                        case Spells.MagicTypes.Water:
                            AddEffect(BuffsDebuffs.buffsDebuffs.wet, MagicBalance.SelfCast.duration);
                            break;
                        case Spells.MagicTypes.Earth:
                            AddEffect(BuffsDebuffs.buffsDebuffs.none, MagicBalance.SelfCast.duration);
                            Debug.Log("This spell not implemented yet");
                            break;
                        case Spells.MagicTypes.Wind:
                            AddEffect(BuffsDebuffs.buffsDebuffs.none, MagicBalance.SelfCast.duration);
                            Debug.Log("This spell not implemented yet");
                            break;
                    }
                    break;

                case Spells.MagicForms.Ring:
                    GameObject ringFormPrefab = Resources.Load<GameObject>("MagicForms/RingFormPrefab");
                    switch (spell.currentType)
                    {
                        case Spells.MagicTypes.Fire:
                            typePrefab = Resources.Load<GameObject>("MagicType/FireRing");
                            break;
                        case Spells.MagicTypes.Earth:
                            //typePrefab = Resources.Load<GameObject>("MagicType/EarthWall");
                            Debug.Log("This spell not implemented yet");
                            break;
                        case Spells.MagicTypes.Water:
                            //typePrefab = Resources.Load<GameObject>("MagicType/Waterball");
                            Debug.Log("This spell not implemented yet");
                            break;
                        case Spells.MagicTypes.Wind:
                            //typePrefab = Resources.Load<GameObject>("MagicType/WindDirected");
                            Debug.Log("This spell not implemented yet");
                            break;
                    }
                    spellObj = Instantiate(ringFormPrefab, transform.position + new Vector3(0,1,0), Quaternion.Euler(transform.rotation.eulerAngles));
                    spellObj.GetComponent<RingForm>().SetStats(spell, typePrefab);
                    Debug.Log("t/spell: RingSpell");
                    break;

                case Spells.MagicForms.Ray:
                    GameObject rayFormPrefab = Resources.Load<GameObject>("MagicForms/RayFormPrefab");
                    //raySpellObject = Instantiate(rayFormPrefab, spellStartPoint.transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
                    raySpellObject = Instantiate(rayFormPrefab, spellStartPoint.transform);
                    raySpellObject.transform.GetComponent<RayForm>().SetStats(spell);
                    break;
            }
        }
        else if (currentMP > spell.requiredMana)
        {
            Debug.Log("You dont have enough mana");
        }
        else Debug.Log("There is no spell");
    }

    private void SetQuickSlot(int id)
    {
        for (int i = 0; i < quickCastCanvas.transform.GetChild(0).transform.childCount; i++)
        {
            Button but = quickCastCanvas.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>();

            if (i == id && !but.interactable)
            {
                but.interactable = true;
            }
            else if(i == id && but.interactable)
            {
                but.interactable = false;
            }
            else
            {
                but.interactable = true;
            }
        }
    }

    public void AddEffect(BuffsDebuffs.buffsDebuffs choosedEffect, float timeForEffect)
    {
        GetComponent<BuffsDebuffs>().AddEffect(choosedEffect, timeForEffect);
    }
}
