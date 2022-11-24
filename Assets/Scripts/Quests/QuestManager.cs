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
    public int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
        if (SceneManager.GetActiveScene().name == "Mountains")
        {
            MountainsQuest();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AdvanceQuest();
            print("skipping quest");
        }
    }

    private void AdvanceQuest()
    {
        for (count = 0; count < questMarkersCompleted.Length; count++)
        {
            if (questMarkersCompleted[count] == false)
            {
                questMarkersCompleted[count] = true;
                break;
            }

        }
        SaveQuestData();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneWasSwitched;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneWasSwitched;
    }

    private void OnSceneWasSwitched(Scene scene, LoadSceneMode mode)
    {
        if (questMarkersCompleted.Length <= 0)
        {
            questMarkersCompleted = new bool[questNames.Length];
            LoadQuestData();
        }
        count = GetQuestNumber(GetCurrentQuest());

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
        if (questNumberToCheck >= 0 && questNumberToCheck <= questNames.Length)
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
        questMarkersCompleted[questNumberToCheck] = !questMarkersCompleted[questNumberToCheck];
        PlayerPrefs.SetInt("QuestMarker_" + questToMark , 1);
        LeanTween.delayedCall(0.2f,() =>
        GameManager.instance.SaveData()
            );
        if(!questMarkersCompleted[questMarkersCompleted.Length-1])
            MenuManager.instance.UpdateQuest(GetCurrentQuest());
        UpdateQuestObjects();
        if(questMarkersCompleted[questMarkersCompleted.Length-1])
            StartCoroutine(EndGame());
        if (questToMark == "Look for the heroes located in the cave and join them")
        {
            GameManager.instance.UpdatePlayerStats();
            GameManager.instance.UpdatePlayerLevels();
            HealthBarsUIManager.instance.UpdateHealthBars();
        }
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
                return questNames[i];
            }
        }
        return "";
    }

    public void MountainsQuest()
    {
        if (Utilities.ReturnSceneName() == "Mountains")
        {
            if (CheckIfComplete("Kill the monsters in the mountains"))
            {
                var battleInstantiators = FindObjectsOfType<BattleInstantiator>();
                for (int i = 0; i < battleInstantiators.Length; i++)
                {
                    Destroy(battleInstantiators[i].gameObject);
                }
                return;
            }
            else
            {
                var battleInstantiators = FindObjectsOfType<BattleInstantiator>();
                print("Fights left: " + battleInstantiators.Length);

                if (battleInstantiators.Length <= 0)
                {
                    MarkQuestComplete("Kill the monsters in the mountains");
                }
            }
        }
    }

    IEnumerator EndGame()
    {
        for(int i = 0; i < questMarkersCompleted.Length; i++)
        {
            if (questMarkersCompleted[i] == false)
            {
                yield return null;
            }
        }
        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Treasure");
        yield return new WaitForSeconds(0.4f);
        MenuManager.instance.FadeOut(0.2f);
    }

    [ContextMenu("Purge Save Data")]

    public void PurgeQuestData()
    {
        print("Purging quest data");
        for (int i = 0; i < questNames.Length; i++)
        {
            questMarkersCompleted = new bool[questNames.Length]; 
            PlayerPrefs.DeleteKey("QuestMarker_" + questNames[i]);
        }
        count = 0;
        SaveQuestData();
    }
}
