using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MenuManager : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject[] statsButtons;
    [SerializeField] TextMeshProUGUI[] nameInfoText, hpInfoText, manaInfoText, xpInfoText, playerInfoLevel;
    [SerializeField] Slider[] xpInfoSlider;
    [SerializeField] Image[] characterInfoImage;
    [SerializeField] GameObject[] characterInfoPanel;
    [SerializeField] TextMeshProUGUI[] nameText, hpText, manaText, currentXPText, xpText, playerLevel;
    [SerializeField] Slider[] xpSlider;
    [SerializeField] Image[] characterImage;
    [SerializeField] GameObject[] characterPanel;
    public static MenuManager instance;
    bool togl = false;
    PlayerStats[] playerStats;
    private void Start()
    {
        instance = this;
        menu.SetActive(false);
    }

    public void FadeImage()
    {
        image.GetComponent<Animator>().SetTrigger("StartFade");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //Toggle Menu
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        togl = !togl;
        UpdateStats();
        menu.SetActive(togl);
        GameManager.instance.gameMenuOpened = togl;
    }

    public void UpdateStats()
    {
        playerStats = GameManager.instance.GetPlayerStats();
        for(int i = 0;i< playerStats.Length; i++)
        {
            //print(i);
            characterInfoPanel[i].SetActive(true);
            nameText[i].text = playerStats[i].playerName;
            characterImage[i].sprite = playerStats[i].characterImage;
            hpText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
            manaText[i].text = "Mana: " + playerStats[i].currentMana + "/" + playerStats[i].maxMana;
            //currentXPText[i].text = "Current XP: " + playerStats[i].currentXP;
            playerLevel[i].text = "Player Level: " + playerStats[i].playerLevel.ToString();
            xpSlider[i].minValue = 0;
            xpSlider[i].value = playerStats[i].currentXP;
            if(playerStats[i].playerLevel >= playerStats[i].maxLevel) 
            {
                xpSlider[i].minValue = 0;
                xpSlider[i].value = 1;
                xpSlider[i].maxValue = 1;
                xpText[i].text = "0 / 0";
                currentXPText[i].text = "";
                return; 
            }
            xpSlider[i].maxValue = playerStats[i].xpForNextLevel[playerStats[i].playerLevel];
            xpText[i].text = playerStats[i].currentXP.ToString() + " / " + playerStats[i].xpForNextLevel[playerStats[i].playerLevel].ToString();
        }
    }

    public void StatsMenu()
    {
        for(int i=0; i < playerStats.Length; i++)
        {
            statsButtons[i].SetActive(true);
            statsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerStats[i].playerName;
        }
    }

    public void StatsMenuUpdate(int playerSelectedNumber)
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
