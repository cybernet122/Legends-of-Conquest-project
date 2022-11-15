using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogHandler : MonoBehaviour
{
    [SerializeField] bool shouldTriggerQuest;
    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete, destroyOnFinish;
    [SerializeField] string checkIfComplete;
    public bool triggerOnEntry;
    [SerializeField] float delayActivation;
    [SerializeField,TooltipAttribute("Remember if the object has been destroyed before(Useful on scene changes).")] 
    bool rememberDestruction;
    public string[] sentences;
    bool canActivateBox;
    DialogueBubble dialogueBubble;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player>())
        {
            canActivateBox = true;
            DialogController.instance.npcInRange = true;
            DialogController.instance.ActivateDialog(sentences,GetComponents<DialogHandler>());
            if (shouldTriggerQuest)            
                DialogController.instance.ActivateQuestAtEnd(questToMark, markAsComplete);                        
            SpokeToSara();
            if (triggerOnEntry)
            {
                LeanTween.delayedCall(delayActivation, () =>
                {
                    DialogController.instance.triggerOnEntry = true;
                });
            }
        }
    }

    public string ReturnCheckIfCompleteQuest()
    {
        return checkIfComplete;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canActivateBox = false;
            //
            DialogController.instance.npcInRange = false;
            //
        }
    }

    private void OnEnable()
    {
        if (checkIfComplete == "Look for the heroes located in the cave and join them")
            DialogController.SpokeToHeroes += CheckIfSpokeToHeroes;
    }

    private void OnDisable()
    {
        if (checkIfComplete == "Look for the heroes located in the cave and join them")
            DialogController.SpokeToHeroes -= CheckIfSpokeToHeroes;
    } 

    private void Start()
    {
        dialogueBubble = GetComponent<DialogueBubble>();

        /*          SpokeToSara();*/
        if (GetComponents<DialogHandler>().Length > 1)
            return;
        if (PlayerPrefs.GetInt("Destroy_on_start" + name) == 1)
        {
            if (triggerOnEntry)
                Destroy(gameObject);
            else
            Destroy(this);
        }
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
        if (destroyOnFinish)
        {
            DialogController.instance.npcInRange = false;
            if (rememberDestruction)            
                PlayerPrefs.SetInt("Destroy_on_start"+ name, 1);            
            if (triggerOnEntry)            
                Destroy(gameObject, 0.3f);            
            Destroy(this,0.2f);
        }
        if (dialogueBubble != null)
            dialogueBubble.CheckForDialogue();
    }

    public void CheckIfSpokeToHeroes()
    {
        string quest = "Look for the heroes located in the cave and join them";
        if (checkIfComplete == quest && QuestManager.instance.CheckIfComplete(quest))        
            Destroy(gameObject);        
    }

    public bool ReturnDestroy()
    {
        return destroyOnFinish;
    }
}
