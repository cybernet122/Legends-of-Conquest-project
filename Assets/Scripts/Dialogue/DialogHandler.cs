using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHandler : MonoBehaviour
{
    [SerializeField] bool shouldActivateQuest;
    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete;
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
            if (shouldActivateQuest)
                DialogController.instance.ActivateQuestAtEnd(questToMark, markAsComplete);
        }
        else
        { 
            DialogController.instance.npcInRange = false; 
        }
    }


}
