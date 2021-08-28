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
    float dialogSkipTimer;
    string currentText;
    
    bool isTyping = false;
    bool checkingSkip = false;
    public bool npcInRange = false;
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
                currentSentence = 0;
                dialogText.text = null;
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
                print("Breaking");
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
        CheckForNPC();
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
            //dialogSentences = null;
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

    public void ActivateDialog(string[] newSentencesToUse)
    {
        dialogSentences = newSentencesToUse;
        dialogText.text = dialogSentences[currentSentence];
    }

    void CheckForName()
    {
        if (dialogSentences[currentSentence].StartsWith("#"))
        {
            nameText.text = dialogSentences[currentSentence].Replace("#", "");
            currentSentence++;
        }
        else
        {
            return;
        }
    }
}
