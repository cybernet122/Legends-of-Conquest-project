using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastScene : MonoBehaviour
{
    CanvasGroup canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    public void Press()
    {
        Utilities.LoadMainMenu();
    }

    // Update is called once per frame
    void Update()
    {

        LoadFade();
    }

    private void LoadFade()
    {
        if(canvas.alpha > 0)        
            Utilities.FadeIn(canvas);
    }
}
