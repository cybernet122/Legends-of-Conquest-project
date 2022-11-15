using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] GameObject dialogueBubble;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("CheckForDialogue",0.5f);
    }

    private void SpawnBubble()
    {
        Instantiate(dialogueBubble, transform);
    }

    public void CheckForDialogue()
    {
        var dialogueHandlers = GetComponents<DialogHandler>();
        if (dialogueHandlers.Length > 0)
        {
            DialogHandler dialogueHandler = dialogueHandlers[0];
            for (int i = 0; i < dialogueHandlers.Length; i++)
            {
                if (dialogueHandlers[i].isActiveAndEnabled)                
                    dialogueHandler = dialogueHandlers[i];                
            }
            if (!dialogueHandler.triggerOnEntry)
                SpawnBubble();
        }
        else
        {
            if (dialogueBubble.activeInHierarchy)
                Destroy(dialogueBubble.gameObject);
        }
    }
}
