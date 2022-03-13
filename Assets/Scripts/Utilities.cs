using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class Utilities
{
    public static void PurgeSaveData()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public static void FadeIn(CanvasGroup canvas)
    {
        if(canvas.alpha > Mathf.Epsilon)
            canvas.alpha -= 0.5f * Time.deltaTime;
    }

    public static void FadeOut(CanvasGroup canvas)
    {
        if(canvas.alpha < 1)
            canvas.alpha += 0.5f * Time.deltaTime;
    }

    public static string ReturnSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static string ReturnPlayersName()
    {
        return PlayerPrefs.GetString("Players_Name_");
    }

    public static void SetSelectedAndHighlight(GameObject gameObject, Button button)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        button.OnSelect(null);
    }
}
