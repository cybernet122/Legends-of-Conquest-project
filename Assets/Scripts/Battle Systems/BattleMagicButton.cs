using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class BattleMagicButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerExitHandler, IDeselectHandler
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
            if (spellName != "Tentacles")
            {
                BattleManager.instance.OpenTargetPanel(spellName);
                BattleManager.instance.abilityChosen = gameObject;
            }
            else
                BattleManager.instance.AreaOfEffectAttack();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (spellName == "Tentacles")
            BattleManager.instance.ShowRingsOnAll();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (spellName == "Tentacles")
            BattleManager.instance.ShowRingsOnAll();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (spellName == "Tentacles")
            BattleManager.instance.HideAllRings();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (spellName == "Tentacles")
            BattleManager.instance.HideAllRings();
    }
}
