using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BattleMagicButton : MonoBehaviour
{
    public string spellName;
    public int spellCost;

    public TextMeshProUGUI spellNameText, spellCostText;

    public void Press()
    {
        var player = BattleManager.instance.GetPlayer();
        if (player.currentMana >= spellCost) 
        {
            BattleManager.instance.magicPanel.SetActive(false);
            BattleManager.instance.OpenTargetPanel(spellName);
        }
    }
}
