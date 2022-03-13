using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image image;
    public GameObject menu;
    [SerializeField] GameObject[] statsButtons;
    [SerializeField] TextMeshProUGUI[] nameInfoText, hpInfoText, manaInfoText, xpInfoText, playerInfoLevel, currentXPText;
    [SerializeField] Slider[] xpInfoSlider;
    [SerializeField] Image[] characterInfoImage;
    [SerializeField] GameObject[] characterInfoPanel;
    [SerializeField] TextMeshProUGUI nameText, hpText, manaText, statDex, statDef, xpText, playerLevel, statEquippedWeapon, statEquippedArmor, statWeaponPower, statArmorDefense, speed, evasion, currentQuest;
    [SerializeField] Slider xpSlider;
    [SerializeField] Image characterImage;
    [SerializeField] GameObject characterPanel, itemsPanel, itemContainer, charInfoList;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] Button useButton, discardButton;
    [SerializeField] GameObject characterChoicePanel, warningPanel;
    [SerializeField] TextMeshProUGUI[] itemsCharacterChoiceNames;
    [SerializeField] VerticalLayoutGroup characterButtons;
    [SerializeField] HorizontalLayoutGroup abilities;
    [SerializeField] Image[] abilitiesIcons;
    [SerializeField] QuestUpdate questUpdate;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] AbilityInfoManager abilityInfoManager;
    [SerializeField] GameObject[] mainMenuButtons;
    [SerializeField] SwitchInputModule eventSystem;
    public static MenuManager instance;
    public TextMeshProUGUI itemName, itemDescription;
    public ItemsManager activeItem;
    private GameObject activeItemGameObject;
    PlayerStats[] playerStats;
    bool toglMenu, toglItems, toglStats, toglWarning, toglOptions = false;
    int currentlyViewing;
    ItemsManager[] itemsCollection;
    public MenuState menuState = MenuState.mainPanels;
    private InputActionMap UI, ShopUI,BattleUI;
    public PlayerInput playerInput,battleInput;
    public static event UnityAction CloseMenu;
    public bool GetInfoPanelActive() { return charInfoList.activeInHierarchy; }
    public bool GetStatPanelActive() { return characterPanel.activeInHierarchy; }

    private void Start()
    {    
        instance = this;   
        menu.SetActive(false);
        characterPanel.SetActive(false);
        CloseCharacterChoicePanel();
        itemDescription.text = "";
        itemName.text = "";
        optionsPanel.SetActive(false);
        itemsCollection = FindObjectsOfType<ItemsManager>();
        playerInput = GetComponent<PlayerInput>();
        eventSystem.GetComponent<SwitchInputModule>();
        UI = playerInput.actions.FindActionMap("UI");
        ShopUI = playerInput.actions.FindActionMap("ShopUI");
        BattleUI = playerInput.actions.FindActionMap("BattleUI");
        Invoke("GetBattleInput", 0.1f);
        Invoke("SwitchToUI",0.1f);
    }

    private void GetBattleInput()
    {
        battleInput = BattleManager.instance.GetComponent<PlayerInput>();
    }

    public void SwitchToUI()
    {
        UI.Enable();
        ShopUI.Disable();
        BattleUI.Disable();
        eventSystem.SwitchToUI();
        if (!playerInput.enabled)
            playerInput.enabled = true;
        if (battleInput.enabled)
            battleInput.enabled = false;
        print("Switching to UI");
    }

    public void SwitchToShopUI()
    {
        UI.Disable();
        ShopUI.Enable();
        BattleUI.Disable();
        eventSystem.SwitchToShopUI();
        if (!playerInput.enabled)
            playerInput.enabled = true;
        if (battleInput.enabled)
            battleInput.enabled = false;
        print("Switching to shopUI");
    }

    public void SwitchToBattleUI()
    {
        UI.Disable();
        ShopUI.Disable();
        BattleUI.Enable();
        eventSystem.SwitchToBattleUI();
        playerInput.enabled = false;
        battleInput.enabled = true;
        print("Switching to BattleUI");
    }


    public void FadeImage()
    {
        image.GetComponent<Animator>().SetTrigger("StartFade");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && warningPanel.activeInHierarchy)
        {
            warningPanel.SetActive(false);
            return;
        }
        else if(Input.GetButtonDown("Cancel") && characterChoicePanel.activeInHierarchy)
        {
            CloseCharacterChoicePanel();
        }
        /*        else if (Input.GetButtonDown("Cancel") && !ShopManager.instance.shopMenu.activeInHierarchy && 
                    !GameManager.instance.dialogBoxOpened && !GameManager.instance.battleIsActive) //Toggle Menu
                {
                    ToggleMenu();            
                }   */
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (!ShopManager.instance.shopMenu.activeInHierarchy && menuState == MenuState.mainPanels &&
                    !GameManager.instance.dialogBoxOpened && !GameManager.instance.battleIsActive)
        {
            if (context.canceled)
            {
                if(ShopUI.enabled)
                Invoke("SwitchToUI",0.1f);
                ToggleMenu();
            }
        }
    }

    public void ToggleMenu()
    {
        if (characterChoicePanel.activeInHierarchy)
            characterChoicePanel.SetActive(false);
        if (warningPanel.activeInHierarchy)
            warningPanel.SetActive(false);
        toglMenu = !toglMenu;
        UpdateStats();
        menu.SetActive(toglMenu);
        GameManager.instance.gameMenuOpened = toglMenu;
        itemsPanel.SetActive(false);
        characterPanel.SetActive(false);
        charInfoList.SetActive(true);
        optionsPanel.SetActive(false);
        toglItems = false;
        toglStats = false;
        toglOptions = false;
        GameManager.instance.UpdatePlayerStats();
        currentQuest.text = "Current Quest: " + QuestManager.instance.GetCurrentQuest();
        DialogController.instance.StartDelay();
        var firstSelectedGameObject = EventSystem.current.firstSelectedGameObject;
        if (firstSelectedGameObject != mainMenuButtons[0])
            EventSystem.current.firstSelectedGameObject = mainMenuButtons[0];
        var button = mainMenuButtons[0].GetComponent<Button>();
        button.OnSelect(null);
        EnableMainButtons(0);
        menuState = MenuState.mainPanels;
        Invoke("CheckForShop", 0.1f);
    }

    private void CheckForShop()
    {
        if (!menu.activeInHierarchy && UI.enabled)
            CloseMenu?.Invoke();
    }

    public void ToggleItems()
    {
        toglItems = !toglItems;
        UpdateStats();
        if (characterChoicePanel.activeInHierarchy)
            characterChoicePanel.SetActive(false);
        characterPanel.SetActive(false);
        optionsPanel.SetActive(false);
        charInfoList.SetActive(!toglItems);
        toglStats = false;
        toglOptions = false;
        itemsPanel.SetActive(toglItems);
        if(toglItems)
        {
            menuState = MenuState.itemPanel;  
            if(itemSlotContainerParent.childCount != 0)            
                Utilities.SetSelectedAndHighlight(itemSlotContainerParent.GetChild(0).gameObject, itemSlotContainerParent.GetComponentInChildren<Button>());            
        }
        else
        {
            menuState = MenuState.mainPanels;
            if(mainMenuButtons[0].GetComponent<Button>().navigation.mode != Navigation.Mode.Automatic)
                EnableMainButtons(0);
        }
    }
    public void ToggleStats()
    {
        toglStats = !toglStats;

        if (characterChoicePanel.activeInHierarchy)
            characterChoicePanel.SetActive(false);
        itemsPanel.SetActive(false);
        optionsPanel.SetActive(false);
        charInfoList.SetActive(!toglStats);
        toglOptions = false;
        toglItems = false;
        if (toglStats)
        {
            UpdateStats();
            if (GameManager.instance.GetPlayerStats().Length == 1)
                characterButtons.childControlHeight = false;
            else
                characterButtons.childControlHeight = true;
        }
        else
        {
            menuState = MenuState.mainPanels;
            if (mainMenuButtons[1].GetComponent<Button>().navigation.mode != Navigation.Mode.Automatic)
                EnableMainButtons(1);
        }
        characterPanel.SetActive(toglStats);
    }

    public void UpdateStats()
    {
        playerStats = GameManager.instance.GetPlayerStats();
        if (!QuestManager.instance.CheckIfComplete("Look for the heroes located in the cave and join them"))
        {
            foreach (GameObject characterPanel in characterInfoPanel)
                characterPanel.SetActive(false);
        }
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (characterInfoPanel.Length <= i) { break; }
            characterInfoPanel[i].SetActive(true);
            nameInfoText[i].text = playerStats[i].playerName;
            characterInfoImage[i].sprite = playerStats[i].characterImage;
            hpInfoText[i].text = "Health: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
            manaInfoText[i].text = "Magic: " + playerStats[i].currentMana + "/" + playerStats[i].maxMana;
            //currentXPText[i].text = "Current XP: " + playerStats[i].currentXP;
            playerInfoLevel[i].text = playerStats[i].playerLevel.ToString();
            xpInfoSlider[i].minValue = 0;
            xpInfoSlider[i].value = playerStats[i].currentXP;
            xpInfoSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpInfoText[i].text = playerStats[i].currentXP.ToString() + " / " + playerStats[i].xpForNextLevel[playerStats[i].playerLevel].ToString();
            if (playerStats[i].playerLevel >= playerStats[i].maxLevel)
            {
                xpInfoSlider[i].minValue = 0;
                xpInfoSlider[i].value = 1;
                xpInfoSlider[i].maxValue = 1;
                xpInfoText[i].text = "0 / 0";
                currentXPText[i].text = "";
                return;
            }
        }
        StatsMenu();
        StatsMenuUpdate(currentlyViewing);
    }

    public void StatsMenu()
    {
        for (int i = 0; i < playerStats.Length; i++)
        {
            statsButtons[i].SetActive(true);
            statsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerStats[i].playerName;
        }
        StatsMenuUpdate(0);
    }

    public void StatsMenuUpdate(int playerSelectedNumber)
    {
        currentlyViewing = playerSelectedNumber;
        PlayerStats playerSelected = playerStats[playerSelectedNumber];
        nameText.text = playerSelected.playerName;
        hpText.text = "Health: " + playerSelected.currentHP.ToString() + " / " + playerSelected.maxHP;
        manaText.text = "Magic: " + playerSelected.currentMana.ToString() + " / " + playerSelected.maxMana;
        statDex.text = "Dexterity: " + playerSelected.dexterity.ToString();
        statDef.text = "Defense: " + playerSelected.defense.ToString();
        characterImage.sprite = playerSelected.characterImage;
        playerLevel.text = playerSelected.playerLevel.ToString();
        xpSlider.minValue = 0;
        xpSlider.value = playerSelected.currentXP;
        if (playerSelected.playerLevel >= playerSelected.maxLevel)
        {
            xpSlider.minValue = 0;
            xpSlider.value = 1;
            xpSlider.maxValue = 1;
            xpText.text = "0 / 0";
            return;
        }
        xpSlider.maxValue = playerSelected.xpForNextLevel[playerStats[playerSelectedNumber].playerLevel];
        xpText.text = playerSelected.currentXP.ToString() + " / " + playerSelected.xpForNextLevel[playerStats[playerSelectedNumber].playerLevel].ToString();
        if (playerSelected.equippedWeaponName != "")
            statEquippedWeapon.text = playerSelected.equippedWeaponName;
        else
            statEquippedWeapon.text = "(Nothing equipped)";
        if (playerSelected.equippedArmorName != "")
            statEquippedArmor.text = playerSelected.equippedArmorName;
        else
            statEquippedArmor.text = "(Nothing equipped)";
        statWeaponPower.text = "Weapon Power: " + playerSelected.weaponPower.ToString();
        statArmorDefense.text = "Armor Defense: " + playerSelected.armorDefense.ToString();
        speed.text = "Turn Speed: " + playerSelected.turnSpeed.ToString();
        evasion.text = "Evasion: " + playerSelected.evasion.ToString() + "%";
        abilityInfoManager.SetAbilitiesOfCharacter(playerSelected);
    }

    public void UpdateItemsInventory()
    {
        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
        }
        foreach (ItemsManager item in Inventory.instance.GetItemList())
        {
            RectTransform itemSlot = Instantiate(itemContainer, itemSlotContainerParent).GetComponent<RectTransform>();
            Image image = itemSlot.Find("Items Image").GetComponent<Image>();
            image.sprite = item.itemImage;
            TextMeshProUGUI itemAmountText = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
            if (item.amount > 1)
            { itemAmountText.text = item.amount.ToString(); }
            else
            { itemAmountText.text = ""; }
            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
            
        }
        Invoke("ChangeFocusToItems",0.1f);
    }

    public void ExitToMainMenu()
    {
        warningPanel.SetActive(false);
        GameManager.instance.gameMenuOpened = false;
        SceneManager.LoadScene(0);
        GameManager.instance.EmptyPlayerStats();
    }

    public void DiscardItem()
    {
        if (activeItem)
        {
            Inventory.instance.RemoveItem(activeItem);
            AudioManager.instance.PlaySFX(4);
        }
    }

    public void UseItem(int characterToUse)
    {
        PlayerStats player = GameManager.instance.GetPlayerStats()[characterToUse];
        if (activeItem.itemName == player.equippedWeaponName || activeItem.itemName == player.equippedArmorName) { return; }
        activeItem.UseItem(characterToUse);
        OpenCharacterChoicePanel();
        Inventory.instance.RemoveItem(activeItem);
        AudioManager.instance.PlaySFX(0);
        itemDescription.text = string.Empty;
        itemName.text = string.Empty;
    }

    public void OpenCharacterChoicePanel()
    {
        if (activeItem)
        {
            characterChoicePanel.SetActive(true);

            for (int i = 0; i < playerStats.Length; i++)
            {
                PlayerStats activePlayer = GameManager.instance.GetPlayerStats()[i];
                itemsCharacterChoiceNames[i].text = activePlayer.playerName;
                bool activePlayerAvailable = activePlayer.gameObject.activeInHierarchy;
                itemsCharacterChoiceNames[i].transform.parent.gameObject.SetActive(activePlayerAvailable);
            }
            itemsCharacterChoiceNames[0].GetComponentInParent<Button>().OnSelect(null);
            EventSystem.current.SetSelectedGameObject(itemsCharacterChoiceNames[0].transform.parent.gameObject);
        }
    }
    public void CloseCharacterChoicePanel()
    {
        characterChoicePanel.SetActive(false);
    }

    public void FindItem(ItemsManager itemToFind)
    {        
        for (int i = 0; i < itemsCollection.Length; i++)
        {
            if(itemsCollection[i].itemType == itemToFind.itemType && itemsCollection[i].itemName == itemToFind.itemName)
            {                
                MenuManager.instance.activeItem = itemsCollection[i];
                return;
            }
        }
        var itemList = Inventory.instance.GetItemList();
        foreach(ItemsManager itemInInventory in itemList)
        {
            if(itemInInventory.itemName == itemToFind.itemName)
            {
                GameObject itemGO = new GameObject(itemInInventory.itemName);
                ItemsManager im = itemGO.AddComponent<ItemsManager>();

                im.itemType = itemInInventory.itemType;
                im.itemName = itemInInventory.itemName;
                im.itemDescription = itemInInventory.itemDescription;
                im.valueInCoins = itemInInventory.valueInCoins;
                im.itemImage = itemInInventory.itemImage;
                im.amountOfEffect = itemInInventory.amountOfEffect;
                im.affectType = itemInInventory.affectType;
                im.weaponDexterity = itemInInventory.weaponDexterity;
                im.armorDefense = itemInInventory.armorDefense;
                im.isStackable = itemInInventory.isStackable;
                im.amount = 1;

                MenuManager.instance.activeItem = im;
            }  
        }
        
        /*
        foreach (ItemsManager item in itemList)
        {
            if (ItemsAssets.instance.GetItemsAsset(item.itemName) != null)
            {
                item.itemName = itemList[0].itemName;
                *//*var newItem = Instantiate(item);
                activeItem = newItem;*//*
            }
        }*/
    }


    public void OptionsMenu()
    {
        toglOptions = !toglOptions;
        characterPanel.SetActive(false);
        charInfoList.SetActive(!toglOptions);
        if (characterChoicePanel.activeInHierarchy)
            characterChoicePanel.SetActive(false);
        itemsPanel.SetActive(false);
        toglStats = false;
        toglItems = false;
        optionsPanel.gameObject.SetActive(toglOptions);
        if (!toglOptions)
        {
            menuState = MenuState.mainPanels;
            if (mainMenuButtons[2].GetComponent<Button>().navigation.mode != Navigation.Mode.Automatic)
                EnableMainButtons(2);
        }
        /*if (PlayerPrefs.HasKey("Difficulty_"))*/ //Check if slider works
    }

    public void SaveButton()
    {
        GameManager.instance.SaveData();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void FadeOut()
    {
        image.GetComponent<Animator>().SetTrigger("EndFade");
    }

    public void ToggleExitWarning()
    {
        toglWarning = !toglWarning;
        warningPanel.SetActive(toglWarning);
        if(toglWarning)
            mainMenuButtons[5].GetComponent<Button>().OnSelect(null);
        else
        {
            menuState = MenuState.mainPanels;
            if (mainMenuButtons[5].GetComponent<Button>().navigation.mode != Navigation.Mode.Automatic)
                EnableMainButtons(5);
        }
    }

    public void UpdateQuest(string quest)
    {
        questUpdate.PlayUpdateAnimation(quest);
        GameManager.instance.UpdatePlayerLevels();
    }

    public void UpdateAbilitiesInfo(int characterToUse)
    {
        BattleCharacters[] playerPrefabs = BattleManager.instance.ReturnPlayerPrefabs();
        PlayerStats[] players = GameManager.instance.GetPlayerStats();
        if (playerPrefabs[characterToUse].characterName == players[characterToUse].playerName) 
        {
            if (playerPrefabs[characterToUse].AttackMovesAvailable().Length < 4)
                abilities.childControlWidth = false;
            else
                abilities.childControlWidth = true;
        }
    }

    // Controls

    public enum MenuState
    {
        mainPanels,
        itemPanel,
        itemUsePanel,
        itemCharacterChoicePanel,
        statPanel,
        optionsPanel,
        exitPanel
    }    

    private void EnableMainButtons(int index)
    {
        foreach (GameObject menu in mainMenuButtons)
        {
            var menuButton = menu.GetComponent<Button>();
            var navigation = menuButton.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            menuButton.navigation = navigation;
        }
        Utilities.SetSelectedAndHighlight(mainMenuButtons[index],mainMenuButtons[index].GetComponent<Button>());
    }

    private void DisableMainButtons(int index)
    {
        foreach (GameObject menu in mainMenuButtons)
        {
            var menuButton = menu.GetComponent<Button>();
            var navigation = menuButton.navigation;
            navigation.mode = Navigation.Mode.None;
            menuButton.navigation = navigation;
        }
        HighlightButton(index);
    }

    private void HighlightButton(int index)
    {
        foreach (GameObject button in mainMenuButtons)
            button.GetComponent<Button>().OnDeselect(null);
        mainMenuButtons[index].GetComponent<Button>().OnSelect(null);
    }

    public void ChangeFocusToItems()
    {
        if (itemsPanel.activeInHierarchy)
        {
            menuState = MenuState.itemPanel;
            DisableMainButtons(0);
            if (itemSlotContainerParent.childCount > 0)
            {
                var item = itemSlotContainerParent.GetChild(0).gameObject;
                Utilities.SetSelectedAndHighlight(item, itemSlotContainerParent.GetComponentInChildren<Button>());
                item.GetComponent<ItemButton>().Press();
            }
        }
    }
    
    public void ChangeFocusToItemChoicePanel()
    {
        menuState = MenuState.itemCharacterChoicePanel;
        DisableItemNavigation();
        Utilities.SetSelectedAndHighlight(itemsCharacterChoiceNames[0].transform.parent.gameObject, itemsCharacterChoiceNames[0].GetComponentInParent<Button>());
        Button[] buttons = new Button[] { useButton, discardButton };
        foreach (Button button in buttons)
        {
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
        }
    }
    
    public void ChangeFocusToStats()
    {
        if (characterPanel.activeInHierarchy)
        {
            menuState = MenuState.statPanel;
            DisableMainButtons(1);
            var statButton = statsButtons[0];
            EventSystem.current.SetSelectedGameObject(statButton);
            statButton.GetComponent<Button>().onClick.Invoke();
        }
    }
    
    public void ChangeFocusToOptions()
    {
        if (optionsPanel.activeInHierarchy)
        {
            menuState = MenuState.optionsPanel;
            DisableMainButtons(2);
            EventSystem.current.SetSelectedGameObject(optionsPanel.transform.GetChild(1).gameObject);
        }
    }
    
    public void ChangeFocusToExit()
    {
        if (warningPanel.activeInHierarchy)
        {
            menuState = MenuState.exitPanel;
            DisableMainButtons(5);
            var exitPanel = warningPanel.transform.GetComponentInChildren<Button>();
            Utilities.SetSelectedAndHighlight(exitPanel.gameObject, exitPanel);
        }
    }

    public void NavigateItems(InputAction.CallbackContext context)
    {
        if (context.canceled && itemsPanel.activeInHierarchy)
        {
            if (itemSlotContainerParent.childCount != 0)
            {
                var button = EventSystem.current.currentSelectedGameObject.GetComponent<ItemButton>();
                if (button)
                {
                    button.Press();
                }
            }
        }
    }

    public void NavigateStats(InputAction.CallbackContext context)
    {
        if (menuState != MenuState.statPanel) { return; }
        if (context.canceled)
        {
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button)
            {
                button.onClick.Invoke();
            }
        }
    }

    public void NavigateToUse(InputAction.CallbackContext context)
    {
        if (menuState == MenuState.itemPanel && context.performed) {
            if (activeItem)
            {
                Invoke("SwitchToUsePanel",0.1f);
            }
        }
    }

    private void SwitchToUsePanel()
    {
        menuState = MenuState.itemUsePanel;
        activeItemGameObject = EventSystem.current.currentSelectedGameObject;
        DisableItemNavigation();
        EventSystem.current.SetSelectedGameObject(useButton.gameObject);
    }

    private void OpenItemCharaterChoice()
    {
        OpenCharacterChoicePanel();
        ChangeFocusToItemChoicePanel();
    }

    private void DisableItemNavigation()
    {
        foreach (Transform item in itemSlotContainerParent)
        {
            var itemButton = item.GetComponent<Button>();
            var navigation = itemButton.navigation;
            navigation.mode = Navigation.Mode.None;
            itemButton.navigation = navigation;
        }
        Button[] buttons = new Button[] { useButton, discardButton };
        foreach (Button button in buttons)
        {
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.Vertical;
            button.navigation = navigation;
        }
    }
    
    private void UnlockItemToNavigate()
    {
        foreach (Transform item in itemSlotContainerParent)
        {
            var itemButton = item.GetComponent<Button>();
            var navigation = itemButton.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            itemButton.navigation = navigation;
        }
        EventSystem.current.SetSelectedGameObject(activeItemGameObject);
        Button[] buttons = new Button[] { useButton, discardButton };
        foreach (Button button in buttons)
        {
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
        }
    }

    public void ReturnToPrevious()
    {
        if (!GameManager.instance.shopMenuOpened && !GameManager.instance.battleIsActive && UI.enabled)
        {
            switch (menuState)
            {
                case MenuState.itemPanel:
                    menuState = MenuState.mainPanels;
                    ToggleItems();
                    EnableMainButtons(0);
                    break;
                case MenuState.itemUsePanel:
                    menuState = MenuState.itemPanel;
                    UnlockItemToNavigate();
                    break;
                case MenuState.itemCharacterChoicePanel:
                    menuState = MenuState.itemUsePanel;
                    EventSystem.current.SetSelectedGameObject(useButton.gameObject);
                    DisableItemNavigation();
                    break;
                case MenuState.statPanel:
                    menuState = MenuState.mainPanels;
                    ToggleStats();
                    EnableMainButtons(1);
                    break;
                case MenuState.optionsPanel:
                    menuState = MenuState.mainPanels;
                    OptionsMenu();
                    EnableMainButtons(2);
                    break;
                case MenuState.exitPanel:
                    menuState = MenuState.mainPanels;
                    EnableMainButtons(5);
                    break;
                case MenuState.mainPanels:
                    ToggleMenu();
                    break;
            }
        }
    }

    public void ReturnToPrevious(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            ReturnToPrevious();
        }
    }
}

