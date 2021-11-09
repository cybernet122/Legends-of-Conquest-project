using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    [SerializeField] string[] questNames;
    [SerializeField] bool[] questMarkersCompleted;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if (questMarkersCompleted.Length <= 0)
        {
            questMarkersCompleted = new bool[questNames.Length];
            LoadQuestData();
        }
    }

    private void OnLevelWasLoaded()
    {
        Invoke("LoadQuestData", 0.15f);
    }

    public int GetQuestNumber(string questToFind)
    {
        for(int i = 0; i < questNames.Length; i++) 
        {
            if (questNames[i] == questToFind)
            {
                return i;
            }
        }
        Debug.LogWarning("Failed to get quest: " + questToFind);
        return 0;
    }

    public bool CheckIfComplete(string questToCheck)
    {        
        int questNumberToCheck = GetQuestNumber(questToCheck);
        if (questNumberToCheck == 0 && questNumberToCheck <= questNames.Length)
        {
            int value = PlayerPrefs.GetInt("QuestMarker_" + questToCheck);
            if(value == 1)
            return true;
        }
        return false;
    }

    public void UpdateQuestObjects()
    {
        var questObjects = FindObjectsOfType<QuestObject>();
        if (questObjects.Length > 1)
        {
            foreach (QuestObject quests in questObjects)
            {
                quests.CheckForCompletion();
            }
        }
    }
    public void MarkQuestComplete(string questToMark)
    {
        int questNumberToCheck = GetQuestNumber(questToMark);
        questMarkersCompleted[questNumberToCheck] = true;
        PlayerPrefs.SetInt("QuestMarker_" + questToMark , 1);
        GameManager.instance.SaveData();
        MenuManager.instance.UpdateQuest(GetCurrentQuest());
        UpdateQuestObjects();
  
    }

    public void MarkQuestInComplete(string questToMark)
    {
        int questNumberToCheck = GetQuestNumber(questToMark);
        questMarkersCompleted[questNumberToCheck] = false;
        UpdateQuestObjects();
    }

    public void SaveQuestData()
    {
        for (int i = 0; i < questNames.Length; i++)
        {
            if (questMarkersCompleted[i])
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i], 1);
            }
            else if (!questMarkersCompleted[i])
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i], 0);
            }
        }
    }

    public void LoadQuestData()
    {
        print("loading quest data");
        for (int i = 0; i < questNames.Length; i++)
        {
            string keyToUse = "QuestMarker_" + questNames[i];
            int valueToSet = 0;
            if (PlayerPrefs.HasKey(keyToUse))
            {
                valueToSet = PlayerPrefs.GetInt(keyToUse);
            }
            if (valueToSet == 0)
                questMarkersCompleted[i] = false;
            else
                questMarkersCompleted[i] = true;
        }
    }

    public string GetCurrentQuest()
    {
        LoadQuestData();
        for (int i = 0; i < questMarkersCompleted.Length; i++)
        {
            if (questMarkersCompleted[i] == false)
            {
                print(questNames[i] + " " + questMarkersCompleted[i]);
                return questNames[i];
            }
        }
        return "";
    }

    public void MountainsQuest()
    {
        if (CheckIfComplete("Kill the monsters in the mountains"))
        {
            print("failed");
            return;
        }
        else
        {
            var battleInstantiators = FindObjectsOfType<BattleInstantiator>();
            print("Fights left: " + battleInstantiators.Length);

            if(battleInstantiators.Length <= 0)
            {
                print("Quest Completed");
                MarkQuestComplete("Kill the monsters in the mountains");
            }
        }
    }
}
