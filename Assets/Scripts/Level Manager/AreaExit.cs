using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AreaExit : MonoBehaviour
{
    public string sceneToLoad;
    [SerializeField] string transitionName;
    [SerializeField] AreaEnter areaEnter;
    [SerializeField] string checkIfQuestIsComplete;
    [SerializeField] AreaEnter teleport;
    public bool teleportAvailable;

    private void Start()
    {
        /*areaEnter.transitionAreaName = transitionName;*/
        GameManager.SavePlayerPos();
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ChangingArea();
        }
    }

    public void ChangingArea()
    {
        if (checkIfQuestIsComplete != "")
        {
            if (!QuestManager.instance.CheckIfComplete(checkIfQuestIsComplete))
            {
                return;
            }
        }
        GameManager.instance.SaveSecondaryData();
        if (!teleport)
        {
            Player.instance.transitionName = transitionName;
            MenuManager.instance.FadeImage();
            StartCoroutine(LoadScene());
        }
        else if (teleport)
        {
            StartCoroutine(Delay());
        }
    }

    IEnumerator Delay()
    {
        GameManager.instance.gameMenuOpened = true;
        Player.instance.transform.position = new Vector3(teleport.transform.position.x, teleport.transform.position.y);
        yield return new WaitForSeconds(0.8f);
        GameManager.instance.gameMenuOpened = false;

    }

    IEnumerator LoadScene()
    {
        transform.GetChild(0).GetComponent<AreaInfoText>().StopCountdown();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
