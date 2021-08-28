using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] GameObject menu;
    // Start is called before the first frame update
    public static MenuManager instance;
    bool togl = false;
    private void Start()
    {
        instance = this;
        menu.SetActive(false);
    }

    public void FadeImage()
    {
        image.GetComponent<Animator>().SetTrigger("StartFade");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //Toggle Menu
        {
            togl = !togl;
            menu.SetActive(togl);
            GameManager.instance.gameMenuOpened = togl;
        }
    }
}
