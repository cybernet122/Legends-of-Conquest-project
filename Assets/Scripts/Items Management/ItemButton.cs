using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemButton : MonoBehaviour
{
    public ItemsManager itemOnButton;
    public Image itemsImage;
    public TextMeshProUGUI itemAmountText;

    public void Press()
    {
        if (MenuManager.instance.menu.activeInHierarchy)
        {
            MenuManager.instance.itemName.text = itemOnButton.itemName;
            MenuManager.instance.itemDescription.text = itemOnButton.itemDescription;
            MenuManager.instance.activeItem = itemOnButton;
        }
        if (ShopManager.instance.shopMenu.activeInHierarchy)
        {
            if (ShopManager.instance.buyPanel.activeInHierarchy)
            {
                ShopManager.instance.SelectedBuyItem(itemOnButton);
            }
            else if (ShopManager.instance.sellPanel.activeInHierarchy)
            {
                ShopManager.instance.SelectedSellItem(itemOnButton);
            }
        }
        if (BattleManager.instance.itemPanel.activeInHierarchy)
        {
            BattleManager.instance.SelectedItemToUse(itemOnButton);
        }
    }

}
