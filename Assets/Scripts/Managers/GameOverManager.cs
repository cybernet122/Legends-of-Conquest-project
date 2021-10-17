using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBackgroundMusic(6);
        Player.instance.gameObject.SetActive(false);
        MenuManager.instance.gameObject.SetActive(false);
        BattleManager.instance.gameObject.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        DestroyGameSession();
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadLastSave()
    {
        DestroyGameSession();
        SceneManager.LoadScene("Loading Scene");
    }

    private static void DestroyGameSession()
    {
        Destroy(GameManager.instance.gameObject);
        Destroy(Player.instance.gameObject);
        Destroy(MenuManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
