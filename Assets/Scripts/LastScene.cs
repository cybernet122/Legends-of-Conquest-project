using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastScene : MonoBehaviour
{
    CanvasGroup canvas;
    int screenHeight, screenWidth;
    [SerializeField] SpriteRenderer imageToResize;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 1;
    }

    public void Press()
    {
        Utilities.LoadMainMenu();
        PlayerPrefs.DeleteAll();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForResize();
        LoadFade();
    }

    private void LoadFade()
    {
        if(canvas.alpha > 0)        
            Utilities.FadeIn(canvas);
    }

    public void CheckForResize()
    {
        int height = Screen.height;
        int width = Screen.width;
        if (height != screenHeight || width != screenWidth)
        {
            ResizeBackground();
            screenWidth = width;
            screenHeight = height;
        }
    }

    private void ResizeBackground()
    {
        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        imageToResize.transform.localScale = new Vector3(
            worldScreenWidth / imageToResize.sprite.bounds.size.x,
            worldScreenHeight / imageToResize.sprite.bounds.size.y, 1);
    }
}
