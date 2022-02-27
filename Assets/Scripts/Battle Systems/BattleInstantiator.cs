using System.Collections;
using UnityEngine;

public class BattleInstantiator : MonoBehaviour
{
    [SerializeField] BattleTypeManager[] availableBattles;
    [SerializeField] Sprite backgroundImage;
    [SerializeField] bool activateOnEnter;
    [SerializeField] float timeBetweenBattles, chanceToFlee;
    [SerializeField] int battleMusic;
    [SerializeField] bool destroyOnVictory,canFlee, shouldCompleteQuest;
    public string questToComplete;

    IEnumerator StartTheBattle()
    {
        GameManager.instance.battleIsActive = true;

        MenuManager.instance.FadeImage();
        int rng = Random.Range(0, availableBattles.Length);
        BattleManager.instance.xpRewardAmount = availableBattles[rng].rewardXP;
        BattleManager.instance.itemsReward = availableBattles[rng].rewardItems;
        BattleManager.instance.goldRewardAmount = availableBattles[rng].rewardGold;
        BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;
        BattleRewardsHandler.instance.questToComplete = questToComplete;
        
        if (backgroundImage != null)
            BattleManager.instance.backgroundBattleImage.sprite = backgroundImage;
        yield return new WaitForSeconds(1f);
        MenuManager.instance.FadeOut();
        BattleManager.instance.StartBattle(availableBattles[rng].enemies);
        BattleManager.instance.ChanceToFlee(canFlee, chanceToFlee);
        if(destroyOnVictory)
            BattleManager.instance.battleInstantiator = this;
        AudioManager.instance.PlayBackgroundMusic(battleMusic);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (activateOnEnter)
            {
                StartCoroutine(StartTheBattle());
            }
            /*else
            {
                inArea = true;
            }*/
        }

    }

}