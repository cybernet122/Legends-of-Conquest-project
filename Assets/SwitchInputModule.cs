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
        UI.enabled = true;
        ShopUI.enabled = false;
        BattleUI.enabled = false;
    }

    public void SwitchToShopUI()
    {
        UI.enabled = false;
        ShopUI.enabled = true;
        BattleUI.enabled = false;
    }

    public void SwitchToBattleUI()
    {
        UI.enabled = false;
        ShopUI.enabled = false;
        BattleUI.enabled = true;
    }
}
