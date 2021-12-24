using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Should write it in playerstatsandfunction class
/// </summary>
public class SetStatsToUI : MonoBehaviour
{
    //references
    public Text hpAmount, mpAmount;

    private void Update()
    {
        hpAmount.text = Mathf.RoundToInt(GetComponent<PlayerStatsAndFunction>().currentHP) + "/" + GetComponent<PlayerStatsAndFunction>().playerMaxHP.ToString();
        mpAmount.text = Mathf.RoundToInt(GetComponent<PlayerStatsAndFunction>().currentMP) + "/" + GetComponent<PlayerStatsAndFunction>().playerMaxMP.ToString();
    }
}
