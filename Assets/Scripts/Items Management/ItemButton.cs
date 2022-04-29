using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            if (itemOnButton.amountOfEffect != 0)
                MenuManager.instance.itemDescription.text = itemOnButton.itemDescription + "(Restores " + itemOnButton.amountOfEffect + " " + itemOnButton.affectType + ")";
            if (itemOnButton != null)
                MenuManager.instance.activeItem = itemOnButton;
            else
            {
                itemOnButton = ItemsAssets.instance.GetItemsAsset(itemOnButton.itemName);
                MenuManager.instance.activeItem = itemOnButton;
            }
        }
        else if (ShopManager.instance.shopMenu.activeInHierarchy)
        {
            if (ShopManager.instance.buyPanel.activeInHierarchy)
            {
                ShopManager.instance.SelectedBuyItem(itemOnButton);
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
            else if (ShopManager.instance.sellPanel.activeInHierarchy)
            {                
                ShopManager.instance.SelectedSellItem(itemOnButton, transform.GetSiblingIndex());
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
        else if (BattleManager.instance.itemPanel.activeInHierarchy)
        {
            if(itemOnButton != null)
                BattleManager.instance.SelectedItemToUse(itemOnButton);
            else
            {
                itemOnButton = ItemsAssets.instance.GetItemsAsset(itemOnButton.itemName);
                BattleManager.instance.SelectedItemToUse(itemOnButton);
            }
        }
    }

}
