using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchInputModule : MonoBehaviour
{
    [SerializeField] BaseInputModule UI;
    [SerializeField] BaseInputModule ShopUI;
    [SerializeField] BaseInputModule BattleUI;

    public void SwitchToUI()
    {
        if (!UI.isActiveAndEnabled)
        {
            BattleUI.enabled = false;
            UI.enabled = true;
            ShopUI.enabled = false;
        }
    }

    public void SwitchToShopUI()
    {
        if (!ShopUI.isActiveAndEnabled)
        {
            BattleUI.enabled = false;
            UI.enabled = false;
            ShopUI.enabled = true;
        }
    }

    public void SwitchToBattleUI()
    {
        if (!BattleUI.isActiveAndEnabled)
        {
            BattleUI.enabled = true;
            ShopUI.enabled = false;
            UI.enabled = false;
        }
    }
}
