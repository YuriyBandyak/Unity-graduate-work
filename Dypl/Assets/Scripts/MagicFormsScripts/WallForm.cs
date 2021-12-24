using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for wall form of magic
/// what shoud be added: checking if place for spell if free(from player, and mb other spell(especialy another walls))
/// </summary>
public class WallForm : MonoBehaviour
{
    //stats
    private Spells.Spell spell;
    public int preparingModeState = 0; // 0 - standart mode "preparing", 1 - makes wall objects , 2 - the end - do nothing, 3 - delete object
    private GameObject magicType;
    private GameObject spellPointer;

    public float distance = -1; // private

    //public bool inPrepState = true;

    //public Transform mousePosition;

    //references
    private GameObject startPoint, endPoint;
    private LineRenderer line;
    public float power;

    private void Start()
    {
        power = MagicBalance.Wall.power;
        startPoint = transform.GetChild(0).gameObject;
        endPoint = transform.GetChild(1).gameObject;
        line = transform.GetChild(2).GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(0, 0.5f, 0));
        distance = MagicBalance.Wall.period;
    }

    private void Update()
    {
        if (preparingModeState == 0)
        {
            endPoint.transform.position = Vector3.MoveTowards(startPoint.transform.position, spellPointer.transform.position + new Vector3(0,1,0), distance);
            line.SetPosition(1, new Vector3(endPoint.transform.localPosition.x, 1, endPoint.transform.localPosition.z));
        }
        else if(preparingModeState == 1)
        {
            SetSupportiveObjectInvisible();
            SetTypePrefabs();
            preparingModeState = 2;
        }
        if (Input.GetMouseButtonUp(0))
        {
            preparingModeState = 1;
        }
    }

    private void SetSupportiveObjectInvisible()
    {
        startPoint.SetActive(false);
        Object.Destroy(line);
        endPoint.SetActive(false);
    }

    private void SetTypePrefabs()
    {
        float distance = Vector3.Distance(startPoint.transform.position, endPoint.transform.position);
        float n = distance / 1.5f; //devide by prefabType size
        float step = 1 / n;
        for(float i = 0; i < 1;)
        {
            Vector3 vect = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, i);
            Instantiate(magicType, vect, Quaternion.Euler(0, Random.Range(0,360), 0), gameObject.transform);
            i += step;
        }
    }

    public void ChangeModeToCasted()
    {
        preparingModeState = 1;
    }

    public void SetStats(Spells.Spell spell, GameObject magicType, GameObject spellPointer)
    {
        this.spell = spell;
        this.magicType = magicType;
        this.spellPointer = spellPointer;
    }
}
