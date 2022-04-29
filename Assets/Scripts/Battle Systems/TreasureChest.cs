using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public bool inRange;
    private bool chestNotOpened= true;
    [SerializeField] BattleTypeManager[] availableItems;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Treasure_Chest_Opened_"))
        {
            if (PlayerPrefs.GetInt("Treasure_Chest_Opened_") == 1)
                chestNotOpened = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && chestNotOpened)
        {
            inRange = true;
            MenuManager.instance.treasureChest = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
            MenuManager.instance.treasureChest = null;
        }
    }

    public void OpenChest()
    {
        if (inRange && chestNotOpened)
        {
            chestNotOpened = false;
            GameManager.instance.DataToSave("Treasure_Chest_Opened_");
            int rng = Random.Range(0, availableItems.Length - 1);
            BattleRewardsHandler.instance.OpenRewardScreen(availableItems[rng].rewardXP, availableItems[rng].rewardItems, availableItems[rng].rewardGold, false);
        }
    }
}