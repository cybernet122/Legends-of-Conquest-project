using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarsUIManager : MonoBehaviour
{
    [SerializeField] GameObject[] healthBarsObjects;
    [SerializeField] Image[] playerIcons;
    [SerializeField] Slider[] healthSliders, magicSliders;
    [SerializeField] TextMeshProUGUI[] healthText, magicText;
    PlayerStats[] playerStats;
    public static HealthBarsUIManager instance;

    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void HandleHealthBars(bool activate)
    {
        if (!activate)
            transform.GetChild(0).gameObject.SetActive(false);
        else
            Invoke("ShowHealthBars",0.07f);
    }

    public void UpdateHealthBars() { ShowHealthBars(); }
    private void ShowHealthBars()
    {
        var sceneName = Utilities.ReturnSceneName();
        if (sceneName == "Main Menu" || sceneName == "Loading Scene" || sceneName == "GameOverScene" || sceneName == "Treasure" || sceneName == "Options")
        {
            transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        playerStats = GameManager.instance.GetPlayerStats();
        transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < playerStats.Length; i++)
        {
            healthBarsObjects[i].SetActive(true);
            playerIcons[i].sprite = playerStats[i].characterIcon;
            healthSliders[i].minValue = 0;
            healthSliders[i].maxValue = playerStats[i].maxHP;
            healthSliders[i].value = playerStats[i].currentHP;
            healthText[i].text = playerStats[i].currentHP + " / " + playerStats[i].maxHP;
            magicSliders[i].minValue = 0;
            magicSliders[i].maxValue = playerStats[i].maxMana;
            magicSliders[i].value = playerStats[i].currentMana;
            magicText[i].text = playerStats[i].currentMana + " / " + playerStats[i].maxMana;
        }
        for (int i = 3; i >= playerStats.Length; i--)
        {
            healthBarsObjects[i].SetActive(false);
        }
    }

}
