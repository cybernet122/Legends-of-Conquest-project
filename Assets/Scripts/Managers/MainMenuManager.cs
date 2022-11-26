using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] string newGameScene;
    [SerializeField] GameObject continueButton, newGameButton, optionsPanel, optionsButton, exitButton;
    MenuState menuState = MenuState.mainMenu;
    Button[] buttons;
    TMP_InputField inputField;
    [SerializeField] bool inputFieldSelected = false;
    public void SetSelected(bool value) { inputFieldSelected = value; }
    
    private void Start()
    {
        if (!PlayerPrefs.HasKey("Player_Pos_X"))
        {
            continueButton.GetComponent<Button>().enabled = false;
            continueButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            Utilities.SetSelectedAndHighlight(newGameButton, newGameButton.GetComponent<Button>());
        }
        if (PlayerPrefs.HasKey("Music_Volume_"))
        {
            Invoke("SetVolumeOnStart",0.1f);
        }
        //if (!AudioManager.instance.IsPlaying() || AudioManager.instance.GetMusicIndex() != 4)
        //AudioManager.instance.PlayBackgroundMusic(4);
        Invoke("PlayMainMenuMusic", 0.1f);
        optionsPanel.SetActive(false);
        buttons = new Button[]{ continueButton.GetComponent<Button>(), newGameButton.GetComponent<Button>(), optionsButton.GetComponent<Button>(), exitButton.GetComponent<Button>() };
        inputField = optionsPanel.GetComponentInChildren<TMP_InputField>();
    }

    private void SetVolumeOnStart()
    {
        float volume = PlayerPrefs.GetFloat("Music_Volume_");
        AudioManager.instance.SetVolumeMusic(volume);
    }

    private void PlayMainMenuMusic()
    {
        AudioManager.instance.PlayBackgroundMusic(4);
    }

    public void NewGame()
    {
        MenuManager.instance.FadeImage();
        LeanTween.delayedCall(0.9f, () =>
        {
            SceneManager.LoadScene(newGameScene);
            int difficulty = 1;
            float musicValue = 0.24f;
            float sfxValue = 0.24f;
            string name = string.Empty;
            int joystickActive = 0;
            if (PlayerPrefs.HasKey("Difficulty_"))
            {
                difficulty = PlayerPrefs.GetInt("Difficulty_");
            }
            if (PlayerPrefs.HasKey("Music_Volume_"))
            {
                musicValue = PlayerPrefs.GetFloat("Music_Volume_");
                sfxValue = PlayerPrefs.GetFloat("SFX_Volume_");
            }
            if (PlayerPrefs.HasKey("Players_name_"))
                name = PlayerPrefs.GetString("Players_name_");
            if (PlayerPrefs.HasKey("Joystick_"))
            {
                if (PlayerPrefs.GetInt("Joystick_") == 1)
                    joystickActive = 1;
                else
                    joystickActive = 0;
            }
            PlayerPrefs.DeleteAll();
            Debug.Log("Purging Data");
            Inventory.instance.PurgeInventoryForLoad();
            GameManager.instance.DestroyPlayer();
            PlayerPrefs.SetInt("Difficulty_", difficulty);
            PlayerPrefs.SetFloat("Music_Volume_", musicValue);
            PlayerPrefs.SetFloat("SFX_Volume_", sfxValue);
            PlayerPrefs.SetString("Players_name_", name);
            PlayerPrefs.SetInt("Joystick_", joystickActive);
            AudioManager.instance.SetVolumeMusic(musicValue);
            AudioManager.instance.SetVolumeSFX(sfxValue);
            GameManager.instance.newGame = true;
        });
        optionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        if (continueButton.GetComponent<Button>().isActiveAndEnabled)
        {
            MenuManager.instance.FadeImage();
            LeanTween.delayedCall(0.5f, () =>
            {
                GameManager.instance.continueGame = true;
                SceneManager.LoadScene("Loading Scene");
            });
        }
    }

    public void OptionsMenu()
    {
        SceneManager.LoadScene("Options");
    }

    public void OpenNewGamePanel()
    {
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        menuState = MenuState.newGame;
        ToggleButtonNavigation(false);
        inputField.OnDeselect(null);
        inputFieldSelected = true;
    }

    private void CloseNewGamePanel()
    {
        EventSystem.current.SetSelectedGameObject(newGameButton);
        optionsPanel.SetActive(false);
        menuState = MenuState.mainMenu;
        ToggleButtonNavigation(true);
    }

    public void Cancel()
    {
        if (inputFieldSelected)
            inputField.OnDeselect(null);
        else
            CloseNewGamePanel();
    }

    private void ToggleButtonNavigation(bool status)
    {
        foreach (Button button in buttons)
        {
            if (!status)
            {
                Navigation navigation = button.navigation;
                navigation.mode = Navigation.Mode.None;
                button.navigation = navigation;
            }
            else
            {
                Navigation navigation = button.navigation;
                navigation.mode = Navigation.Mode.Automatic;
                button.navigation = navigation;
            }
        }
    }

    private enum MenuState
    {
        mainMenu,
        newGame
    }
}
