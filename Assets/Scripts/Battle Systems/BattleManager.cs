using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    bool isBattleActive;

    [SerializeField] GameObject battleScene;
    [SerializeField] Transform[] playersPositions,enemiesPositions;
    [SerializeField] BattleCharacters[] playerPrefabs, enemiesPrefabs;
    List<BattleCharacters> activeBattleCharacters = new List<BattleCharacters>();
    [SerializeField] int currentTurn;
    [SerializeField] bool waitingForTurn;
    [SerializeField] GameObject UIButtonHolder;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartBattle(new string[] { "Entropy Mage", "Entropy Battlemage", "Entropy Warlock" });
        }
    }

    public void StartBattle(string[] enemiesToSpawn)
    {
        SettingUpBattle();
        if (isBattleActive)
        {
            AddingPlayers();
            AddingEnemies(enemiesToSpawn);
        }
    }

    private void AddingEnemies(string[] enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            if (enemiesToSpawn[i] != null)
            {
                for (int j = 0; j < enemiesPrefabs.Length; j++)
                {
                    if (enemiesPrefabs[j].characterName == enemiesToSpawn[i])
                    {
                        BattleCharacters newEnemy = Instantiate(
                            enemiesPrefabs[j],
                            enemiesPositions[i].position,
                            enemiesPositions[i].rotation,
                            enemiesPositions[i]
                            );
                        activeBattleCharacters.Add(newEnemy);
                    }
                }
            }
        }

    }

    private void AddingPlayers()
    {
        PlayerStats[] playerStats = GameManager.instance.GetPlayerStats();
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                for (int j = 0; j < playerPrefabs.Length; j++)
                {
                    if (playerPrefabs[j].characterName == playerStats[i].playerName)
                    {
                        BattleCharacters newPlayer = Instantiate(
                            playerPrefabs[j],
                            playersPositions[i].position,
                            playersPositions[i].rotation,
                            playersPositions[i]
                            );
                        activeBattleCharacters.Add(newPlayer);
                        ImportPlayerStats(playerStats, i);
                    }
                }
            }
        }
    }

    private void ImportPlayerStats(PlayerStats[] playerStats, int i)
    {
        PlayerStats player = playerStats[i];
        activeBattleCharacters[i].currentHP = player.currentHP;
        activeBattleCharacters[i].maxHP = player.maxHP;
        activeBattleCharacters[i].currentMana = player.currentMana;
        activeBattleCharacters[i].maxMana = player.maxMana;
        activeBattleCharacters[i].dexterity = player.dexterity;
        activeBattleCharacters[i].defence = player.defence;
        activeBattleCharacters[i].weaponPower = player.weaponPower;
        activeBattleCharacters[i].armorDefence = player.armorDefence;
    }

    private void SettingUpBattle()
    {
        if (!isBattleActive)
        {
            isBattleActive = true;
            GameManager.instance.battleIsActive = true;
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            battleScene.SetActive(true);
        }
        else
        {
            isBattleActive = false;
            GameManager.instance.battleIsActive = false;
            battleScene.SetActive(false);
            return;
        }
    }
}
