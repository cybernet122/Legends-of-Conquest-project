using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] GameObject objectToActivate;
    [SerializeField] string questToCheck;
    [SerializeField] bool activateIfComplete;

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.G))
        {
            CheckForCompletion();
        }*/
    }

    public void CheckForCompletion()
    {
        if (QuestManager.instance.CheckIfComplete(questToCheck))
        {
            objectToActivate.SetActive(activateIfComplete);
        }
    }

}
