using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public bool inRange;
    private bool chestNotOpened= true;
    [SerializeField] BattleTypeManager[] availableItems;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && chestNotOpened)
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(inRange && Input.GetButtonDown("Fire1") && chestNotOpened)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        chestNotOpened = false;
        int rng = Random.Range(0,availableItems.Length -1);
        BattleRewardsHandler.instance.OpenRewardScreen(availableItems[rng].rewardXP, availableItems[rng].rewardItems, availableItems[rng].rewardGold, false);
    }
}
