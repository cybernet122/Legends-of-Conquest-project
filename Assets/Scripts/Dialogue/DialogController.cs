using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogController : MonoBehaviour
{
    public static DialogController instance;
    [SerializeField] TextMeshProUGUI dialogText, nameText;
    [SerializeField] GameObject dialogBox,nameBox;
    [SerializeField] string[] dialogSentences;
    [SerializeField]int currentSentence;
    [Header("Dialogue Timers")]
    [SerializeField] float dialogSkipDelay = 0.5f;
    [SerializeField] float typewriterDelay = 0.1f;
/*    [SerializeField] Healer healer;*/
    float dialogSkipTimer;
    string currentText;
    string questToMark;
    [SerializeField]bool markTheQuestComplete ,shouldMarkQuest;
    bool isTyping = false;
    bool checkingSkip = false;
    public bool npcInRange = false;
    public float count = 0;
    private string npcName;
    [SerializeField] DialogHandler[] dialogHandler;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        dialogBox.SetActive(false);
    }

    IEnumerator ProcessWindowDialog()
    {

        //dialogText.text = dialogSentences[currentSentence];

        if (Input.GetButtonUp("Fire1") && !isTyping)
        {
            dialogBox.SetActive(true);
            GameManager.instance.dialogBoxOpened = true;
            if (dialogSentences.Length > currentSentence)
            {
                CheckForName();
                StartCoroutine(PrintText());
            }
            else
            {
                dialogBox.SetActive(false);
                GameManager.instance.dialogBoxOpened = false;
                if (shouldMarkQuest)
                {
                    if (markTheQuestComplete)
                    {           
                        if(!QuestManager.instance.CheckIfComplete(questToMark))
                            QuestManager.instance.MarkQuestComplete(questToMark);
                    }
                    else
                    {
                        QuestManager.instance.MarkQuestInComplete(questToMark);
                    }
                }
                dialogHandler[0].DestroyOnFinish();
                currentSentence = 0;
                dialogText.text = null;
                count = 0;
                if (dialogHandler.Length > 1)
                {
                    if(dialogHandler[0].name == "Sara the Healer")
                    {
                        PlayerPrefs.SetInt("Spoke_To_Sara", 1);
                        Destroy(dialogHandler[1]);
                        dialogHandler = new DialogHandler[1];
                    }
                }
            }
        }
        yield return new WaitForEndOfFrame();
    }

    IEnumerator PrintText()
    {      
        isTyping = true;
        int index = 0;
        for (int i = 0; i <= dialogSentences[currentSentence].Length; i++)
        {
            yield return new WaitForSeconds(typewriterDelay);
            if (checkingSkip)
            {
                dialogText.text = dialogSentences[currentSentence];
                checkingSkip = false;
                break;
            }
            currentText = dialogSentences[currentSentence].Substring(index, i);
            dialogText.text = currentText;
        }
        isTyping = false;
        currentSentence++;
    }

    private void Update()
    {
        if (count <= 0.7f)
            StartCoroutine(CountDown());
        if (!ShopManager.instance.shopMenu.activeInHierarchy && !MenuManager.instance.menu.activeInHierarchy && count >= 0.7f)
        {
            CheckForNPC();
        }
        if (!npcInRange)
        {
            if(markTheQuestComplete == true || questToMark != "" || shouldMarkQuest == true)
            {
                markTheQuestComplete = false;
                questToMark = "";
                shouldMarkQuest = false;
                DialogController.instance.dialogHandler = new DialogHandler[1];
            }
        }
        /*if (Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(dialogHandler[0].GetComponent<DialogHandler>());

        }*/
    }

    IEnumerator CountDown()
    {
        count = count + Time.deltaTime;
        yield return new WaitForEndOfFrame();
    }

    private void CheckForNPC()
    {
        if (npcInRange)
        {
            StartCoroutine(ProcessWindowDialog());
            if (Input.GetButtonUp("Fire1") && dialogSentences.Length <= currentSentence)
            {
                currentSentence = 0;                
            }
            CheckForSkip();
        }
        else
        {
            dialogBox.SetActive(false);
            currentSentence = 0;
            
        }
    }

    private void CheckForSkip()
    {
        if (isTyping)
        {
            dialogSkipTimer += Time.deltaTime;
            if (Input.GetButtonUp("Fire1") && dialogSkipTimer > dialogSkipDelay)
            {
                checkingSkip = true;
                dialogSkipTimer = 0;
            }
        }
        else { dialogSkipTimer = 0; }
    }

    public void ActivateDialog(string[] newSentencesToUse,DialogHandler[] dialogues)
    {
        dialogHandler = dialogues;
        dialogSentences = newSentencesToUse;
        dialogText.text = dialogSentences[currentSentence];
    }

    void CheckForName()
    {
        if (dialogSentences[currentSentence].StartsWith("#"))
        {
            nameText.text = dialogSentences[currentSentence].Replace("#", "");
            if (dialogSentences[currentSentence].EndsWith("$"))
            {
                nameText.text = nameText.text.Remove(nameText.text.Length - 1);
                var playerStats = GameManager.instance.GetPlayerStats();
                for (int i = 0; i < playerStats.Length; i++)
                {
                    int heal = (int)(playerStats[i].maxHP * 0.33);
                    if (playerStats[i].currentHP < heal)
                        playerStats[i].currentHP = heal;
                }
            }
            currentSentence++;
        }
        
        else
        {
            return;
        }
    }

    public void ActivateQuestAtEnd(string questName, bool markComplete)
    {        
        questToMark = questName;
        markTheQuestComplete = markComplete;
        shouldMarkQuest = true;
    }
}
