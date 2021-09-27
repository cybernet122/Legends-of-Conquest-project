//using System;
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
    [SerializeField] List<BattleCharacters> activeBattleCharacters = new List<BattleCharacters>();
    //List<BattleCharacters> characterSpeed = new List<BattleCharacters>();

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

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            NextTurn();
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) && waitingForTurn)
        {
            print(activeBattleCharacters[0].characterName + " attacking");
            StartCoroutine(PlayerMoveCoroutine(activeBattleCharacters[0]));

        }
        CheckPlayerButtonHolder();
    }

    private void CheckPlayerButtonHolder()
    {
        if (isBattleActive)
        {
            if (waitingForTurn)
            {
                if (activeBattleCharacters[0].IsPlayer())
                {
                    UIButtonHolder.SetActive(true);
                }
                else
                {
                    UIButtonHolder.SetActive(false);
                    StartCoroutine(EnemyMoveCoroutine(activeBattleCharacters[0]));
                }
            }
        }
    }

    public void StartBattle(string[] enemiesToSpawn)
    {
        SettingUpBattle();
        if (isBattleActive)
        {
            AddingPlayers();
            AddingEnemies(enemiesToSpawn);
            currentTurn = 0;
            NextTurn();
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

    private void NextTurn()
    {
        if (activeBattleCharacters.Count == 0)
        {
            FillActiveBattleCharacters();
            foreach (BattleCharacters battleCharacters in activeBattleCharacters)
            {
                battleCharacters.hasPlayed = false;
            }
            currentTurn++;
        }
        if (activeBattleCharacters.Count != 0)
            SortBySpeed();
        if (activeBattleCharacters[0].IsPlayer())
        {
            waitingForTurn = true;
        }
        else if (!activeBattleCharacters[0].IsPlayer())
        {
            StartCoroutine(EnemyMoveCoroutine(activeBattleCharacters[0]));
            activeBattleCharacters.RemoveAt(0);
        }
        foreach (BattleCharacters battleCharacters in activeBattleCharacters)
        {
            print(battleCharacters.speed + " " + battleCharacters.characterName);
        }

        // Old turn system
        /*currentTurn++;
        if (currentTurn >= activeBattleCharacters.Count)
            currentTurn = 0;
        waitingForTurn = true;
        UpdateBattle();*/
    }

    private void FillActiveBattleCharacters()
    {
        activeBattleCharacters.Clear();
        var battleCharacters = FindObjectsOfType<BattleCharacters>();
        for (int i=0;i< battleCharacters.Length; i++)
        {
            activeBattleCharacters.Add(battleCharacters[i]);
        }
    }

    public void ProcessSpeed()
    {

        for (int i = 0; i < activeBattleCharacters.Count; i++)
        {
            if (activeBattleCharacters[i].currentHP > 0 && !activeBattleCharacters[i].isDead && !activeBattleCharacters[i].hasPlayed)
            {
                activeBattleCharacters.Add(new BattleCharacters()
                {
                    characterName = activeBattleCharacters[i].characterName,
                    speed = activeBattleCharacters[i].speed,
                    hasPlayed = activeBattleCharacters[i].hasPlayed
                });
            }
        }
        SortBySpeed();
        /*characterSpeed.Sort(delegate (BattleCharacters x, BattleCharacters y)
        {
            if (x.speed == null || y.speed == null) return 0;
            else return x.speed.CompareTo(y.speed);
        });
        characterSpeed.Reverse();*/
    }

    private void SortBySpeed()
    {
        activeBattleCharacters.Sort(delegate (BattleCharacters x, BattleCharacters y)
        {
            if (x.speed == null || y.speed == null) return 0;
            else return x.speed.CompareTo(y.speed);
        });
        activeBattleCharacters.Reverse();
    }

    private void UpdateBattle()
    {
        bool allEnemiesAreDead = true;
        bool allPlayersAreDead = true;
        for (int i = 0; i < activeBattleCharacters.Count; i++)
        {
            if(activeBattleCharacters[i].currentHP < 0)
                activeBattleCharacters[i].currentHP = 0;
            if (activeBattleCharacters[i].currentHP == 0) { /* kill character */ }
            else
            {
                if (activeBattleCharacters[i].IsPlayer())
                    allPlayersAreDead = false;
                else
                    allEnemiesAreDead = false;
            }
        }
        if (allEnemiesAreDead || allPlayersAreDead)
        {
            if (allEnemiesAreDead)
                print("Victory");
            else if (allPlayersAreDead)
                print("Defeat");
            battleScene.SetActive(false);
            GameManager.instance.battleIsActive = false;
            isBattleActive = false;
        }
    }

    IEnumerator EnemyMoveCoroutine(BattleCharacters enemyAttacking)
    {
        print(enemyAttacking.characterName + " attacking");
        waitingForTurn = false;
        yield return new WaitForSeconds(1);
        EnemyAttack(enemyAttacking);
        activeBattleCharacters.RemoveAt(0);
        yield return new WaitForSeconds(1);
        NextTurn();
    }

    private void EnemyAttack(BattleCharacters enemyAttacking)
    {
        List<BattleCharacters> players = new List<BattleCharacters>();
        for (int i = 0; i < activeBattleCharacters.Count; i++)
        {
            if (activeBattleCharacters[i].IsPlayer() && activeBattleCharacters[i].currentHP > 0 && !activeBattleCharacters[i].isDead)
            {
                print("True");
                players.Add(activeBattleCharacters[i]);
            }
        }

        enemyAttacking.hasPlayed = true;
    }

    IEnumerator PlayerMoveCoroutine(BattleCharacters playerAttacking)
    {
        print(playerAttacking.characterName + " attacking");
        yield return new WaitForSeconds(1);
        playerAttacking.hasPlayed = true;
        activeBattleCharacters.RemoveAt(0);
        waitingForTurn = false;
        NextTurn();
    }
}
