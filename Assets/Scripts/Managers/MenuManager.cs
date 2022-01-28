using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MenuManager : MonoBehaviour
{
    [SerializeField] Image image;
    public GameObject menu;
    [SerializeField] GameObject[] statsButtons;
    [SerializeField] TextMeshProUGUI[] nameInfoText, hpInfoText, manaInfoText, xpInfoText, playerInfoLevel, currentXPText;
    [SerializeField] Slider[] xpInfoSlider;
    [SerializeField] Image[] characterInfoImage;
    [SerializeField] GameObject[] characterInfoPanel;
    [SerializeField] TextMeshProUGUI nameText, hpText, manaText, statDex, statDef, xpText, playerLevel, statEquippedWeapon, statEquippedArmor, statWeaponPower, statArmorDefence, speed, evasion, currentQuest;
    [SerializeField] Slider xpSlider;
    [SerializeField] Image characterImage;
    [SerializeField] GameObject characterPanel, itemsPanel, itemContainer, charInfoList;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] Button useButton,discardButton;
    [SerializeField] GameObject characterChoicePanel, warningPanel;
    [SerializeField] TextMeshProUGUI[] itemsCharacterChoiceNames;
    [SerializeField] VerticalLayoutGroup characterButtons;
    [SerializeField] HorizontalLayoutGroup abilities;
    [SerializeField] Image[] abilitiesIcons; 
    [SerializeField] QuestUpdate questUpdate;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] AbilityInfoManager abilityInfoManager;

    public static MenuManager instance;
    public TextMeshProUGUI itemName,itemDescription;
    public ItemsManager activeItem;
    PlayerStats[] playerStats;
    bool toglMenu, toglItems, toglStats, toglWarning, toglOptions = false;
    int currentlyViewing;
    ItemsManager[] itemsCollection;
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
        else if (Input.GetButtonDown("Cancel") && !ShopManager.instance.shopMenu.activeInHierarchy && 
            !GameManager.instance.dialogBoxOpened && !GameManager.instance.battleIsActive) //Toggle Menu
        {
            ToggleMenu();            
        }        
        if (Input.GetKeyDown(KeyCode.I))
        {
            var itemList = Inventory.instance.GetItemList();
            string printInv = "Inventory has " + itemList.Count + " items, ";
            foreach(ItemsManager item in itemList)
            {
                printInv += item.itemName + " " + item.amount + ", ";
            }
            Debug.Log(printInv);
        }
    }

    public void ToggleMenu()
    {
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
        DialogController.instance.count = 0;
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
        GameManager.instance.gameMenuOpened = toglItems;
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
        characterPanel.SetActive(toglStats);
        GameManager.instance.gameMenuOpened = toglStats;
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
        statDef.text = "Defence: " + playerSelected.defence.ToString();
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
        statArmorDefence.text = "Armor Defence: " + playerSelected.armorDefence.ToString();
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
    }

    public void ExitToMainMenu()
    {
        warningPanel.SetActive(false);
        GameManager.instance.gameMenuOpened = false;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
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
        itemDescription.text = "";
        itemName.text = "";
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
                im.armorDefence = itemInInventory.armorDefence;
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
        /*if(PlayerPrefs.HasKey("Difficulty_"))*/
    }

    public void SaveButton()
    {
        GameManager.instance.SaveData();
    }

    public void FadeOut()
    {
        image.GetComponent<Animator>().SetTrigger("EndFade");
    }

    public void ToggleExitWarning()
    {
        toglWarning = !toglWarning;
        warningPanel.SetActive(toglWarning);
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
}
