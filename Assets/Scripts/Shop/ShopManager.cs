using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    [SerializeField] GameObject[] mainShopButtons;
    [SerializeField] Button[] buyAndSellButtons;
    private GameObject activeItemGameObject;
    public bool canOpenShop;
    [SerializeField]float salePriceReductionInPercent;
    private int salePriceReduced;
    public ShopMenuState shopMenuState = ShopMenuState.mainPanel;
    public bool finishedCount = true;
    private static LTDescr delay;
    public int siblingIndex;
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
        shopMenu.SetActive(false);
        sellPanel.SetActive(false);
    }

    private void Update()
    {
        if (!finishedCount)
        {
            delay = LeanTween.delayedCall(0.7f, () =>
            {
                finishedCount = true;
            });
        }
    }

    public void OpenShopMenu(InputAction.CallbackContext context)
    {
        if (canOpenShop && context.canceled && shopMenuState == ShopMenuState.mainPanel && !shopMenu.activeInHierarchy && finishedCount
            && !GameManager.instance.dialogBoxOpened && !GameManager.instance.gameMenuOpened)
            OpenShopMenu(); 
    }

    public void OpenShopMenu()
    {
        MenuManager.instance.HideInfoText();
        shopMenu.SetActive(true);
        GameManager.instance.shopMenuOpened = true;
        currentGoldCoinText.text = GameManager.instance.currentGoldCoins.ToString();
        EventSystem.current.firstSelectedGameObject = mainShopButtons[0];
        UpdateItemsInShop(itemSlotBuyContainerParent, itemsForSale, false);
        Utilities.SetSelectedAndHighlight(mainShopButtons[0], mainShopButtons[0].GetComponent<Button>());
    }

    public void CloseShopMenu()
    {
        shopMenu.SetActive(false);
        EmptyText(0);
        shopMenuState = ShopMenuState.mainPanel;
        GameManager.instance.shopMenuOpened = false;
        DialogController.instance.StartDelay();
        finishedCount = false;
        SwitchActiveMap.instance.SwitchToUI();
        MenuManager.instance.ShowInfoText("Interact with shop");
    }

    public void OpenBuyPanel()
    {
        EmptyText(1);
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
        UpdateItemsInShop(itemSlotBuyContainerParent, itemsForSale,false);
        Invoke("ChangeFocusToBuyBrowsePanel",0.2f);
        Invoke("EnableItemToNavigate",0.2f);
    }

    public void OpenSellPanel()
    {
        EmptyText(2);
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
        UpdateItemsInShop(itemSlotSellContainerParent, Inventory.instance.GetItemList(), true);
        Invoke("ChangeFocusToSellBrowsePanel", 0.2f);
        Invoke("EnableItemToNavigate",0.2f);
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
            for (int i = 0; i < itemSlotBuyContainerParent.childCount; i++)
            {
                Destroy(itemSlotBuyContainerParent.GetChild(i).gameObject);
            }
            for (int i = 0; i < itemSlotSellContainerParent.childCount; i++)
            {
                Destroy(itemSlotSellContainerParent.GetChild(i).gameObject);
            }
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
            currentGoldCoinText.text = GameManager.instance.currentGoldCoins.ToString();
        }
    }

    public void SelectedBuyItem(ItemsManager itemToBuy)
    {
        selectedItem = itemToBuy;       
        buyItemName.text = selectedItem.itemName;
        buyItemDescription.text = selectedItem.itemDescription;
        buyItemValue.text = "Value: " + selectedItem.valueInCoins;
        shopMenuState = ShopMenuState.buyBrowsePanel;
    }

    public void SelectedSellItem(ItemsManager itemToSell, int index)
    {
        selectedItem = itemToSell;
        siblingIndex = index;
        if (!selectedItem)
            selectedItem = ItemsAssets.instance.GetItemsAsset(itemToSell.itemName);
        sellItemName.text = selectedItem.itemName;
        sellItemDescription.text = selectedItem.itemDescription;
        salePriceReduced = (int)(selectedItem.valueInCoins * (salePriceReductionInPercent / 100));
        sellItemValue.text = "Value: " + salePriceReduced;
        shopMenuState = ShopMenuState.sellBrowsePanel;
    }

    public void BuyItem()
    {
        if (selectedItem)
        {
            if (GameManager.instance.currentGoldCoins >= selectedItem.valueInCoins)
            {
                GameManager.instance.currentGoldCoins -= selectedItem.valueInCoins;
                Inventory.instance.AddItems(selectedItem, true);
                currentGoldCoinText.text = GameManager.instance.currentGoldCoins.ToString();
            }
        }
    }

    public void SellItem()
    {
        if (selectedItem)
        {
            GameManager.instance.currentGoldCoins += salePriceReduced;
            Inventory.instance.RemoveItem(selectedItem);
            currentGoldCoinText.text = GameManager.instance.currentGoldCoins.ToString();
            if (!selectedItem.isStackable || selectedItem.amount <= 0)
            {
                EmptyText(1);
                selectedItem = null;
            }
            else if (selectedItem.amount > 0)
            {
                LeanTween.delayedCall(0.1f, () =>
                Utilities.SetSelectedAndHighlight(itemSlotSellContainerParent.GetChild(siblingIndex).gameObject, itemSlotSellContainerParent.GetChild(siblingIndex).GetComponent<Button>())
                );
            }
            UpdateItemsInShop(itemSlotSellContainerParent, Inventory.instance.GetItemList(), true);
            Invoke("SelectNextItemToSell", 0.1f);
            if (shopMenuState == ShopMenuState.sellPanel)
                ReturnToPrevious();
        }
    }

    private void SelectNextItemToSell()
    {
        if (itemSlotSellContainerParent.childCount != 0 && !selectedItem)
        {
            Button button;
            if (siblingIndex - 1 > 0)
            button = itemSlotSellContainerParent.GetChild(siblingIndex - 1).GetComponent<Button>();
            else
            button = itemSlotSellContainerParent.GetChild(0).GetComponent<Button>();
            button.OnSelect(null);
            Utilities.SetSelectedAndHighlight(button.gameObject, button);
            var itemButton = button.GetComponent<ItemButton>();
            selectedItem = itemButton.itemOnButton;
            itemButton.Press();
        }
        else if(itemSlotSellContainerParent.childCount == 0)
        {
            ReturnToPrevious();
            EmptyText(2);
        }
    }

    //Shop Menu Navigation

    public enum ShopMenuState
    {
        mainPanel,
        buyBrowsePanel,
        buyPanel,
        sellBrowsePanel,
        sellPanel
    }

    private void EnableMainButtons(int index)
    {
        foreach (GameObject menu in mainShopButtons)
        {
            var menuButton = menu.GetComponent<Button>();
            var navigation = menuButton.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            menuButton.navigation = navigation;
        }
        Utilities.SetSelectedAndHighlight(mainShopButtons[index], mainShopButtons[index].GetComponent<Button>());
    }

    private void DisableMainButtons(int index)
    {
        foreach (GameObject menu in mainShopButtons)
        {
            var menuButton = menu.GetComponent<Button>();
            var navigation = menuButton.navigation;
            navigation.mode = Navigation.Mode.None;
            menuButton.navigation = navigation;
        }
        Invoke("HighlightButton",0.1f);
    }

    private void HighlightButton()
    {
        for (int i = 0; i < mainShopButtons.Length; i++)        
            mainShopButtons[i].GetComponent<Button>().OnDeselect(null);        
        if (buyPanel.activeInHierarchy)
            mainShopButtons[0].GetComponent<Button>().OnSelect(null);
        else
            mainShopButtons[1].GetComponent<Button>().OnSelect(null);
    }

    public void ChangeFocusToBuyBrowsePanel()
    {
        shopMenuState = ShopMenuState.buyBrowsePanel;
        DisableMainButtons(0);
        if (itemSlotBuyContainerParent.childCount != 0)
        {
            var itemButton = itemSlotBuyContainerParent.GetChild(0).GetComponent<Button>();
            Utilities.SetSelectedAndHighlight(itemButton.gameObject, itemButton);
            itemButton.onClick.Invoke();
        }
    }

    public void ChangeFocusToSellBrowsePanel()
    {
        shopMenuState = ShopMenuState.sellBrowsePanel;
        DisableMainButtons(1);
        if (itemSlotSellContainerParent.childCount != 0)
        {
            var itemSellButton = itemSlotSellContainerParent.GetChild(0).GetComponent<Button>();
            Utilities.SetSelectedAndHighlight(itemSellButton.gameObject, itemSellButton);
            itemSellButton.onClick.Invoke();
        }
    }


    public void NavigateMenu(InputAction.CallbackContext context)
    {
        if (!EventSystem.current.currentSelectedGameObject && context.canceled && shopMenu.activeInHierarchy)
        {
            switch (shopMenuState)
            {
                case ShopMenuState.buyBrowsePanel:
                    if(itemSlotBuyContainerParent.childCount > 0)
                    EventSystem.current.SetSelectedGameObject(itemSlotBuyContainerParent.GetChild(0).gameObject);
                    break;
                case ShopMenuState.buyPanel:
                    EventSystem.current.SetSelectedGameObject(buyAndSellButtons[0].gameObject);
                    break;
                case ShopMenuState.mainPanel:
                    EventSystem.current.SetSelectedGameObject(mainShopButtons[0]);
                    break;
                case ShopMenuState.sellBrowsePanel:
                    if (itemSlotSellContainerParent.childCount > 0)
                        EventSystem.current.SetSelectedGameObject(itemSlotSellContainerParent.GetChild(0).gameObject);
                    break;
                case ShopMenuState.sellPanel:
                        EventSystem.current.SetSelectedGameObject(buyAndSellButtons[1].gameObject);
                    break;
            }
        }
    }

    public void NavigateItems(InputAction.CallbackContext context)
    {
        if (context.canceled && shopMenu.activeInHierarchy)
        {
            if (buyPanel.activeInHierarchy || sellPanel.activeInHierarchy)
            {
                if (itemSlotBuyContainerParent.childCount != 0 || itemSlotSellContainerParent.childCount !=0)
                {
                    var button = EventSystem.current.currentSelectedGameObject.GetComponent<ItemButton>();
                    if (button)
                    {
                        button.Press();
                        selectedItem = button.itemOnButton;
                    }
                }
            }
        }
    }

    public void NavigateToBuyOrSell(InputAction.CallbackContext context)
    {
        if (shopMenu.activeInHierarchy)
        {
            if (shopMenuState == ShopMenuState.buyBrowsePanel && context.performed)
            {
                if (selectedItem)
                {
                    Invoke("SwitchToBuyPanel", 0.2f);
                }
            }
            if (shopMenuState == ShopMenuState.sellBrowsePanel && context.performed)
            {
                if (selectedItem)
                {
                    Invoke("SwitchToSellPanel", 0.2f);
                }
            }
        }
    }

    private void SwitchToBuyPanel()
    {
        shopMenuState = ShopMenuState.buyPanel;
        activeItemGameObject = EventSystem.current.currentSelectedGameObject;
        DisableItemNavigation(0);
        Utilities.SetSelectedAndHighlight(buyAndSellButtons[0].gameObject, buyAndSellButtons[0]);
        activeItemGameObject.GetComponent<Button>().OnSelect(null);
    }

    private void SwitchToSellPanel()
    {
        shopMenuState = ShopMenuState.sellPanel;
        activeItemGameObject = EventSystem.current.currentSelectedGameObject;
        DisableItemNavigation(1);
        Utilities.SetSelectedAndHighlight(buyAndSellButtons[1].gameObject, buyAndSellButtons[1]);
        activeItemGameObject.GetComponent<Button>().OnSelect(null);
    }

    private void DisableItemNavigation(int index)
    {
        foreach (Transform item in itemSlotBuyContainerParent)
        {
            var itemButton = item.GetComponent<Button>();
            var itemNavigation = itemButton.navigation;
            itemNavigation.mode = Navigation.Mode.None;
            itemButton.navigation = itemNavigation;
        }
        foreach (Transform item in itemSlotSellContainerParent)
        {
            var itemButton = item.GetComponent<Button>();
            var itemNavigation = itemButton.navigation;
            itemNavigation.mode = Navigation.Mode.None;
            itemButton.navigation = itemNavigation;
        }
    }

    private void EnableItemToNavigate()
    {
        foreach (Transform item in itemSlotBuyContainerParent)
        {
            var itemButton = item.GetComponent<Button>();
            var navigation = itemButton.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            itemButton.navigation = navigation;
        }
        foreach (Transform item in itemSlotSellContainerParent)
        {
            var itemButton = item.GetComponent<Button>();
            var navigation = itemButton.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            itemButton.navigation = navigation;
        }
    }

    public void ReturnToPrevious()
    {
        if (shopMenu.activeInHierarchy)
            switch (shopMenuState)
            {
                case ShopMenuState.buyBrowsePanel:
                    shopMenuState = ShopMenuState.mainPanel;
                    EnableMainButtons(0);
                    break;
                case ShopMenuState.buyPanel:
                    shopMenuState = ShopMenuState.buyBrowsePanel;
                    EnableItemToNavigate();
                    if (itemSlotBuyContainerParent.childCount != 0)
                    {
                        if (activeItemGameObject)
                        {
                            var itemBuyButton = activeItemGameObject;
                            Utilities.SetSelectedAndHighlight(itemBuyButton, itemBuyButton.GetComponent<Button>());
                        }
                        else
                        {
                            var itemBuyButton = itemSlotBuyContainerParent.GetChild(0).gameObject;
                            Utilities.SetSelectedAndHighlight(itemBuyButton, itemBuyButton.GetComponent<Button>());
                        }
                    }
                    else
                        ReturnToPrevious();
                    break;
                case ShopMenuState.sellBrowsePanel:
                    shopMenuState = ShopMenuState.mainPanel;
                    EnableMainButtons(1);
                    break;
                case ShopMenuState.sellPanel:
                    shopMenuState = ShopMenuState.sellBrowsePanel;
                    EnableItemToNavigate();
                    if (itemSlotSellContainerParent.childCount != 0)
                    {
                        var itemSellButton = itemSlotSellContainerParent.GetChild(0).gameObject;
                        Utilities.SetSelectedAndHighlight(itemSellButton, itemSellButton.GetComponent<Button>());
                    }
                    else
                        ReturnToPrevious();
                    break;
                case ShopMenuState.mainPanel:
                    CloseShopMenu();
                    break;
            }
    }

    public void ReturnToPrevious(InputAction.CallbackContext context)
    {
        if (context.canceled && !GameManager.instance.battleIsActive)
        {
            ReturnToPrevious();
        }
    }
}
