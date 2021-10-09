//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    bool isBattleActive;

    [SerializeField] GameObject battleScene;
    [SerializeField] Transform[] playersPositions,enemiesPositions;
    [SerializeField] BattleCharacters[] playerPrefabs, enemiesPrefabs;
    [SerializeField] TextMeshProUGUI[] playerNames,healthText,manaText;
    [SerializeField] GameObject[] border;
    [SerializeField] Slider[] sliderHP,sliderMana;
    [SerializeField] CharacterDamageGUI damageGUI;
    [SerializeField] TextMeshProUGUI currentTurnText;

    private List<BattleCharacters> activeBattleCharacters = new List<BattleCharacters>();
    private List<BattleCharacters> players = new List<BattleCharacters>();
    private List<BattleCharacters> enemies = new List<BattleCharacters>();

    private int currentTurn = 1;
    [SerializeField] bool waitingForTurn;
    [SerializeField] GameObject UIButtonHolder,enemyTargetPanel,returnButton;
    [SerializeField] BattleMoves[] battleMovesList;
    [SerializeField] BattleTargetButtons[] targetButtons;
    [SerializeField] BattleMagicButton magicButtonPrefab;
    [SerializeField] Animator fleeFade;
    int currentPlayerIndex;
    public GameObject magicPanel;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        battleScene.SetActive(false);
        enemyTargetPanel.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            foreach(BattleCharacters player in players)
            {
                if(!player.isDead)
                player.currentHP = 20;
            }
        }
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
            currentTurn = 1;
            SortBySpeed();
            NextTurn();
        }
    }

    public void UpdateText()
    {
        for(int i=0;i<players.Count;i++)
        {
            playerNames[i].text = players[i].characterName;
            sliderHP[i].maxValue = players[i].maxHP;
            sliderHP[i].value = players[i].currentHP;
            sliderMana[i].maxValue = players[i].maxMana;
            sliderMana[i].value = players[i].currentMana;
            healthText[i].text = Mathf.Clamp(players[i].currentHP,0,players[i].maxHP).ToString() + " / " + players[i].maxHP.ToString();
            manaText[i].text = Mathf.Clamp(players[i].currentMana, 0, players[i].maxMana).ToString() + " / " + players[i].maxMana.ToString();
            currentTurnText.text = "Current Turn: " + currentTurn;
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
                        enemies.Add(newEnemy);
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
                        players.Add(newPlayer);
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
            players.Clear();
            activeBattleCharacters.Clear();
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
            UpdateText();
        }
        if (activeBattleCharacters[0].IsPlayer() && !activeBattleCharacters[0].isDead)
        {
            waitingForTurn = true;
            ReturnButton();
            UpdateText();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].characterName == activeBattleCharacters[0].characterName)
                {
                    border[i].SetActive(true);
                    currentPlayerIndex = i;
                }
            }
            print("Current player's turn is " + activeBattleCharacters[0]);
        }
        else if (!activeBattleCharacters[0].IsPlayer() && !activeBattleCharacters[0].isDead)
        {
            StartCoroutine(EnemyMoveCoroutine(activeBattleCharacters[0]));
            print("Current player's turn is " + activeBattleCharacters[0]);
            activeBattleCharacters.RemoveAt(0);
        }
        else if (activeBattleCharacters[0].isDead)
        {
            activeBattleCharacters.RemoveAt(0);
            NextTurn();
        }
        CheckIfAllDead();
    }

    private void CheckIfAllDead()
    {
        int friendlies = 0,enemies = 0;
        var battleCharacters = FindObjectsOfType<BattleCharacters>();
        for (int i = 0; i < battleCharacters.Length; i++)
        {
            if(battleCharacters[i].IsPlayer() && !battleCharacters[i].isDead)
            {
                friendlies++;
            }
            else if(!battleCharacters[i].IsPlayer() && !battleCharacters[i].isDead)
            {
                enemies++;
            }
        }
        if (friendlies > 0 && enemies == 0)
        {
            print("Victory");
        }
        else if (friendlies == 0 && enemies > 0)
        {
            print("Defeat");
        }
        else
        {
            return;
        }
        battleScene.SetActive(false);
        isBattleActive = false;
        players.Clear();
        activeBattleCharacters.Clear();
        StopAllCoroutines();
        currentTurn = 1;
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
        var battleCharacters = FindObjectsOfType<BattleCharacters>();
        for (int i = 0; i < battleCharacters.Length; i++)
        {
            if (battleCharacters[i].IsPlayer() && !battleCharacters[i].isDead)
            {
                players.Add(battleCharacters[i].GetComponent<BattleCharacters>());
            }
        }
        BattleCharacters selectedPlayerToAttack = players[Random.Range(0, players.Count)];
        enemyAttacking.hasPlayed = true;
        int selectedAttack = Random.Range(0, enemyAttacking.AttackMovesAvailable().Length);
        int movePower = 0;
        for (int i = 0; i < battleMovesList.Length; i++)
        {
            if (battleMovesList[i].moveName == enemyAttacking.AttackMovesAvailable()[selectedAttack])
            {
                print("Enemy attacking with " + battleMovesList[i].moveName);
                if (battleMovesList[i].moveName == "Shadow")
                {
                    Instantiate(
                    battleMovesList[i].effectToUse,
                    new Vector3(selectedPlayerToAttack.transform.position.x + 2, selectedPlayerToAttack.transform.position.y, transform.position.z),
                    selectedPlayerToAttack.transform.rotation);
                    float rng = Random.Range(0, 1f);
                    if (rng > 0.5f)
                    {
                        print("Succesfully stunned " + selectedPlayerToAttack.characterName);
                        activeBattleCharacters.Remove(selectedPlayerToAttack);
                    }
                }
/*                else if (battleMovesList[i].moveName == "Blood drain")
                {
                    Instantiate(
                    battleMovesList[i].effectToUse,
                    new Vector3(selectedPlayerToAttack.transform.position.x + 2, selectedPlayerToAttack.transform.position.y, transform.position.z),
                    selectedPlayerToAttack.transform.rotation);
                    selectedPlayerToAttack.
                }
*/
                else
                    movePower = InstantiatingEffect(selectedPlayerToAttack, i);
            }
        }
        StartCoroutine(DealDamageToCharacters(selectedPlayerToAttack, movePower));
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

    IEnumerator DealDamageToCharacters(BattleCharacters selectedCharacterToAttack, int movePower)
    {
        float attackPower = activeBattleCharacters[0].dexterity + activeBattleCharacters[0].weaponPower;
        float defenceAmount = selectedCharacterToAttack.defence + selectedCharacterToAttack.armorDefence;
        float damageAmount = (attackPower / defenceAmount) * movePower * Random.Range(0.9f, 1.1f);
        int damageToGive = (int)damageAmount;
        damageToGive = CriticalStrike(damageToGive, selectedCharacterToAttack.transform);
        Debug.Log(activeBattleCharacters[0].characterName + " did " + damageToGive + " damage to " + selectedCharacterToAttack);
        yield return new WaitForSeconds(0f);
        selectedCharacterToAttack.TakeHpDamage(damageToGive);
    }

    private int CriticalStrike(int damage,Transform position)
    {
        if (Random.value > 0.8f)
        {
            damage *= 2;
            DamageText(damage, true, position);
            return damage;
        }
        else
        {
            DamageText(damage, false, position);
            return damage;
        }
        
    }

    private void DamageText(int damage,bool isCritical,Transform position)
    {
        CharacterDamageGUI damageUI = Instantiate(damageGUI, position.transform.position, transform.rotation);
        damageUI.SetDamage(damage, isCritical);
    }

    public void PlayerAttack(string moveName,BattleCharacters selectEnemyTarget)
    {
        int movePower = 0;
        for(int i = 0; i < battleMovesList.Length; i++)
        {
            if(battleMovesList[i].moveName == moveName)
            {
                movePower = InstantiatingEffect(selectEnemyTarget, i);
            }
        }
        StartCoroutine(DealDamageToCharacters(selectEnemyTarget, movePower));
        activeBattleCharacters[0].hasPlayed = true;
        activeBattleCharacters.RemoveAt(0);
        waitingForTurn = false;
        border[currentPlayerIndex].SetActive(false);
        enemyTargetPanel.SetActive(false);
        ReturnButton();
        Invoke("NextTurn",1f);
    }

    private int InstantiatingEffect(BattleCharacters selectEnemyTarget, int i)
    {
        activeBattleCharacters[0].currentMana -= battleMovesList[i].manaCost;
        int movePower;
        Instantiate(
               battleMovesList[i].effectToUse,
               selectEnemyTarget.transform.position,
               selectEnemyTarget.transform.rotation);
        movePower = battleMovesList[i].movePower;
        if (battleMovesList[i].moveName == "Blood drain")
        {
            int heal = activeBattleCharacters[0].currentHP + (movePower / 2);
            if (heal <= activeBattleCharacters[0].maxHP)
            {
                activeBattleCharacters[0].currentHP += (movePower / 2);
            }
        }
        return movePower;
    }

    public void OpenTargetPanel(string moveName)
    {
        returnButton.GetComponent<RectTransform>().localPosition = new Vector3(-875.7f, -250, 0);
        enemyTargetPanel.SetActive(true);
        returnButton.SetActive(true);
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].IsPlayer() && enemies[i].isDead)
            {
                enemies.RemoveAt(i);
                targetButtons[i].button.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (enemies.Count > i)
            {
                targetButtons[i].button.gameObject.SetActive(true);
                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattleTarget = enemies[i];
                targetButtons[i].targetName.text = enemies[i].characterName;
            }
        }
    }

    private void ReturnButton()
    {
        returnButton.SetActive(false);
        if (enemyTargetPanel.activeInHierarchy)
            enemyTargetPanel.SetActive(false);
        if (magicPanel.activeInHierarchy)
        {
            magicPanel.SetActive(false);
            var buttons = magicPanel.GetComponentsInChildren<Button>();
            if (buttons.Length >= 1)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    Destroy(buttons[i].gameObject); // Destroy previous buttons
                }
            }
        }
    }

    public void OpenMagicPanel()
    {
        var buttons = magicPanel.GetComponentsInChildren<Button>();
        if (buttons.Length >= 1)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject); // Destroy previous buttons
            }
        }
        magicPanel.SetActive(true);
        returnButton.SetActive(true);
        int count = 0;
        for (int i = 0; i < battleMovesList.Length; i++)
        {          
            for (int x = 0; x < activeBattleCharacters[0].AttackMovesAvailable().Length; x++) 
            {

                if (battleMovesList[i].moveName == activeBattleCharacters[0].AttackMovesAvailable()[x])
                {
                    var magicButton = Instantiate(magicButtonPrefab, magicPanel.transform);
                    magicButton.spellCost = battleMovesList[i].manaCost;
                    magicButton.spellCostText.text = magicButton.spellCost.ToString();
                    magicButton.spellName = battleMovesList[i].moveName;
                    magicButton.spellNameText.text = magicButton.spellName;
                    if (activeBattleCharacters[0].currentMana < battleMovesList[i].manaCost)
                    {
                        magicButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.75f);
                    }
                }
            }
        }
        for (int i = 1; i <= activeBattleCharacters[0].AttackMovesAvailable().Length; i++) 
        {
            if (i % 2 == 1)
                count++;
        }
        var magicRect = magicPanel.GetComponent<RectTransform>();
        if (count > 2)
        {
            magicRect.offsetMax = new Vector2(magicRect.sizeDelta.x, -1080);
            magicRect.offsetMax = new Vector2(magicRect.sizeDelta.x, -(-magicRect.sizeDelta.y - 140f * count) - (15 * count));
        }
        else
        {
            magicRect.offsetMax = new Vector2(magicRect.sizeDelta.x, -800);
        }
        var button = returnButton.GetComponent<RectTransform>();
        print(-magicRect.sizeDelta.y);
        button.offsetMin = new Vector2(0, magicRect.rect.height - 5);
        button.offsetMax = new Vector2(-1760, -((-magicRect.sizeDelta.y)-42));
    }

    public BattleCharacters GetPlayer()
    {
        if (activeBattleCharacters[0].IsPlayer())
            return activeBattleCharacters[0];
        else
            return null;
    }

    public void FleeBattle()
    {
        if (activeBattleCharacters[0].IsPlayer())
        {
            if (Random.value >= 0.8f)
            {
                SettingUpBattle();
            }
            else
            {
                print("Failed to flee");
                fleeFade.SetTrigger("Play Fade");
                border[currentPlayerIndex].SetActive(false);
                activeBattleCharacters[0].hasPlayed = true;
                waitingForTurn = false;
                activeBattleCharacters.RemoveAt(0);
                Invoke("NextTurn",2f);    
            }
        }
    }
}
