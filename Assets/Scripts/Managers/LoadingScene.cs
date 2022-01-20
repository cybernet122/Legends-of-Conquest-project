using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadingScene : MonoBehaviour
{
    [SerializeField] float waitToLoadTime =1;


    // Start is called before the first frame update
    void Start()
    {
        if(waitToLoadTime > 0)
        {
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(waitToLoadTime);
        if (PlayerPrefs.HasKey("Current_Scene"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("Current_Scene"));
            GameManager.instance.LoadData();
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
