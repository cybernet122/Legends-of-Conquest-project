using System.Collections;
using UnityEngine;

public class BattleInstantiator : MonoBehaviour
{
    [SerializeField] BattleTypeManager[] availableBattles;
    [SerializeField] Sprite backgroundImage;
    [SerializeField] bool activateOnEnter;
    [SerializeField] float timeBetweenBattles, chanceToFlee;
    [SerializeField] int battleMusic;
    [SerializeField] bool destroyOnVictory,canFlee, shouldCompleteQuest, rememberVictory;
    public string questToComplete;

    private void Start()
    {
        if (PlayerPrefs.HasKey(name))
            if (PlayerPrefs.GetInt(name) == 1)
                Destroy(gameObject);        
    }

    IEnumerator StartTheBattle()
    {
        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(1);
        GameManager.instance.battleIsActive = true;
        int rng = Random.Range(0, availableBattles.Length);
        BattleManager.instance.xpRewardAmount = availableBattles[rng].rewardXP;
        BattleManager.instance.itemsReward = availableBattles[rng].rewardItems;
        BattleManager.instance.goldRewardAmount = availableBattles[rng].rewardGold;
        BattleRewardsHandler.instance.markQuestComplete = shouldCompleteQuest;
        BattleRewardsHandler.instance.questToComplete = questToComplete;
        
        if (backgroundImage != null)
            BattleManager.instance.backgroundBattleImage.sprite = backgroundImage;
        MenuManager.instance.FadeOut(1.5f);
        BattleManager.instance.StartBattle(availableBattles[rng].enemies);
        BattleManager.instance.ChanceToFlee(canFlee, chanceToFlee);
        if (destroyOnVictory)
        {
            BattleManager.instance.battleInstantiator = this;
            BattleManager.instance.battleInstantiatorRememberVictory = rememberVictory;
        }
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