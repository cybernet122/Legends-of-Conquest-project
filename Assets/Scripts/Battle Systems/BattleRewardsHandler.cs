using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleRewardsHandler : MonoBehaviour
{
    public static BattleRewardsHandler instance;
    [SerializeField] TextMeshProUGUI xpText,itemsText,goldRewardText, typeOfReward;
    [SerializeField] GameObject rewardsScreen;
    [SerializeField] ItemsManager[] rewardItems;
    [SerializeField] int xpReward, goldReward;
    public bool markQuestComplete;
    public string questToComplete;
    private float count;
    bool startCount =false;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (startCount && count <= 0.7f)
        {
            count += Time.deltaTime;
        }
        if (Input.GetButtonDown("Submit") && rewardsScreen.activeInHierarchy && count > 0.7f)
        {
            CloseRewardScreen();
            startCount = false;
            count = 0;
        }
    }
    
    public void OpenRewardScreen(int xpEarned,ItemsManager[] itemsEarned,int goldReward, bool fight)
    {
        startCount = true;
        if (fight)
            typeOfReward.text = "Victory!";
        else
            typeOfReward.text = "Items looted: ";
        xpReward = xpEarned;
        rewardItems = itemsEarned;
        this.goldReward = goldReward;
        xpText.text = xpEarned + " XP";
        goldRewardText.text = this.goldReward.ToString();
        itemsText.text = "";
        foreach(ItemsManager rewardItemText in rewardItems)
        {
            if (itemsText.text == "")
                itemsText.text = rewardItemText.itemName;
            else
                itemsText.text += ", " + rewardItemText.itemName;
        }        
        rewardsScreen.SetActive(true);
        if(fight)
        BattleManager.instance.isRewardScreenOpen = true;
    }

    public void CloseRewardScreen()
    {
        var playerStats = GameManager.instance.GetPlayerStats();
        foreach (PlayerStats player in GameManager.instance.GetPlayerStats())
        {
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
        WipeRewards();
        if (markQuestComplete)
        {
            QuestManager.instance.MarkQuestComplete(questToComplete);
        }
        QuestManager.instance.MountainsQuest();
    }

    private void WipeRewards()
    {
        xpText.text = "";
        itemsText.text = "";
        goldRewardText.text = "";
        typeOfReward.text = "";        
        rewardItems = new ItemsManager[0];
        xpReward = 0;
        goldReward = 0;
    }
}
