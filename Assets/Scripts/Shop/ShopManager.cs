using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopMenu, buyPanel, sellPanel;
    [SerializeField] TextMeshProUGUI currentGoldCoinText;
    public List<ItemsManager> itemsForSale;
    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] Transform itemSlotBuyContainerParent;
    [SerializeField] Transform itemSlotSellContainerParent;
    [SerializeField] ItemsManager selectedItem;
    [SerializeField] TextMeshProUGUI buyItemName, buyItemDescription, buyItemValue;
    [SerializeField] TextMeshProUGUI sellItemName, sellItemDescription, sellItemValue;
    int salePriceReduction;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        shopMenu.SetActive(false);
        buyPanel.SetActive(false);
        sellPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenShopMenu()
    {
        shopMenu.SetActive(true);
        GameManager.instance.shopMenuOpened = true;
        currentGoldCoinText.text = "Gold coins: " + GameManager.instance.currentGoldCoins;
        buyPanel.SetActive(true);
    }

    public void CloseShopMenu()
    {
        shopMenu.SetActive(false);
        EmptyText(0);
        GameManager.instance.shopMenuOpened = false;
        DialogController.instance.count = 0;
    }

    public void OpenBuyPanel()
    {
        EmptyText(1);
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        UpdateItemsInShop(itemSlotBuyContainerParent, itemsForSale,false);
    }

    public void OpenSellPanel()
    {
        EmptyText(2);
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
        UpdateItemsInShop(itemSlotSellContainerParent, Inventory.instance.GetItemList(), true);
    }

    public void EmptyText(int panel)
    {   
        if(panel == 1)
        {
            selectedItem = null;
            buyItemName.text = "";
            buyItemDescription.text = "";
            buyItemValue.text = "";
        }
        else if (panel == 2)
        {
            selectedItem = null;
            sellItemName.text = "";
            sellItemDescription.text = "";
            sellItemValue.text = "";
        }
        if(panel == 0)
        {
            selectedItem = null;
            buyItemName.text = "";
            buyItemDescription.text = "";
            buyItemValue.text = "";
            selectedItem = null;
            sellItemName.text = "";
            sellItemDescription.text = "";
            sellItemValue.text = "";
        }
    }

    private void UpdateItemsInShop(Transform itemSlotContainerParent, List<ItemsManager> itemsToLookThrough, bool isSelling)
    {
        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
        }
        foreach (ItemsManager item in itemsToLookThrough)
        {
            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>();
            Image itemImage = itemSlot.Find("Items Image").GetComponent<Image>();
            itemImage.sprite = item.itemImage;
            TextMeshProUGUI itemsAmountText = itemSlot.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            if (isSelling)
            {
                if (item.amount > 1)
                    itemsAmountText.text = item.amount.ToString();
                else
                    itemsAmountText.text = "";
            }
            else
            {
                itemsAmountText.text = "";
            }
            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
            currentGoldCoinText.text = "Gold Coins: " + GameManager.instance.currentGoldCoins.ToString();
        }
    }

    public void SelectedBuyItem(ItemsManager itemToBuy)
    {
        selectedItem = itemToBuy;
        buyItemName.text = selectedItem.itemName;
        buyItemDescription.text = selectedItem.itemDescription;
        buyItemValue.text = "Value: " + selectedItem.valueInCoins;
    }

    public void SelectedSellItem(ItemsManager itemToSell)
    {
        selectedItem = itemToSell;
        sellItemName.text = selectedItem.itemName;
        sellItemDescription.text = selectedItem.itemDescription;
        salePriceReduction = (int)(selectedItem.valueInCoins * 0.75);
        sellItemValue.text = "Value: " + salePriceReduction;
    }

    public void BuyItem()
    {
        if (selectedItem)
        {
            if (GameManager.instance.currentGoldCoins >= selectedItem.valueInCoins)
            {
                GameManager.instance.currentGoldCoins -= selectedItem.valueInCoins;
                Inventory.instance.AddItems(selectedItem, true);
                currentGoldCoinText.text = "Gold Coins: " + GameManager.instance.currentGoldCoins;
            }
        }
    }

    public void SellItem()
    {
        if (selectedItem)
        {
            GameManager.instance.currentGoldCoins += (int)salePriceReduction;
            Inventory.instance.RemoveItem(selectedItem);
            currentGoldCoinText.text = "Gold Coins: " + GameManager.instance.currentGoldCoins;
            if (!selectedItem.isStackable || selectedItem.amount <= 0)
            {
                EmptyText(1);
            }
            UpdateItemsInShop(itemSlotSellContainerParent, Inventory.instance.GetItemList(),true);
        }
    }
}
