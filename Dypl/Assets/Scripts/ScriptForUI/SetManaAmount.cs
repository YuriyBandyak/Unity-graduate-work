using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetManaAmount : MonoBehaviour
{
    //reference
    public GameObject saveSpellButton;

    public void SetManaAmountFunc()
    {
        GetComponent<Text>().text = saveSpellButton.GetComponent<SaveSpellButton>().ManaAmount().ToString();
    }
}
