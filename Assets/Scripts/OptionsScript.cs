using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class OptionsScript : MonoBehaviour
{

    [SerializeField] Slider sliderSFX, sliderMusic;
    [SerializeField] Slider sliderDifficulty;
    [SerializeField] TextMeshProUGUI difficultyText;
    private int difficulty;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Music_Volume_") && sliderMusic != null || sliderSFX != null)
        {
            sliderMusic.value = PlayerPrefs.GetFloat("Music_Volume_");
            sliderSFX.value = PlayerPrefs.GetFloat("SFX_Volume_");
            UpdateVolume();
            UpdateSFX();
        }
        if (sliderDifficulty != null)
        {
            sliderDifficulty.value = PlayerPrefs.GetInt("Difficulty_");
            UpdateDifficulty();
        }
    }

    public void SaveButton()
    {
        if (sliderMusic != null && sliderSFX != null)
        {
            PlayerPrefs.SetFloat("Music_Volume_", sliderMusic.value);
            PlayerPrefs.SetFloat("SFX_Volume_", sliderSFX.value);
        }
        PlayerPrefs.SetInt("Difficulty_", difficulty);
    }

    public void ReturnToMainMenu()
    {
        float musicValue = PlayerPrefs.GetFloat("Music_Volume_", sliderMusic.value);
        float sfxValue = PlayerPrefs.GetFloat("SFX_Volume_", sliderSFX.value);
        int diffValue = PlayerPrefs.GetInt("Difficulty_", difficulty);
        if (musicValue != sliderMusic.value || sfxValue != sliderSFX.value || diffValue != difficulty)
        {
            Debug.LogWarning("Do you want to keep changes?");
            //warn for save
        }
        SceneManager.LoadScene("Main Menu");

    }

    public void UpdateVolume()
    {
        AudioManager.instance.SetVolumeMusic(sliderMusic.value);
    }

    public void UpdateSFX()
    {
        AudioManager.instance.SetVolumeSFX(sliderSFX.value);
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
    }
}
