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
    private Animator fader;
    public GameObject menu;
    [SerializeField] GameObject[] statsButtons;
    [SerializeField] TextMeshProUGUI[] nameInfoText, hpInfoText, manaInfoText, xpInfoText, playerInfoLevel, currentXPText;
    [SerializeField] Slider[] hpSlider, magicSlider, xpInfoSlider;
    [SerializeField] Image[] characterInfoImage;
    [SerializeField] GameObject[] characterInfoPanel;
    [SerializeField] TextMeshProUGUI nameText, hpText, manaText, statDex, statDef, xpText, playerLevel, statEquippedWeapon, statEquippedArmor, statWeaponPower, statArmorDefense, speed, evasion, currentQuest, goldCoins;
    [SerializeField] Slider xpSlider;
    [SerializeField] Image characterImage;
    [SerializeField] GameObject characterPanel, itemsPanel, itemContainer, charInfoList;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] Button useButton, discardButton, returnButton;
    [SerializeField] GameObject characterChoicePanel, warningPanel;
    [SerializeField] TextMeshProUGUI[] itemsCharacterChoiceNames;
    [SerializeField] VerticalLayoutGroup characterButtons;
    [SerializeField] HorizontalLayoutGroup abilities;
    [SerializeField] Image[] abilitiesIcons;
    [SerializeField] QuestUpdate questUpdate;
    [SerializeField] GameObject optionsPanel, generalSettingsButton, exitToMainMenuButton;
    [SerializeField] AbilityInfoManager abilityInfoManager;
    [SerializeField] GameObject[] mainMenuButtons;
    [SerializeField] SwitchInputModule eventSystem;
    [SerializeField] TextMeshProUGUI infoText;
    private BattleCharacters[] players;
    public static MenuManager instance;
    public TextMeshProUGUI itemName, itemDescription;
    public ItemsManager activeItem;
    private GameObject activeItemGameObject;
    PlayerStats[] playerStats;
    bool toglMenu, toglItems, toglStats, toglWarning, toglOptions = false;
    int currentlyViewing;
    ItemsManager[] itemsCollection;
    public MenuState menuState = MenuState.mainPanels;
    public static event UnityAction CloseMenu;
    public static event UnityAction<bool> InspectingStats;
    public TreasureChest treasureChest;
    private bool closedMenu = true;
    private MainMenuManager mainMenuManager;
    OptionsScript optionsScript;
    public bool GetInfoPanelActive() { return charInfoList.activeInHierarchy; }
    public bool GetStatPanelActive() { return characterPanel.activeInHierarchy; }
    public void GetMainMenuReference() { mainMenuManager = FindObjectOfType<MainMenuManager>(); }


    private void Start()
    {    
        instance = this;   
        menu.SetActive(false);
        characterPanel.SetActive(false);
        CloseCharacterChoicePanel();
        itemDescription.text = "";
        itemName.text = "";
        optionsScript = optionsPanel.GetComponent<OptionsScript>();
        optionsScript.ClosePanel();
        itemsCollection = FindObjectsOfType<ItemsManager>();
        fader = image.GetComponent<Animator>();
        HideInfoText();
        infoText.transform.SetSiblingIndex(1);
    }

    public void ResetToggles()
    {
        toglMenu = false;
        toglItems = false;
        toglOptions = false;
        toglStats = false;
        toglWarning = false;
        closedMenu = true;
    }

    public void FadeImage()
    {
        GameManager.instance.loading = true;
        fader.SetTrigger("StartFade");
    }

    public void FadeOut(float delay)
    {
        StartCoroutine(FadeCoroutine(delay));
    }

    IEnumerator FadeCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        fader.SetTrigger("EndFade");
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.loading = false;
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (!ShopManager.instance.shopMenu.activeInHierarchy && ShopManager.instance.finishedCount && menuState == MenuState.mainPanels && !BattleRewardsHandler.instance.RewardScreenOpen() &&
                !GameManager.instance.dialogBoxOpened && SceneManager.GetActiveScene().name != "Main Menu" && SceneManager.GetActiveScene().name != "Options" && SceneManager.GetActiveScene().name != "Loading Screen")
        {
            if (!GameManager.instance.loading && closedMenu)
            {
                foreach (GameObject button in mainMenuButtons)
                    button.GetComponent<Button>().OnDeselect(null);
                ToggleMenu();
                closedMenu = false;
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
        optionsScript.ClosePanel();
        toglItems = false;
        toglStats = false;
        toglOptions = false;
        GameManager.instance.UpdatePlayerStats();
        currentQuest.text = "Current Quest: " + QuestManager.instance.GetCurrentQuest();
        DialogController.instance.StartDelay();
        if (!GameManager.instance.battleIsActive && toglMenu)
        {
            SetFirstSelectedObject(0);
            mainMenuButtons[0].GetComponent<Button>().interactable = true;
            mainMenuButtons[3].GetComponent<Button>().interactable = true;
            var button = mainMenuButtons[0].GetComponent<Button>();
            button.OnSelect(null);
            EnableMainButtons(0);
        }
        else if(GameManager.instance.battleIsActive && toglMenu)
        {
            SetFirstSelectedObject(1);
            var button = mainMenuButtons[1].GetComponent<Button>();
            button.OnSelect(null);
            EnableMainButtons(1);
            mainMenuButtons[0].GetComponent<Button>().interactable = false;
            mainMenuButtons[3].GetComponent<Button>().interactable = false;
        }
        goldCoins.text = GameManager.instance.currentGoldCoins.ToString();
        menuState = MenuState.mainPanels;
        Invoke("CheckForShop", 0.1f);
    }

    public void SetFirstSelectedObject(int num)
    {
        if (SceneManager.GetActiveScene().name != "Main Menu" && SceneManager.GetActiveScene().name != "Loading Scene" && SceneManager.GetActiveScene().name != "GameOverScene")
        {
            if (EventSystem.current.firstSelectedGameObject != mainMenuButtons[num])
                EventSystem.current.firstSelectedGameObject = mainMenuButtons[num];
        }
    }

    public void Submit()
    {
        if (ShopManager.instance.canOpenShop)
            LeanTween.delayedCall(0.1f, () =>
            {
                if (!menu.activeInHierarchy)
                {
                    SendMessageUpwards("SwitchToShopUI");
                    if (ShopManager.instance.finishedCount)
                        ShopManager.instance.OpenShopMenu();
                }
            });
        else if (menu.activeInHierarchy)
            MenuNavigation();
        else if (DialogController.instance.npcInRange)
            DialogController.instance.SkipDialogue();
        else if (treasureChest != null)
            treasureChest.OpenChest();
        else if (SceneManager.GetActiveScene().name == "Options")
            if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>())
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if(context.canceled)
            Submit();
    }

    private void CheckForShop()
    {
        /*if (!menu.activeInHierarchy && SwitchActiveMap.instance.GetInputAction().name == "UI")
            CloseMenu?.Invoke();*/
    }

    public void ToggleItems()
    {        
        toglItems = !toglItems;
        UpdateStats();
        if (characterChoicePanel.activeInHierarchy)
            characterChoicePanel.SetActive(false);
        characterPanel.SetActive(false);
        charInfoList.SetActive(!toglItems);
        toglStats = false;
        toglOptions = false;
        itemsPanel.SetActive(toglItems);
        if(toglItems)
        {
            menuState = MenuState.itemPanel;
            if (itemSlotContainerParent.childCount != 0)
                Utilities.SetSelectedAndHighlight(itemSlotContainerParent.GetChild(0).gameObject, itemSlotContainerParent.GetComponentInChildren<Button>());            
        }
        else
        {
            menuState = MenuState.mainPanels;
            itemDescription.text = string.Empty;
            itemName.text = string.Empty;
            if(mainMenuButtons[0].GetComponent<Button>().navigation.mode != Navigation.Mode.Automatic)
                EnableMainButtons(0);
        }
    }
    public void ToggleStats()
    {
        CloseOptions();
        toglStats = !toglStats;
        if (characterChoicePanel.activeInHierarchy)
            characterChoicePanel.SetActive(false);
        itemsPanel.SetActive(false);
        optionsScript.ClosePanel();
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
        players = BattleManager.instance.GetPlayers();
        HealthBarsUIManager.instance.UpdateHealthBars();
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
            if (!GameManager.instance.battleIsActive)
            {
                hpInfoText[i].text = playerStats[i].currentHP + "/" + playerStats[i].maxHP;
                hpSlider[i].value = Mathf.InverseLerp(0, playerStats[i].maxHP, playerStats[i].currentHP);
                manaInfoText[i].text = playerStats[i].currentMana + "/" + playerStats[i].maxMana;
                magicSlider[i].value = Mathf.InverseLerp(0, playerStats[i].maxMana, playerStats[i].currentMana);
            }
            else if(GameManager.instance.battleIsActive && players.Length > 0)
            {
                hpInfoText[i].text = players[i].currentHP + "/" + players[i].maxHP;
                hpSlider[i].value = Mathf.InverseLerp(0, players[i].maxHP, players[i].currentHP);
                manaInfoText[i].text = players[i].currentMana + "/" + players[i].maxMana;
                magicSlider[i].value = Mathf.InverseLerp(0, players[i].maxMana, players[i].currentMana);
            }
            playerInfoLevel[i].text = playerStats[i].playerLevel.ToString();
            xpInfoSlider[i].minValue = 0;
            xpInfoSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpInfoSlider[i].value = playerStats[i].currentXP;
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
        TooltipSystem.Hide();
        currentlyViewing = playerSelectedNumber;
        PlayerStats playerSelected = playerStats[playerSelectedNumber];
        nameText.text = playerSelected.playerName;
        if (!GameManager.instance.battleIsActive)
        {
            hpText.text = "Health: " + playerSelected.currentHP.ToString() + " / " + playerSelected.maxHP;
            manaText.text = "Magic: " + playerSelected.currentMana.ToString() + " / " + playerSelected.maxMana;
            statDex.text = "Dexterity: " + playerSelected.dexterity.ToString();
            statDef.text = "Defense: " + playerSelected.defense.ToString();
        }
        else if (GameManager.instance.battleIsActive && players.Length > 0)
        {
            hpText.text = "Health: " + players[currentlyViewing].currentHP.ToString() + " / " + players[currentlyViewing].maxHP;
            manaText.text = "Magic: " + players[currentlyViewing].currentMana.ToString() + " / " + players[currentlyViewing].maxMana;
            statDex.text = "Dexterity: " + players[currentlyViewing].dexterity.ToString();
            statDef.text = "Defense: " + players[currentlyViewing].defense.ToString();
        }
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
        menu.SetActive(false);
        GameManager.instance.gameMenuOpened = false;
        menuState = MenuState.mainPanels;
        ResetToggles();
        GameManager.instance.EmptyPlayerStats();        
        warningPanel.SetActive(false);
        HideInfoText();
        float delay = 0.2f;
        if (GameManager.instance.battleIsActive)
        {
            delay = 1f;
            BattleManager.instance.OnExitToMainMenu(delay);
            LeanTween.delayedCall(delay, () =>
            FadeOut(1));
        }
        LeanTween.delayedCall(delay, () =>        
            SceneManager.LoadScene("Main Menu"));
    }

    public void DiscardItem()
    {
        if (activeItem)
        {
            Inventory.instance.RemoveItem(activeItem);
            AudioManager.instance.PlaySFX(4);
            LeanTween.delayedCall(0.1f, () =>
            {
                if (itemSlotContainerParent.childCount == 0)
                {
                    activeItem = null;
                    itemDescription.text = string.Empty;
                    itemName.text = string.Empty;
                }
            });
        }
    }

    public void UseItem(int characterToUse)
    {
        PlayerStats player = GameManager.instance.GetPlayerStats()[characterToUse];
        if (activeItem.itemName == player.equippedWeaponName || activeItem.itemName == player.equippedArmorName) { return; }
        activeItem.UseItem(characterToUse);
        Inventory.instance.RemoveItem(activeItem);
        AudioManager.instance.PlaySFX(0);
        itemDescription.text = string.Empty;
        itemName.text = string.Empty;
    }

    private void OpenCharacterChoicePanel()
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
        DisableUseAndDiscard();
    }
    public void CloseCharacterChoicePanel()
    {
        returnButton.gameObject.SetActive(false);
        characterChoicePanel.SetActive(false);
    }

    public void FindItem(ItemsManager itemToFind)
    {        
        for (int i = 0; i < itemsCollection.Length; i++)
        {
            if(itemsCollection[i].itemType == itemToFind.itemType && itemsCollection[i].itemName == itemToFind.itemName)
            {
                activeItem = itemsCollection[i];
                return;
            }
        }
        var itemList = Inventory.instance.GetItemList();
        foreach (ItemsManager itemInInventory in itemList)
        {
            if (itemInInventory.itemName == itemToFind.itemName)
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
                activeItem = im;
            }
        }
    }

    public void OptionsMenu()
    {
        if (!optionsScript.changedSettings)
        {
            toglOptions = !toglOptions;
            characterPanel.SetActive(false);
            charInfoList.SetActive(!toglOptions);
            if (characterChoicePanel.activeInHierarchy)
                characterChoicePanel.SetActive(false);
            itemsPanel.SetActive(false);
            toglStats = false;
            toglItems = false;
            optionsPanel.transform.GetChild(0).gameObject.SetActive(toglOptions);
            if (!toglOptions)
            {
                menuState = MenuState.mainPanels;
                if (mainMenuButtons[2].GetComponent<Button>().navigation.mode != Navigation.Mode.Automatic)
                    EnableMainButtons(2);
                optionsScript.ReturnButton();
            }
            else
                optionsScript.SetValuesOnStart();
        }
        else
            optionsScript.OpenWarningPanel(false);
    }

    private void CloseOptions()
    {
        if (optionsScript.changedSettings)
        {
            optionsScript.OpenWarningPanel(false);
            return;
        }
        optionsScript.ClosePanel();
    }

    public void SaveButton()
    {
        GameManager.instance.SaveData();
        if (optionsScript.changedSettings)
            optionsScript.SaveButton();
    }

    public void QuitGame()
    {
        Application.Quit();
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

    public void ShowInfoText(string text)
    {
        infoText.text = text;
        infoText.gameObject.SetActive(true);
    }
    
    public void ShowInfoText(string text, float delay)
    {
        infoText.text = text;
        infoText.gameObject.SetActive(true);
        LeanTween.delayedCall(delay, () => { HideInfoText(); });
    }

    public void HideInfoText()
    {
        infoText.text = string.Empty;
        infoText.gameObject.SetActive(false);
    }

    public void UpdateQuest(string quest)
    {
        questUpdate.PlayUpdateAnimation(quest);
        //GameManager.instance.UpdatePlayerLevels();
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

    public void NavigateMenu(InputAction.CallbackContext context)
    {
        if (!EventSystem.current.currentSelectedGameObject && context.canceled && menu.activeInHierarchy)
        {
            switch (menuState)
            {
                case MenuState.mainPanels:
                    EventSystem.current.SetSelectedGameObject(mainMenuButtons[0]);
                    break;
                case MenuState.itemPanel:
                    if(itemSlotContainerParent.childCount != 0)
                    EventSystem.current.SetSelectedGameObject(itemSlotContainerParent.GetChild(0).gameObject);
                    break;
                case MenuState.itemUsePanel:
                    EventSystem.current.SetSelectedGameObject(useButton.gameObject);
                    break;
                case MenuState.optionsPanel:
                    EventSystem.current.SetSelectedGameObject(generalSettingsButton);
                    break;
                case MenuState.statPanel:
                    print(statsButtons[0].name);
                    EventSystem.current.SetSelectedGameObject(statsButtons[0]);
                    break;
                case MenuState.itemCharacterChoicePanel:
                    EventSystem.current.SetSelectedGameObject(characterChoicePanel.transform.GetChild(0).gameObject);
                    break;
                case MenuState.exitPanel:
                    EventSystem.current.SetSelectedGameObject(exitToMainMenuButton);
                    break;
            }
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
            if (menuState == MenuState.statPanel && EventSystem.current.currentSelectedGameObject)
            {
                var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                TooltipSystem.Hide();
                if (button)
                {
                    button.onClick.Invoke();
                }
            }
        }
    }

    public void MenuNavigation(/*InputAction.CallbackContext context*/)
    {
        if (menuState == MenuState.itemPanel/* && context.performed*/) {
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

    public void OpenItemCharaterChoice()
    {
        returnButton.gameObject.SetActive(true);
        OpenCharacterChoicePanel();
        ChangeFocusToItemChoicePanel();
    }

    private void DisableUseAndDiscard()
    {
        var useNavigation = useButton.navigation;
        useNavigation.mode = Navigation.Mode.None;
        useButton.navigation = useNavigation;
        var discardNavigation = discardButton.navigation;
        discardNavigation.mode = Navigation.Mode.None;
        discardButton.navigation = discardNavigation;
    }

    private void EnableUseAndDiscard()
    {
        var useNavigation = useButton.navigation;
        useNavigation.mode = Navigation.Mode.Automatic;
        useButton.navigation = useNavigation;
        var discardNavigation = discardButton.navigation;
        discardNavigation.mode = Navigation.Mode.Automatic;
        discardButton.navigation = discardNavigation;
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

    private void EnableStatButtons()
    {
        for (int i = 0; i < statsButtons.Length; i++)
        {
            if (EventSystem.current.currentSelectedGameObject == statsButtons[i])            
                InspectingStats?.Invoke(false);            
            statsButtons[i].GetComponent<Button>().interactable = true;
        }
    }

    public void ReturnToPrevious()
    {
        if (!GameManager.instance.shopMenuOpened && menu.activeInHierarchy && (SwitchActiveMap.instance.GetActiveInputAction().name == "UI" || SwitchActiveMap.instance.GetActiveInputAction().name == "BattleUI"))
        {
            TooltipSystem.Hide();
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
                    CloseCharacterChoicePanel();
                    EventSystem.current.SetSelectedGameObject(useButton.gameObject);
                    DisableItemNavigation();
                    break;
                case MenuState.statPanel:
                    menuState = MenuState.mainPanels;
                    ToggleStats();
                    EnableMainButtons(1);
                    break;               
                case MenuState.optionsPanel:
                    if (!optionsScript.changedSettings)
                    {
                        menuState = MenuState.mainPanels;
                        EnableMainButtons(2);
                    }
                        OptionsMenu();
                    break;
                case MenuState.exitPanel:
                    menuState = MenuState.mainPanels;
                    ToggleExitWarning();
                    EnableMainButtons(5);
                    break;
                case MenuState.mainPanels:
                    ToggleMenu();
                    if (GameManager.instance.battleIsActive)
                        BattleManager.instance.OnCloseMenu();
                    LeanTween.delayedCall(0.15f, () => closedMenu = true);
                    break;
            }
        }
        else if(Utilities.ReturnSceneName() == "Main Menu")            
            mainMenuManager.Cancel();
    }

    public void ReturnToPrevious(InputAction.CallbackContext context)
    {
        if (context.canceled)
            ReturnToPrevious();
    }
}

