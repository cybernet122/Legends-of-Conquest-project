using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    [SerializeField] Image image;
    // Start is called before the first frame update
    public static MenuManager instance;
    private void Start()
    {
        instance = this;
    }

    public void FadeImage()
    {
        image.GetComponent<Animator>().SetTrigger("StartFade");
    }
}
