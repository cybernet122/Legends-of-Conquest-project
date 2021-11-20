using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleRewardsHandler : MonoBehaviour
{
    public static BattleRewardsHandler instance;
    [SerializeField] TextMeshProUGUI xpText,itemsText;
    [SerializeField] GameObject rewardsScreen;
    [SerializeField] ItemsManager[] rewardItems;
    [SerializeField] int xpReward;
    public bool markQuestComplete;
    public string questToComplete;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit") && rewardsScreen.activeInHierarchy)
        {
            CloseRewardScreen();
        }
    }
    
    public void OpenRewardScreen(int xpEarned,ItemsManager[] itemsEarned)
    {
        xpReward = xpEarned;
        rewardItems = itemsEarned;
        xpText.text = xpEarned + " XP";
        itemsText.text = "";
        foreach(ItemsManager rewardItemText in rewardItems)
        {
            if (itemsText.text == "")
                itemsText.text = rewardItemText.itemName;
            itemsText.text += ", " + rewardItemText.itemName;
        }
        rewardsScreen.SetActive(true);
        BattleManager.instance.isRewardScreenOpen = true;
    }

    public void CloseRewardScreen()
    {
        var playerStats = GameManager.instance.GetPlayerStats();
        foreach (PlayerStats player in GameManager.instance.GetPlayerStats())
        {
            print(player.playerName);
            for (int i = 0; i < playerStats.Length; i++) {
                PlayerStats.instance.AddXP(xpReward);                
            }
        }
        foreach (ItemsManager itemRewarded in rewardItems)
        {
            Inventory.instance.AddItems(itemRewarded, false);
        }
        rewardsScreen.SetActive(false);
        GameManager.instance.battleIsActive = false;
        BattleManager.instance.isRewardScreenOpen = false;
        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }
        QuestManager.instance.MountainsQuest();
    }
}
