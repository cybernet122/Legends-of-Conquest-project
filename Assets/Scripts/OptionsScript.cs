using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class OptionsScript : MonoBehaviour
{
    [SerializeField] GameObject discardWarningPanel, optionsPanel;
    [SerializeField] Slider sliderSFX, sliderMusic;
    [SerializeField] Slider sliderDifficulty;
    [SerializeField] TextMeshProUGUI difficultyText;
    [SerializeField] Button[] saveAndReturnButtons;
    float musicValue, sfxValue, diffValue;
    private int difficulty;
    public bool changedSettings;
    // Start is called before the first frame update
    void Start()
    {
        if (discardWarningPanel != null && discardWarningPanel.activeInHierarchy)
            discardWarningPanel.SetActive(false);
        SetValuesOnStart();
    }

    private void OnEnable()
    {
        SaveValues();
        changedSettings = false;
    }

    public void ClosePanel()
    {
        optionsPanel.SetActive(false);
    }

    private void SaveValues()
    {
        if (sliderMusic)
            musicValue = sliderMusic.value;
        if(sliderSFX)
            sfxValue = sliderSFX.value;
        if (sliderDifficulty)
            diffValue = sliderDifficulty.value;
    }

    public void SetValuesOnStart()
    {
        if (sliderMusic && PlayerPrefs.HasKey("Music_Volume_"))
            sliderMusic.value = PlayerPrefs.GetFloat("Music_Volume_");
        if (sliderSFX && PlayerPrefs.HasKey("SFX_Volume_"))
            sliderSFX.value = PlayerPrefs.GetFloat("SFX_Volume_");
        if (sliderDifficulty && PlayerPrefs.HasKey("Difficulty_"))
        {
            sliderDifficulty.value = PlayerPrefs.GetInt("Difficulty_");
            UpdateDifficulty();
        }
        changedSettings = false;
    }

    public void SaveButton()
    {
        if (sliderMusic != null)
            PlayerPrefs.SetFloat("Music_Volume_", sliderMusic.value);
        if(sliderSFX != null)
            PlayerPrefs.SetFloat("SFX_Volume_", sliderSFX.value);
        if(sliderDifficulty != null)
            PlayerPrefs.SetInt("Difficulty_", (int)sliderDifficulty.value);
        SaveValues();
        changedSettings = false;
    }

    public void ReturnButton()
    {
        changedSettings = false;
        discardWarningPanel.SetActive(false);
        if(PlayerPrefs.HasKey("Music_Volume_"))
            sliderMusic.value = PlayerPrefs.GetFloat("Music_Volume_");
        if(PlayerPrefs.HasKey("SFX_Volume_"))
            sliderSFX.value = PlayerPrefs.GetFloat("SFX_Volume_");
        if(PlayerPrefs.HasKey("Difficulty_"))
            sliderDifficulty.value = PlayerPrefs.GetInt("Difficulty_");
    }

    public void OpenWarningPanel(bool returnToMainMenu)
    {
        if (changedSettings)
        {
            discardWarningPanel.SetActive(true);
            foreach(Button button in saveAndReturnButtons)
            {
                Navigation buttonNav = button.navigation;
                buttonNav.mode = Navigation.Mode.None;
                button.navigation = buttonNav;
            }
            var warningButton = discardWarningPanel.GetComponentInChildren<Button>();
            Utilities.SetSelectedAndHighlight(warningButton.gameObject, warningButton);
        }
        else
            ReturnToPreviousMenu(returnToMainMenu);
    }

    public void CloseWarningPanel()
    {
        discardWarningPanel.SetActive(false);
        foreach (Button button in saveAndReturnButtons)
        {
            Navigation buttonNav = button.navigation;
            buttonNav.mode = Navigation.Mode.Horizontal;
            button.navigation = buttonNav;
        }
        Utilities.SetSelectedAndHighlight(saveAndReturnButtons[1].gameObject, saveAndReturnButtons[1]);
    }

    public void ReturnToMenu()
    {
        MenuManager.instance.ReturnToPrevious();
    }

    public void ReturnToPreviousMenu(bool returnToMainMenu)
    {
        changedSettings = false;
        if (returnToMainMenu)
            SceneManager.LoadScene("Main Menu");
        else
            ReturnToMenu();
    }

    public void UpdateVolume()
    {
        changedSettings = true;
        LeanTween.delayedCall(0.13f, () => { AudioManager.instance.SetVolumeMusic(sliderMusic.value); });
    }

    public void UpdateSFX()
    {
        changedSettings = true;
        LeanTween.delayedCall(0.13f, () => { AudioManager.instance.SetVolumeSFX(sliderSFX.value); });
    }

    public void UpdateDifficulty()
    {
        difficulty = (int)sliderDifficulty.value;
        switch (difficulty)
        {
            case 1:
                difficultyText.text = "Easy";
                break;
            case 2:
                difficultyText.text = "Normal";
                break;
            case 3:
                difficultyText.text = "Hard";
                break;
        }
        changedSettings = true;
    }
}
