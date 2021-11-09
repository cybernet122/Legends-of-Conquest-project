using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AreaExit : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] string transitionName;
    [SerializeField] AreaEnter areaEnter;
    [SerializeField] string checkIfQuestIsComplete;
    [SerializeField] AreaEnter teleport;

    private void Start()
    {
        /*areaEnter.transitionAreaName = transitionName;*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (checkIfQuestIsComplete != "")
            {
                if (!QuestManager.instance.CheckIfComplete(checkIfQuestIsComplete))
                {
                    print(!QuestManager.instance.CheckIfComplete(checkIfQuestIsComplete));
                    return;
                }
            }
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
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneToLoad);
    }
}
