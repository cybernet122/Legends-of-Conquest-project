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

    [SerializeField] List<BattleCharacters> activeBattleCharacters = new List<BattleCharacters>();
    private List<BattleCharacters> players = new List<BattleCharacters>();
    private List<BattleCharacters> enemies = new List<BattleCharacters>();
    [SerializeField] List<Transform> items = new List<Transform>();

    private int currentTurn = 1;
    [SerializeField] List<BattleCharacters> characterToStun = new List<BattleCharacters>();
    [SerializeField] bool waitingForTurn;
    [SerializeField] GameObject UIButtonHolder,enemyTargetPanel,returnButton, useItemButton;
    [SerializeField] BattleMoves[] battleMovesList;
    [SerializeField] BattleTargetButtons[] targetButtons;
    [SerializeField] BattleMagicButton magicButtonPrefab;
    [SerializeField] Animator fleeFade;
    int currentPlayerIndex;
    public GameObject magicPanel,itemPanel;
    [SerializeField] ItemsManager selectedItem;
    [SerializeField] GameObject itemSlotContainer,characterChoicePanel, buttonBorderPrefab, itemBorderPrefab, characterChoiceButtonPrefab;
    [SerializeField] GameObject[] targetButton,mainPanelButtons;
    [SerializeField] TextMeshProUGUI[] targetName;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] TextMeshProUGUI itemName, itemDescription;
    bool hasActivatedBorder = false;
    GameObject menuBorder;
    List<GameObject> magicButtons = new List<GameObject>();
    int buttonBorderIndex;
    public int xpRewardAmount;
    public ItemsManager[] itemsReward;
    public bool isRewardScreenOpen;
    private bool canFlee;
    private float percentToFlee;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        battleScene.SetActive(false);
        enemyTargetPanel.SetActive(false);
        itemPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartBattle(new string[] { "Entropy Mage", "Entropy Battlemage", "Entropy Warlock" });
        }

        if (Input.GetButtonDown("Fire1") && isBattleActive)
        {
            if (!hasActivatedBorder)            
                hasActivatedBorder = true;            
            if (UIButtonHolder.activeInHierarchy)            
                mainPanelButtons[buttonBorderIndex].GetComponentInChildren<Button>().onClick.Invoke();            
            else if (magicPanel.activeInHierarchy)            
                magicButtons[buttonBorderIndex].GetComponentInChildren<Button>().onClick.Invoke();            
            else if (enemyTargetPanel.activeInHierarchy)            
                targetButtons[buttonBorderIndex].GetComponentInChildren<Button>().onClick.Invoke();            
            else if (characterChoicePanel.activeInHierarchy)            
                targetButton[buttonBorderIndex].GetComponentInChildren<Button>().onClick.Invoke();            
            else if (itemPanel.activeInHierarchy)            
                useItemButton.GetComponent<Button>().onClick.Invoke();            
            else if (isRewardScreenOpen)            
                BattleRewardsHandler.instance.CloseRewardScreen();            
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            foreach(BattleCharacters player in players)
            {
                if(!player.isDead)
                player.currentHP = 20;
            }
        }
        if (Input.GetButtonDown("Cancel"))
        {
            ReturnButton();
        }
        CheckPlayerButtonHolder();
    }

    private void CheckPlayerButtonHolder()
    {
        if (isBattleActive && waitingForTurn && !magicPanel.activeInHierarchy && !enemyTargetPanel.activeInHierarchy && !itemPanel.activeInHierarchy && !characterChoicePanel.activeInHierarchy)
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
            UpdateText();
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
        if (player.currentHP <= (player.maxHP * 0.20f))
        {
            player.currentHP =(int)(player.maxHP * 0.20f);
        }
        else
            activeBattleCharacters[i].currentHP = player.currentHP;
        activeBattleCharacters[i].maxHP = player.maxHP;
        if (player.currentMana <= (player.maxMana * 0.20f))
        {
            player.currentMana = (int)(player.maxMana * 0.20f);
        }
        else
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
            for(int i = 0; i < playersPositions.Length; i++)
            {
                if (playersPositions[i].transform.childCount >= 1)
                    Destroy(playersPositions[i].GetChild(1).gameObject);
            }
            for (int x = 0; x < enemiesPositions.Length; x++)
            {
                if(enemiesPositions[x].transform.childCount >= 1)
                    Destroy(enemiesPositions[x].GetChild(0).gameObject);
            }
            players.Clear();
            enemies.Clear();
            activeBattleCharacters.Clear();
            GameManager.instance.battleIsActive = false;
            itemPanel.SetActive(false);
            battleScene.SetActive(false);
            for (int i = 0; i < border.Length; i++)
            {
                border[i].SetActive(false);
            }
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
        int friendlies = 0,Enemies = 0;
        var battleCharacters = FindObjectsOfType<BattleCharacters>();
        for (int i = 0; i < battleCharacters.Length; i++)
        {
            if(battleCharacters[i].IsPlayer() && !battleCharacters[i].isDead)
            {
                friendlies++;
            }
            else if(!battleCharacters[i].IsPlayer() && !battleCharacters[i].isDead)
            {
                Enemies++;
            }
        }
        if (friendlies > 0 && Enemies == 0)
        {
            print("Victory");
            UpdatePlayerStats(battleCharacters,0);
            BattleRewardsHandler.instance.OpenRewardScreen(xpRewardAmount, itemsReward);
        }
        else if (friendlies == 0 && Enemies > 0)
        {
            print("Defeat");
            UpdatePlayerStats(battleCharacters,1);
        }
        else
        {
            return;
        }
        battleScene.SetActive(false);
        isBattleActive = false;
        players.Clear();
        enemies.Clear();
        activeBattleCharacters.Clear();
        for (int i = 0; i < battleCharacters.Length; i++)
        {
            Destroy(battleCharacters[i].gameObject);
        }
        StopAllCoroutines();
        currentTurn = 1;
    }

    private void UpdatePlayerStats(BattleCharacters[] battleCharacters,int index)
    {
        for (int i = 0; i < battleCharacters.Length; i++)
        {
            if(index == 0 && battleCharacters[i].IsPlayer())
            {
                battleCharacters[i].ExitBattle(0);
            }
            else if(index == 1 && battleCharacters[i].IsPlayer())
            {
                battleCharacters[i].ExitBattle(1);
            }
        }
    }

    private void FillActiveBattleCharacters()
    {
        activeBattleCharacters.Clear();
        var battleCharacters = FindObjectsOfType<BattleCharacters>();
        for (int i=0;i< battleCharacters.Length; i++)
        {
            activeBattleCharacters.Add(battleCharacters[i]);
            /*if (characterToStun.Count > 1)
            {
                if (characterToStun.Contains(battleCharacters[i]))
                {
                    activeBattleCharacters.Remove(battleCharacters[i]);
                    characterToStun.Remove(battleCharacters[i]);
                }
            }*/
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
        yield return new WaitForSeconds(1.5f);
        EnemyAttack(enemyAttacking);
        yield return new WaitForSeconds(1);
        activeBattleCharacters.Remove(enemyAttacking);
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
/*                else if (battleMovesList[i].moveName == "Blood drain")
                {
                    Instantiate(
                    battleMovesList[i].effectToUse,
                    new Vector3(selectedPlayerToAttack.transform.position.x + 2, selectedPlayerToAttack.transform.position.y, transform.position.z),
                    selectedPlayerToAttack.transform.rotation);
                    selectedPlayerToAttack.
                }
*/              
                movePower = InstantiatingEffect(selectedPlayerToAttack,enemyAttacking, i);
            }
        }
        var attack = battleMovesList[selectedAttack];
        StartCoroutine(DealDamageToCharacters(selectedPlayerToAttack,enemyAttacking, movePower, attack));
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

    IEnumerator DealDamageToCharacters(BattleCharacters selectedCharacterToAttack,BattleCharacters characterAttacking, int movePower, BattleMoves attack)
    {
        float attackPower = characterAttacking.dexterity + characterAttacking.weaponPower;
        float defenceAmount = selectedCharacterToAttack.defence + selectedCharacterToAttack.armorDefence;
        float damageAmount = (attackPower / defenceAmount) * movePower * Random.Range(0.9f, 1.1f);
        int damageToGive = (int)damageAmount;
        damageToGive = CriticalStrike(damageToGive, selectedCharacterToAttack.transform);
        Debug.Log(characterAttacking.characterName + " did " + damageToGive + " damage to " + selectedCharacterToAttack);
        yield return new WaitForSeconds(0.5f);
        if (attack.moveName == "Blood drain")
        {
            int heal = damageToGive / 2;
            if (!characterAttacking.IsPlayer())
                heal /= 2;
            characterAttacking.AddHP(heal);
            print("Healed " + characterAttacking + " for " + heal);
        }
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
        BattleCharacters playerAttacking = activeBattleCharacters[0]; 
        int movePower = 0;
        var attack = new BattleMoves();
        for (int i = 0; i < battleMovesList.Length; i++)
        {
            if(battleMovesList[i].moveName == moveName)
            {
                movePower = InstantiatingEffect(selectEnemyTarget,playerAttacking, i);
                attack = battleMovesList[i];
            }
        }
        StartCoroutine(DealDamageToCharacters(selectEnemyTarget,playerAttacking, movePower, attack));
        activeBattleCharacters[0].hasPlayed = true;
        activeBattleCharacters.RemoveAt(0);
        waitingForTurn = false;
        border[currentPlayerIndex].SetActive(false);
        enemyTargetPanel.SetActive(false);
        ReturnButton();
        Invoke("NextTurn",1f);
    }

    private int InstantiatingEffect(BattleCharacters selectEnemyTarget,BattleCharacters characterAttacking, int i)
    {
        
        if (battleMovesList[i].moveName != "Shockwave")
            characterAttacking.currentMana -= battleMovesList[i].manaCost;
        int movePower;
        if (battleMovesList[i].moveName == "Shadow")
        {
            if (!characterAttacking.IsPlayer())
            {
                var battleMove = Instantiate(
                battleMovesList[i].effectToUse,
                new Vector3(selectEnemyTarget.transform.position.x + 2, selectEnemyTarget.transform.position.y, transform.position.z),
                selectEnemyTarget.transform.rotation);
                battleMove.transform.localScale = new Vector3(-1, 1);
            }
            else
            {
                var battleMove = Instantiate(
                battleMovesList[i].effectToUse,
                new Vector3(selectEnemyTarget.transform.position.x - 2, selectEnemyTarget.transform.position.y, transform.position.z),
                selectEnemyTarget.transform.rotation);
                battleMove.transform.localScale = new Vector3(-1, 1);
                if (selectEnemyTarget.characterName == "Elder Fire Dragon")
                    battleMove.transform.position = new Vector3(
                        selectEnemyTarget.transform.position.x - 7,
                        selectEnemyTarget.transform.position.y - 2,
                        transform.position.z);
            }
            float rng = Random.Range(0, 1f);
            if (rng > 0.5f)
            {
                print("Succesfully stunned " + selectEnemyTarget.characterName);
                activeBattleCharacters.Remove(selectEnemyTarget);
            }
        }
        else
        {
            Instantiate(
            battleMovesList[i].effectToUse,
            selectEnemyTarget.transform.position,
            selectEnemyTarget.transform.rotation);
        }
        movePower = battleMovesList[i].movePower;
        return movePower;
    }

    public void OpenTargetPanel(string moveName)
    {
        returnButton.GetComponent<RectTransform>().localPosition = new Vector3(-880f, -238, 0);
        enemyTargetPanel.SetActive(true);
        returnButton.SetActive(true);
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].IsPlayer() && enemies[i].isDead)
            {
                /*targetButtons[i].button.gameObject.SetActive(false);*/
                enemies.RemoveAt(i);
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
            else
            {
                targetButtons[i].activeBattleTarget = null;
                targetButtons[i].button.gameObject.SetActive(false);
            }
        }
        DestroyPreviousBorder();
    }

    private void DestroyPreviousBorder()
    {
        if (hasActivatedBorder && enemyTargetPanel.activeInHierarchy)
        {
            Destroy(menuBorder);
            menuBorder = Instantiate(buttonBorderPrefab, targetButtons[0].transform);
            menuBorder.transform.SetSiblingIndex(0);
            buttonBorderIndex = 0;
        }
        else if (hasActivatedBorder && characterChoicePanel.activeInHierarchy)
        {
            Destroy(menuBorder);
            menuBorder = Instantiate(characterChoiceButtonPrefab, targetButton[0].transform);
            menuBorder.transform.SetSiblingIndex(0);
            buttonBorderIndex = 0;
        }
        else if (hasActivatedBorder && itemPanel.activeInHierarchy && items.Count > 0)
        {
            Destroy(menuBorder);
            menuBorder = Instantiate(itemBorderPrefab, items[0].transform);
            menuBorder.transform.SetSiblingIndex(0);
            buttonBorderIndex = 0;
            SelectedItemToUse(items[0].GetComponentInChildren<ItemButton>().itemOnButton);
        }
        else if (hasActivatedBorder && magicPanel.activeInHierarchy)
        {
            Destroy(menuBorder);
            menuBorder = Instantiate(buttonBorderPrefab, magicButtons[0].transform);
            menuBorder.transform.SetSiblingIndex(0);
            buttonBorderIndex = 0;
        }
        else if (hasActivatedBorder)
        {
            Destroy(menuBorder);
            menuBorder = Instantiate(buttonBorderPrefab, mainPanelButtons[0].transform);
            menuBorder.transform.SetSiblingIndex(0);
            buttonBorderIndex = 0;
        }
    }

    private void ReturnButton()
    {
        if (UIButtonHolder.activeInHierarchy)
            return;
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
        if (characterChoicePanel.activeInHierarchy)
            characterChoicePanel.SetActive(false);
        if (itemPanel.activeInHierarchy)
            itemPanel.SetActive(false);
        DestroyPreviousBorder();
    }

    public void OpenMagicPanel()
    {
        var buttons = magicPanel.GetComponentsInChildren<BattleMagicButton>();
        if (buttons.Length > 0)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject); // Destroy previous buttons
                magicButtons.Clear();
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
                    magicButtons.Add(magicButton.gameObject);
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
        button.offsetMin = new Vector2(0, magicRect.rect.height - 5);
        button.offsetMax = new Vector2(-1760, -((-magicRect.sizeDelta.y) - 42));
        DestroyPreviousBorder();
    }

    public BattleCharacters GetPlayer()
    {
        if (activeBattleCharacters[0].IsPlayer())
            return activeBattleCharacters[0];
        else
            return null;
    }

    public List<BattleCharacters> GetPlayers()
    {
        return players;
    }

    public void ChanceToFlee(bool flee,float chanceToFlee)
    {
        canFlee = flee;
        percentToFlee = -((chanceToFlee/100) - 1);
    }

    public void FleeBattle()
    {
        if (canFlee)
        {
            if (Random.value >= percentToFlee)
            {
                UpdatePlayerStats(players.ToArray(), 0);
                SettingUpBattle();
            }
            else
            {
                fleeFade.SetTrigger("Play Fade");
                fleeFade.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Failed to escape!";
                border[currentPlayerIndex].SetActive(false);
                activeBattleCharacters[0].hasPlayed = true;
                waitingForTurn = false;
                activeBattleCharacters.RemoveAt(0);
                Invoke("NextTurn", 3f);
            }
        }
        else
        {
            fleeFade.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Can't flee from this battle!";
            fleeFade.SetTrigger("Play Fade");
            border[currentPlayerIndex].SetActive(false);
            activeBattleCharacters[0].hasPlayed = true;
            waitingForTurn = false;
            activeBattleCharacters.RemoveAt(0);
            Invoke("NextTurn", 3f);
        }
    }

    private void UpdateItemsInInventory()
    {
        foreach (Transform itemSlot in itemSlotContainerParent)
        {
            Destroy(itemSlot.gameObject);
            items.Clear();
        }
        foreach (ItemsManager item in Inventory.instance.GetItemList())
        {
            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>();
            ItemButton itemButton = itemSlot.GetComponentInChildren<ItemButton>();
            Image image = itemButton.itemsImage;
            image.sprite = item.itemImage;
            TextMeshProUGUI itemAmountText = itemButton.itemAmountText;
            if (item.amount > 1)
            { itemAmountText.text = item.amount.ToString(); }
            else
            { itemAmountText.text = ""; }
            itemButton.itemOnButton = item;
            items.Add(itemSlot.transform);
        }
        DestroyPreviousBorder();
    }

    public void SelectedItemToUse(ItemsManager item)
    {
        selectedItem = item;
        itemName.text = item.itemName;

        if (selectedItem.itemType == ItemsManager.ItemType.Weapon)
            itemDescription.text = item.itemDescription + "(Adds: " + selectedItem.weaponDexterity + " weapon power)";
        else if (selectedItem.itemType == ItemsManager.ItemType.Armor)
            itemDescription.text = item.itemDescription + "(Adds: " + selectedItem.armorDefence + " armor defence)";
        else if (selectedItem.affectType == ItemsManager.AffectType.HP || selectedItem.affectType == ItemsManager.AffectType.Mana && selectedItem.itemType == ItemsManager.ItemType.Item)
            itemDescription.text = item.itemDescription + "(Adds: " + selectedItem.amountOfEffect + " " + selectedItem.affectType + ")";
        else itemDescription.text = item.itemDescription;
    }

    public void UseItem(int index)
    {
        players[index].UseItemInBattle(selectedItem);
        Inventory.instance.RemoveItem(selectedItem);
        UpdateText();
        UseItemEndTurn();

        UpdateItemsInInventory();
        selectedItem = null;
        itemName.text = "";
        itemDescription.text = "";
    }

    private void UseItemEndTurn()
    {
        activeBattleCharacters[0].hasPlayed = true;
        waitingForTurn = false;
        itemPanel.SetActive(false);
        border[currentPlayerIndex].SetActive(false);
        characterChoicePanel.SetActive(false);
        activeBattleCharacters.RemoveAt(0);
        Invoke("NextTurn", 1f);
    }

    public void OpenItemTargetPanel()
    {
        if (selectedItem)
        {
            characterChoicePanel.SetActive(true);
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].isDead)
                {
                    PlayerStats activePlayer = GameManager.instance.GetPlayerStats()[i];
                    targetButton[i].SetActive(true);
                    targetName[i].text = activePlayer.playerName;
                    bool activePlayherInHierarchy = activePlayer.gameObject.activeInHierarchy;
                    targetButton[i].transform.parent.gameObject.SetActive(activePlayherInHierarchy);
                }
            }
            DestroyPreviousBorder();
        }
    }

    public void OpenItemPanel()
    {
        itemPanel.SetActive(true);
        UpdateItemsInInventory();
    }

    public void ButtonNavigation(int horizontal, int vertical)
    {
        Destroy(menuBorder);
        if (vertical != 0)
            vertical *= -2;
        if (UIButtonHolder.activeInHierarchy)
        {
            if (buttonBorderIndex + horizontal < 0)
            {
                buttonBorderIndex = mainPanelButtons.Length - 1;
            }
            else if (buttonBorderIndex + horizontal >= mainPanelButtons.Length)
            {
                buttonBorderIndex = 0;
            }
            else
            {
                buttonBorderIndex += horizontal;
            }
            if (buttonBorderIndex + vertical < 0)
            {
                if (buttonBorderIndex % 2 == 0)
                    buttonBorderIndex = mainPanelButtons.Length - 2;
                else
                    buttonBorderIndex = mainPanelButtons.Length - 1;
            }
            else if (buttonBorderIndex + vertical >= mainPanelButtons.Length)
            {
                if (buttonBorderIndex % 2 == 0)
                    buttonBorderIndex = 0;
                else
                    buttonBorderIndex = 1;
            }
            else
            {
                buttonBorderIndex += vertical;
            }
            if (!hasActivatedBorder)
            {
                buttonBorderIndex = 0;
                hasActivatedBorder = true;
            }
            menuBorder = Instantiate(buttonBorderPrefab, mainPanelButtons[buttonBorderIndex].transform);
            menuBorder.transform.SetSiblingIndex(0);
        }
        else if (magicPanel.activeInHierarchy)
        {
            if (buttonBorderIndex + horizontal < 0)
            {
                buttonBorderIndex = magicButtons.Count - 1;
            }
            else if (buttonBorderIndex + horizontal >= magicButtons.Count)
            {
                buttonBorderIndex = 0;
            }
            else
            {
                buttonBorderIndex += horizontal;
            }
            if (buttonBorderIndex + vertical < 0)
            {
                if (buttonBorderIndex == 0 && magicButtons.Count % 2 == 0)
                    buttonBorderIndex = magicButtons.Count - 2;
                else
                    buttonBorderIndex = magicButtons.Count - 1;
            }
            else if (buttonBorderIndex + vertical >= magicButtons.Count)
            {
                if (buttonBorderIndex % 2 == 0)
                    buttonBorderIndex = 0;
                else
                    buttonBorderIndex = 1;
            }
            else
            {
                buttonBorderIndex += vertical;
            }
            if (!hasActivatedBorder)
            {
                buttonBorderIndex = 0;
                hasActivatedBorder = true;
            }
            menuBorder = Instantiate(buttonBorderPrefab, magicButtons[buttonBorderIndex].transform);
            menuBorder.transform.SetSiblingIndex(0);
        }
        else if (enemyTargetPanel.activeInHierarchy)
        {
            if (buttonBorderIndex + horizontal < 0)
            {
                buttonBorderIndex = targetButtons.Length - 1;
            }
            /*else if (buttonBorderIndex + horizontal >= targetButtons.Length) //backup for below condition
            {
                buttonBorderIndex = 0;
            }*/
            else if (!targetButtons[buttonBorderIndex + horizontal].activeBattleTarget)
            {
                buttonBorderIndex = 0;
            }
            else
            {
                buttonBorderIndex += horizontal;
            }
            if (buttonBorderIndex + vertical < 0)
            {
                buttonBorderIndex = targetButtons.Length - 1;
            }
            else if (buttonBorderIndex + vertical >= targetButtons.Length)
            {
                if (targetButtons.Length % 2 == 0)
                    buttonBorderIndex = 0;
                else
                    buttonBorderIndex = 1;
            }
            else
            {
                buttonBorderIndex += vertical;
            }
            if (!hasActivatedBorder)
            {
                buttonBorderIndex = 0;
                hasActivatedBorder = true;
            }
            if (!targetButtons[buttonBorderIndex].activeBattleTarget)
            {
                for (int i = targetButtons.Length - 1; i > 0; i--) // may be flawed
                {
                    if (targetButtons[i].activeBattleTarget)
                    {
                        buttonBorderIndex = i;
                        break;
                    }
                }
            }
            menuBorder = Instantiate(buttonBorderPrefab, targetButtons[buttonBorderIndex].transform);
            menuBorder.transform.SetSiblingIndex(0);
        }
        else if (itemPanel.activeInHierarchy && !characterChoicePanel.activeInHierarchy && items.Count > 0)
        {
            if (buttonBorderIndex + horizontal < 0)
            {
                buttonBorderIndex = items.Count - 1;
            }
            else if (buttonBorderIndex + horizontal >= items.Count)
            {
                buttonBorderIndex = 0;
            }
            else
            {
                buttonBorderIndex += horizontal;
            }
            if (buttonBorderIndex + vertical < 0)
            {
                if (buttonBorderIndex == 0 && items.Count % 2 == 0)
                    buttonBorderIndex = items.Count - 2;
                else
                    buttonBorderIndex = items.Count - 1;
            }
            else if (buttonBorderIndex + vertical >= items.Count)
            {
                if (buttonBorderIndex % 2 == 0)
                    buttonBorderIndex = 0;
                else
                    buttonBorderIndex = 1;
            }
            else
            {
                buttonBorderIndex += vertical;
            }
            if (!hasActivatedBorder)
            {
                buttonBorderIndex = 0;
                hasActivatedBorder = true;
            }
            if(items[buttonBorderIndex].GetComponentInChildren<ItemButton>().itemOnButton != null)
                SelectedItemToUse(items[buttonBorderIndex].GetComponentInChildren<ItemButton>().itemOnButton);
            menuBorder = Instantiate(itemBorderPrefab, items[buttonBorderIndex].transform);
            menuBorder.transform.SetSiblingIndex(0);            
        }
        else if (characterChoicePanel.activeInHierarchy)
        {
            if (buttonBorderIndex + horizontal < 0)
            {
                buttonBorderIndex = targetButton.Length - 1;
            }
            else if (buttonBorderIndex + horizontal >= targetButton.Length)
            {
                buttonBorderIndex = 0;
            }
            else
            {
                buttonBorderIndex += horizontal;
            }
            if (!hasActivatedBorder)
            {
                buttonBorderIndex = 0;
                hasActivatedBorder = true;
            }
            print(buttonBorderIndex);
            menuBorder = Instantiate(characterChoiceButtonPrefab, targetButton[buttonBorderIndex].transform);
            menuBorder.transform.SetSiblingIndex(0);
        }
    }
}    //TODO fix stun for next turn
