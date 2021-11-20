using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHandler : MonoBehaviour
{
    [SerializeField] bool shouldTriggerQuest;
    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete, destroyOnFinish;
    [SerializeField] string checkIfComplete;
    public string[] sentences;
    bool canActivateBox;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DialogController.instance.ActivateDialog(sentences,GetComponents<DialogHandler>());

            canActivateBox = true;
            SpokeToSara();
            ReturnFromMountains();
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
            if (shouldTriggerQuest)
            {
                DialogController.instance.ActivateQuestAtEnd(questToMark, markAsComplete);
            }
        }
        else
        { 
            DialogController.instance.npcInRange = false;
        }
    }
    private void Start()
    {
        SpokeToSara();
        Invoke("ReturnFromMountains",0.3f);
    }

    public void SpokeToSara()
    {
        var dialogHandlers = GetComponents<DialogHandler>();
        if (PlayerPrefs.GetInt("Spoke_To_Sara") == 1 && name == "Sara the Healer" && dialogHandlers.Length > 1)
        {
            Destroy(dialogHandlers[1]);
            print("Spoke to Sara");
        }
    }

    public void DestroyOnFinish()
    {
        if(destroyOnFinish)
            Destroy(this);
    }

    public void ReturnFromMountains()
    {
        var dialogHandlers = GetComponents<DialogHandler>();
        if (QuestManager.instance.CheckIfComplete(checkIfComplete) && dialogHandlers.Length > 1)
        {
            Destroy(dialogHandlers[1]);
        }
    }
}
