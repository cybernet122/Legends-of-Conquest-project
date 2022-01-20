using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] string newGameScene;
    [SerializeField] GameObject continueButton;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Player_Pos_X"))
        {
            continueButton.GetComponent<Button>().enabled = false;
            continueButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
        }
        if (PlayerPrefs.HasKey("Music_Volume_"))
            AudioManager.instance.SetVolumeMusic(PlayerPrefs.GetFloat("Music_Volume_"));
        if (!AudioManager.instance.IsPlaying() || AudioManager.instance.GetMusicIndex() != 4)
            AudioManager.instance.PlayBackgroundMusic(4);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(newGameScene);
        int difficulty = 1;
        float musicValue = 0.24f;
        float sfxValue = 0.24f;
        if (PlayerPrefs.HasKey("Difficulty_"))
        {
            difficulty = PlayerPrefs.GetInt("Difficulty_");
        }
        if (PlayerPrefs.HasKey("Music_Volume_"))
        {
            musicValue = PlayerPrefs.GetFloat("Music_Volume_");
            sfxValue = PlayerPrefs.GetFloat("SFX_Volume_");
        }

        PlayerPrefs.DeleteAll();
        Debug.Log("Purging Data");
        PlayerPrefs.SetInt("Difficulty_", difficulty);
        PlayerPrefs.SetFloat("Music_Volume_", musicValue);
        PlayerPrefs.SetFloat("SFX_Volume_", sfxValue);
        AudioManager.instance.SetVolumeMusic(musicValue);
        AudioManager.instance.SetVolumeSFX(sfxValue);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        if (continueButton.GetComponent<Button>().isActiveAndEnabled)
        {
            SceneManager.LoadScene("Loading Scene");
        }
    }

    public void OptionsMenu()
    {
        SceneManager.LoadScene("Options");
    }
}
