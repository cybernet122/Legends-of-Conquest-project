using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHandler : MonoBehaviour
{
    public string[] sentences;
    bool canActivateBox;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DialogController.instance.ActivateDialog(sentences);
            canActivateBox = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canActivateBox = false;
        }    
    }

    private void Update()
    {
        if(canActivateBox)
        {
            DialogController.instance.npcInRange = true;
        }
        else
        { 
            DialogController.instance.npcInRange = false; 
        }
    }
}
