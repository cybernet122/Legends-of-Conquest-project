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
                    return;
                }
            }
            Player.instance.transitionName = transitionName;
            MenuManager.instance.FadeImage();
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneToLoad);
    }
}
