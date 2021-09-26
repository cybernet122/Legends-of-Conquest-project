using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingFade : MonoBehaviour
{
    public void PlaySaveAnimation()
    {
        Animator savingPanel = GetComponent<Animator>();
        savingPanel.Play("Saving Fade");
    }
}
