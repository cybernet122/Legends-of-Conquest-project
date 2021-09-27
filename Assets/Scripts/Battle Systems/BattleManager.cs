//using System;
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
    [SerializeField] GameObject[] border;

    List<BattleCharacters> activeBattleCharacters = new List<BattleCharacters>();
    //List<BattleCharacters> characterSpeed = new List<BattleCharacters>();

    [SerializeField] int currentTurn;
    [SerializeField] bool waitingForTurn;
    [SerializeField] GameObject UIButtonHolder;
    [SerializeField] BattleMoves[] battleMovesList;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        battleScene.SetActive(false);
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
/*        if (Input.GetKeyDown(KeyCode.KeypadMinus) && waitingForTurn)
        {
            print(activeBattleCharacters[0].characterName + " attacking");
            StartCoroutine(PlayerMoveCoroutine(activeBattleCharacters[0]));
        }*/
        CheckPlayerButtonHolder();
    }

    private void CheckPlayerButtonHolder()
    {
        if (isBattleActive && waitingForTurn)
        {
            UIButtonHolder.SetActive(true);
        }
        else
        {
            UIButtonHolder.SetActive(false);
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
            SortBySpeed();
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
        if (activeBattleCharacters[0].IsPlayer())
        {
            waitingForTurn = true;
            print("Current player's turn is " + activeBattleCharacters[0]);
        }
        else if (!activeBattleCharacters[0].IsPlayer())
        {
            StartCoroutine(EnemyMoveCoroutine(activeBattleCharacters[0]));
            print("Current player's turn is " + activeBattleCharacters[0]);
            activeBattleCharacters.RemoveAt(0);
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
        SortBySpeed();
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
        waitingForTurn = false;
        yield return new WaitForSeconds(1);
        EnemyAttack(enemyAttacking);
        activeBattleCharacters.Remove(enemyAttacking);
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
                players.Add(activeBattleCharacters[i]);
            }
        }
        BattleCharacters selectedPlayerToAttack = players[Random.Range(0, players.Count)];
        //print(enemyAttacking.characterName + " is attacking " + selectedPlayerToAttack.characterName);
        enemyAttacking.hasPlayed = true;
        int selectedAttack = Random.Range(0, enemyAttacking.AttackMovesAvailable().Length);
        for (int i = 0; i < battleMovesList.Length; i++)
        {
            if (battleMovesList[i].moveName == enemyAttacking.AttackMovesAvailable()[selectedAttack])
            {
                print("Enemy attacking with " + battleMovesList[i].moveName);
                if(battleMovesList[i].moveName == "Shadow")
                {
                    Instantiate(
                    battleMovesList[i].effectToUse,
                    new Vector3(enemyAttacking.transform.position.x - 3,enemyAttacking.transform.position.y,transform.position.z),
                    selectedPlayerToAttack.transform.rotation
                    );
                }
                else
                Instantiate(
                    battleMovesList[i].effectToUse,
                    selectedPlayerToAttack.transform.position,
                    selectedPlayerToAttack.transform.rotation
                    );
            }
        }
    }

    IEnumerator PlayerMoveCoroutine(BattleCharacters playerAttacking)
    {
        print(playerAttacking.characterName + " attacking");
        yield return new WaitForSeconds(1);
        playerAttacking.hasPlayed = true;
        activeBattleCharacters.Remove(playerAttacking);
        waitingForTurn = false;
        yield return new WaitForSeconds(1);
        NextTurn();
    }

    public void AttackEnemySelectionMenu()
    {
        print(activeBattleCharacters[0].characterName + " attacking");
        StartCoroutine(PlayerMoveCoroutine(activeBattleCharacters[0]));
    }

    /*public void IncreaseSpeed()
    {
        BattleCharacters playerSelected = activeBattleCharacters[0];
        if (currentTurn == currentTurn + 2 && playerSelected.characterName == activeBattleCharacters[0].characterName && buffApplied)
        {
            playerSelected.speed -= 5;
            print("Reseting Speed " + playerSelected);
            buffApplied = false;
            return;
        }
        else if (!buffApplied)
        {
            playerSelected.speed += 5;
            buffApplied = true;
            print("Increasing speed of " + playerSelected + " by " + playerSelected.speed);
            if (!playerSelected.hasPlayed)
            {
                SortBySpeed();
            }
            buffApplied = true;
            activeBattleCharacters.Remove(playerSelected);
            waitingForTurn = false;
            NextTurn();
        }
    }*/
}
