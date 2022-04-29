using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingFade : MonoBehaviour
{
    public static SavingFade instance;
    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySaveAnimation()
    {
        Animator savingPanel = GetComponent<Animator>();
        savingPanel.Play("Saving Fade");
    }
}
