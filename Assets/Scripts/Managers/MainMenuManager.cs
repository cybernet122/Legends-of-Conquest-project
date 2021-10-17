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
        AudioManager.instance.PlayBackgroundMusic(4);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(newGameScene);
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
}
