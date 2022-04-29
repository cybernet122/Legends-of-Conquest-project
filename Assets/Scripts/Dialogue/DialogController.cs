using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.InputSystem;

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
    string currentText;
    string questToMark;
    [SerializeField]bool markTheQuestComplete ,shouldMarkQuest;
    bool isTyping = false;
    bool checkingSkip = false;
    public bool triggerOnEntry = false;
    public bool npcInRange = false;
    private bool countFinished = true;
    private string npcName;
    [SerializeField] DialogHandler[] dialogHandler;
    bool advance, finishCountDown, countStarted;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;        
        DontDestroyOnLoad(gameObject);
        dialogBox.SetActive(false);
        Invoke("ReturnFromMountains",0.3f);
    }
    private void Update()
    {
        if (!ShopManager.instance.shopMenu.activeInHierarchy && !MenuManager.instance.menu.activeInHierarchy && countFinished)
        {
            CheckForNPC();
        }
        if (!npcInRange)
        {
            if (markTheQuestComplete == true || questToMark != "" || shouldMarkQuest == true || triggerOnEntry == true)
            {
                markTheQuestComplete = false;
                questToMark = "";
                shouldMarkQuest = false;
                triggerOnEntry = false;
                dialogHandler = new DialogHandler[0];
            }
        }
    }

    public void StartDelay()
    {
        LeanTween.delayedCall(0.7f, () =>
        {
            countFinished = true;
        });
    }

    public void SkipDialogueButton(InputAction.CallbackContext context)
    {
        if (context.performed)        
            SkipDialogue();        
    }

    public void SkipDialogueTouch(InputAction.CallbackContext context)
    {
        if(dialogBox.activeInHierarchy && context.performed)
        SkipDialogue();
    }

    public void SkipDialogue()
    {    
        if (!ShopManager.instance.shopMenu.activeInHierarchy && !MenuManager.instance.menu.activeInHierarchy && npcInRange && countFinished && dialogSentences.Length > 0)
        {
            if (!isTyping)
            {
                advance = true;
                StartCoroutine(ProcessWindowDialog());
                countFinished = false;
                StartDelay();
                CountDownForSkip();
            }
            else
            {
                if (!finishCountDown)
                    CountDownForSkip();
                else
                {
                    checkingSkip = true;
                    finishCountDown = false;
                }
            }
        }
    }

    private void CountDownForSkip() //Time before you can skip
    {
        LeanTween.delayedCall(dialogSkipDelay, () =>
        {
            finishCountDown = true;
        });
    }

    IEnumerator ProcessWindowDialog()
    {
        if (advance && !isTyping || triggerOnEntry)
        {
            advance = false;
            dialogBox.SetActive(true);
            triggerOnEntry = false;
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
                        if (!QuestManager.instance.CheckIfComplete(questToMark))
                            QuestManager.instance.MarkQuestComplete(questToMark);
                    }
                    else
                    {
                        QuestManager.instance.MarkQuestInComplete(questToMark);
                    }
                }
                if (dialogHandler.Length >= 1)
                    dialogHandler[dialogHandler.Length - 1].DestroyOnFinish();
                string[] dialogSentences = new string[0];
                AdvanceDialogue();
                MultipleDialogues();
                ReturnFromMountains();
                currentSentence = 0;
                dialogText.text = null;
            }
        }
        yield return new WaitForEndOfFrame();
    }

    private void MultipleDialogues()
    {
        if (dialogHandler.Length > 1)
        {
            dialogHandler[1].DestroyOnFinish();            
            if (dialogHandler[0].name == "Sara the Healer")
            {
                PlayerPrefs.SetInt("Spoke_To_Sara", 1);
            }
        }
    }

    public void ReturnFromMountains()
    {
        if (Utilities.ReturnSceneName() == "Shop")
        {
            var shopkeeper = FindObjectOfType<ShopKeeper>();
            var dialogHandlers = shopkeeper.GetComponents<DialogHandler>();
            /*if (QuestManager.instance.CheckIfComplete("Speak to the Innkeeper") && dialogHandlers.Length > 1)
            {
                Destroy(dialogHandlers[1]);
                dialogHandler = new DialogHandler[1];
                dialogHandlers[0].enabled = true;
            }*/            
            if (QuestManager.instance.CheckIfComplete("Kill the monsters in the mountains") && dialogHandlers.Length >= 1)
            {
                if (dialogHandlers.Length > 1)
                {
                    Destroy(dialogHandlers[1]);
                    dialogHandlers[0].enabled = true;
                }
            }
            if(QuestManager.instance.CheckIfComplete("Return to the Innkeeper"))
            {
                foreach(DialogHandler dialog in dialogHandlers)
                {
                    Destroy(dialog);
                }
                dialogSentences = new string[0];
                shopkeeper.CheckForShop();
            }
        }
    }

    private void AdvanceDialogue()
    {
        if (dialogHandler.Length > 1)
        {
            if(dialogHandler[0].name == "ShopKeeper NPC") { return; }
            var dialogue = dialogHandler[0];
            dialogHandler = new DialogHandler[1] { dialogHandler[0] = dialogue };
            dialogSentences = dialogHandler[0].sentences;
        }

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

    private void CheckForNPC()
    {
        if (npcInRange)
        {
            StartCoroutine(ProcessWindowDialog());
            if (advance && dialogSentences.Length <= currentSentence)
            {
                currentSentence = 0;                
            }
            //CheckForSkip();
        }
        else
        {
            dialogBox.SetActive(false);
            currentSentence = 0;
            
        }
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
            if(nameText.text == Player.instance.playersName)
                nameText.text = PlayerPrefs.GetString("Players_name_");
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
        if (dialogSentences[currentSentence].Contains("%"))
        {
            string name = PlayerPrefs.GetString("Players_name_");
            string stringInput = dialogSentences[currentSentence];
            dialogSentences[currentSentence] = Regex.Replace(stringInput,"%", name);
        }
    }

    public void ActivateQuestAtEnd(string questName, bool markComplete)
    {        
        questToMark = questName;
        markTheQuestComplete = markComplete;
        shouldMarkQuest = true;
    }
}
