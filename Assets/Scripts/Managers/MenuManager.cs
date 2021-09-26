using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] TextMeshProUGUI nameText, hpText, manaText, statDex, statDef, xpText, playerLevel, statEquippedWeapon, statEquippedArmor, statWeaponPower, statArmorDefence;
    [SerializeField] Slider xpSlider;
    [SerializeField] Image characterImage;
    [SerializeField] GameObject characterPanel, itemsPanel, itemContainer;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] Button useButton,discardButton;
    [SerializeField] GameObject characterChoicePanel;
    [SerializeField] TextMeshProUGUI[] itemsCharacterChoiceNames;
    public static MenuManager instance;
    public TextMeshProUGUI itemName,itemDescription;
    public ItemsManager activeItem;
    PlayerStats[] playerStats;
    bool toglMenu,toglItems,toglStats = false;
    int currentlyViewing;
    private void Start()
    {
        instance = this;
        menu.SetActive(false);
        characterPanel.SetActive(false);
        CloseCharacterChoicePanel();
        itemDescription.text = "";
        itemName.text = "";
    }

    public void FadeImage()
    {
        image.GetComponent<Animator>().SetTrigger("StartFade");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && !ShopManager.instance.shopMenu.activeInHierarchy && !GameManager.instance.dialogBoxOpened) //Toggle Menu
        {
            ToggleMenu();
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
        toglItems = false;
        toglStats = false;
        DialogController.instance.count = 0;
    }

    public void ToggleItems()
    {
        toglItems = !toglItems;
        UpdateStats();
        characterPanel.SetActive(false);
        toglStats = false;
        itemsPanel.SetActive(toglItems);
        GameManager.instance.gameMenuOpened = toglItems;
    }
    public void ToggleStats()
    {
        toglStats = !toglStats;
        UpdateStats();
        itemsPanel.SetActive(false);
        toglItems = false;
        characterPanel.SetActive(toglStats);
        GameManager.instance.gameMenuOpened = toglStats;
    }

    public void UpdateStats()
    {
        playerStats = GameManager.instance.GetPlayerStats();
        for(int i = 0;i< playerStats.Length; i++)
        {
            if(characterInfoPanel.Length <= i) { break; }
            //print(i);
            characterInfoPanel[i].SetActive(true);
            nameInfoText[i].text = playerStats[i].playerName;
            characterInfoImage[i].sprite = playerStats[i].characterImage;
            hpInfoText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
            manaInfoText[i].text = "Mana: " + playerStats[i].currentMana + "/" + playerStats[i].maxMana;
            //currentXPText[i].text = "Current XP: " + playerStats[i].currentXP;
            playerInfoLevel[i].text = playerStats[i].playerLevel.ToString();
            xpInfoSlider[i].minValue = 0;
            xpInfoSlider[i].value = playerStats[i].currentXP;
            if(playerStats[i].playerLevel >= playerStats[i].maxLevel) 
            {
                xpInfoSlider[i].minValue = 0;
                xpInfoSlider[i].value = 1;
                xpInfoSlider[i].maxValue = 1;
                xpInfoText[i].text = "0 / 0";
                currentXPText[i].text = "";
                return; 
            }
            xpInfoSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpInfoText[i].text = playerStats[i].currentXP.ToString() + " / " + playerStats[i].xpForNextLevel[playerStats[i].playerLevel].ToString();
        }
        StatsMenu();
        StatsMenuUpdate(currentlyViewing);
    }

    public void StatsMenu()
    {
        for(int i=0; i < playerStats.Length; i++)
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
        hpText.text = "HP: " + playerSelected.currentHP.ToString() + " / " + playerSelected.maxHP;
        manaText.text = "Mana: " + playerSelected.currentMana.ToString() + " / " + playerSelected.maxMana;
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
        statEquippedWeapon.text = playerSelected.equippedWeaponName;
        statEquippedArmor.text = playerSelected.equippedArmorName;
        statWeaponPower.text = "Weapon Power: " + playerSelected.weaponPower.ToString();
        statArmorDefence.text = "Armor Defence: " + playerSelected.armorDefence.ToString();
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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DiscardItem()
    {
        Inventory.instance.RemoveItem(activeItem);
        AudioManager.instance.PlaySFX(4);
    }

    public void UseItem(int characterToUse)
    {
        activeItem.UseItem(characterToUse);
        OpenCharacterChoicePanel();
        Inventory.instance.RemoveItem(activeItem);
        AudioManager.instance.PlaySFX(0);
    }

    public void OpenCharacterChoicePanel()
    {
        characterChoicePanel.SetActive(true);
        if (activeItem)
        {
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
}
