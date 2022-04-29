//using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    bool isBattleActive;

    [SerializeField] GameObject battleScene;
    public SpriteRenderer backgroundBattleImage;

    [SerializeField] Transform[] playersPositions,enemiesPositions;
    [SerializeField] BattleCharacters[] playerPrefabs, enemiesPrefabs;
    [SerializeField] TextMeshProUGUI[] playerNames,healthText,manaText;
    [SerializeField] GameObject[] border;
    [SerializeField] Slider[] sliderHP,sliderMana;
    [SerializeField] CharacterDamageGUI damageGUI;
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] GameObject[] playerCanvas;
    private List<BattleCharacters> activeBattleCharacters = new List<BattleCharacters>();
    private List<BattleCharacters> players = new List<BattleCharacters>();
    private List<BattleCharacters> enemies = new List<BattleCharacters>();
    private List<Transform> items = new List<Transform>();
    private List<BattleCharacters> characterToStun = new List<BattleCharacters>();
    private int currentTurn = 1;
    [SerializeField] bool waitingForTurn;
    [SerializeField] GameObject UIButtonHolder,enemyTargetPanel,returnButton, useItemButton;
    [SerializeField] BattleMoves[] battleMovesList;
    [SerializeField] BattleTargetButtons[] targetButtons;
    [SerializeField] BattleMagicButton[] magicButtons;
    [SerializeField] Animator fleeFade;
    int currentPlayerIndex;
    public GameObject magicPanel,itemPanel;
    [SerializeField] ItemsManager selectedItem;
    [SerializeField] GameObject itemSlotContainer, characterChoicePanel;
    [SerializeField] GameObject[] targetButton,mainPanelButtons;
    [SerializeField] TextMeshProUGUI[] targetName;
    [SerializeField] Transform itemSlotContainerParent;
    [SerializeField] TextMeshProUGUI itemName, itemDescription;
    [SerializeField] CanvasGroup battleCanvasGroup;
    //List<GameObject> magicButtons = new List<GameObject>();
    public int xpRewardAmount, goldRewardAmount;
    public ItemsManager[] itemsReward;
    public bool isRewardScreenOpen;
    private bool canFlee;
    private float percentToFlee;
    public BattleInstantiator battleInstantiator;
    private int screenHeight, screenWidth;
    [SerializeField] Camera cameraToResize;
    private int musicIndex;
    BattleMenuState battleMenuState = BattleMenuState.mainPanel;
    public GameObject abilityChosen;
    [SerializeField] Button[] useAndCloseButtons;
    GameObject activeItemObject;
    public static event UnityAction<string> MoveName;

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
        returnButton.SetActive(false);
        CheckForCamera();
        battleCanvasGroup.alpha = 0;
        if (PlayerPrefs.HasKey("Players_name_"))
            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i].characterName == Player.instance.playersName)
                    playerPrefabs[i].characterName = PlayerPrefs.GetString("Players_name_");
            }        
    }

    private void OnDisable()
    {
        if (PlayerPrefs.HasKey("Players_name_"))
            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i].characterName == PlayerPrefs.GetString("Players_name_"))
                    playerPrefabs[i].characterName = Player.instance.playersName;
            }
    }

    void Update()
    {
        CheckForResize();
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
            StartCoroutine(FadeTo(1, 1));
            NextTurn();
        }
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        yield return new WaitForSeconds(0.5f);
        float alpha = battleCanvasGroup.alpha;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            battleCanvasGroup.alpha = Mathf.Lerp(alpha, aValue, t);
            yield return null;
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
        }/*
        if (PlayerPrefs.HasKey("Players_name_"))
        {
            string playersName = PlayerPrefs.GetString("Players_name_");
            playerNames[0].text = playersName;
        }*/
        currentTurnText.text = "Current Turn: " + currentTurn;
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
                        if(newEnemy.characterName == "Elder Fire Dragon")
                            activeBattleCharacters.Add(newEnemy);
                        enemies.Add(newEnemy);
                        DifficultyModifier(newEnemy);
                    }
                }
            }
        }
    }

    private void DifficultyModifier(BattleCharacters enemy)
    {
        int playerLevel = Player.instance.ReturnPlayerStats().playerLevel;
        enemy.maxHP += (int)(enemy.maxHP * 0.05) * playerLevel;
        enemy.currentHP = enemy.maxHP;
        enemy.dexterity += (int)(playerLevel * 0.5);
        enemy.weaponPower += (int)(playerLevel * 0.3);
        enemy.defense += (int)(playerLevel * 0.4);
        if (PlayerPrefs.HasKey("Difficulty_"))
        {
            var difficulty = PlayerPrefs.GetInt("Difficulty_");
            float modifier = 1;
            switch (difficulty)
            {
                case 1:
                    modifier = 0.8f;
                    break;
                case 2:
                    return;                    
                case 3:
                    modifier = 1.2f;
                    break;
            }
            enemy.maxHP = (int)(enemy.maxHP * modifier);
            enemy.currentHP = enemy.maxHP;
            enemy.evasion *= modifier;
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
        for (int i = 0; i < playerNames.Length; i++)
        {
            if(players.Count < i)
                playerNames[i].transform.parent.gameObject.SetActive(false);
        }
    }

    private void ImportPlayerStats(PlayerStats[] playerStats, int i)
    {
        PlayerStats player = playerStats[i];
        if (player.currentHP <= (player.maxHP * 0.20f))        
            player.currentHP =(int)(player.maxHP * 0.20f);        
        else
            activeBattleCharacters[i].currentHP = player.currentHP;
        activeBattleCharacters[i].maxHP = player.maxHP;
        if (player.currentMana <= (player.maxMana * 0.20f))        
            player.currentMana = (int)(player.maxMana * 0.20f);        
        else
            activeBattleCharacters[i].currentMana = player.currentMana;
        activeBattleCharacters[i].maxMana = player.maxMana;
        activeBattleCharacters[i].dexterity = player.dexterity;
        activeBattleCharacters[i].defense = player.defense;
        activeBattleCharacters[i].weaponPower = player.weaponPower;
        activeBattleCharacters[i].armorDefense = player.armorDefense;
        activeBattleCharacters[i].speed = player.turnSpeed;
        activeBattleCharacters[i].evasion = player.evasion;
        activeBattleCharacters[i].lifestealWeap = player.lifestealWeap;
    }

    private void SettingUpBattle()
    {
        if (!isBattleActive)
        {
            isBattleActive = true;
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            //resize sprite renderer to fit camera
            StartCoroutine(ResizeBackground());
            MenuManager.instance.UpdateStats();
            GameManager.instance.UpdatePlayerStats();
            LeanTween.delayedCall(0.5f, () =>
            {
                for (int i = players.Count; i > 0; i--)               
                    playerCanvas[i - 1].SetActive(true);                
            });
            battleScene.SetActive(true);
            for (int i = 0; i < border.Length; i++)
            {
                border[i].SetActive(false);
            }
            EventSystem.current.firstSelectedGameObject = mainPanelButtons[0];
            EventSystem.current.SetSelectedGameObject(mainPanelButtons[0]);
            mainPanelButtons[0].GetComponent<Button>().OnSelect(null);
            SwitchActiveMap.instance.SwitchToBattleUI();
            if (Utilities.ReturnSceneName() != "GameOverScene")
                musicIndex = FindObjectOfType<CamController>().GetMusicIndex();
        }
        else
        {
            isBattleActive = false;
            for(int i = 0; i < playersPositions.Length; i++)
            {
                if (playersPositions[i].transform.childCount >= 1)
                    Destroy(playersPositions[i].GetChild(0).gameObject);
            }
            for (int x = 0; x < enemiesPositions.Length; x++)
            {
                if(enemiesPositions[x].transform.childCount >= 1)
                    Destroy(enemiesPositions[x].GetChild(0).gameObject);
            }
            MenuManager.instance.ResetToggles();
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
            AudioManager.instance.StopMusic();
        }
    }

    public void CheckForResize()
    {
        int height = Screen.height;
        int width = Screen.width;
        if (height != screenHeight || width != screenWidth)
        {
            StartCoroutine(ResizeBackground());
            screenWidth = width;
            screenHeight = height;
        }
    }

    IEnumerator ResizeBackground()
    {
        if (!isBattleActive) { yield break; }
        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        backgroundBattleImage.transform.localScale = new Vector3(
            worldScreenWidth / backgroundBattleImage.sprite.bounds.size.x,
            worldScreenHeight / backgroundBattleImage.sprite.bounds.size.y, 1);
        
        yield return new WaitForSeconds(0.3f);
        if (!cameraToResize)
            CheckForCamera();
        if (cameraToResize)
            transform.position = new Vector3(cameraToResize.transform.position.x, cameraToResize.transform.position.y, transform.position.z);
        else
            Debug.LogError("Couldn't find a camera to resize");
    }

    private void CheckForCamera()
    {
        cameraToResize = FindObjectOfType<Camera>();
    }

    private void NextTurn()
    {
        if (battleMenuState != BattleMenuState.mainPanel)
        {
            battleMenuState = BattleMenuState.mainPanel;
            EventSystem.current.SetSelectedGameObject(mainPanelButtons[0]);
            mainPanelButtons[0].GetComponent<Button>().OnSelect(null);
        }
        if (activeBattleCharacters.Count == 0)
        {
            FillActiveBattleCharacters();
            currentTurn++;
            UpdateText();
        }        
        if (activeBattleCharacters[0].IsPlayer() && !activeBattleCharacters[0].isDead)
        {
            waitingForTurn = true;
            Utilities.SetSelectedAndHighlight(mainPanelButtons[0], mainPanelButtons[0].GetComponent<Button>());
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
            //activeBattleCharacters.RemoveAt(0); // may break
        }
        else if (activeBattleCharacters[0].isDead)
        {
            activeBattleCharacters.RemoveAt(0);
            NextTurn();
        }
        if(currentTurn >= 1)
            CheckIfAllDead();
        CheckPlayerButtonHolder();
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
            UpdatePlayerStats(battleCharacters, 0);
            DestroyInstantiator();
            SwitchActiveMap.instance.SwitchToUI();
            BattleRewardsHandler.instance.OpenRewardScreen(xpRewardAmount, itemsReward, goldRewardAmount, true);
            QuestManager.instance.MountainsQuest();
        }
        else if (friendlies == 0 && Enemies > 0)
        {
            SettingUpBattle();
            LeanTween.delayedCall(0.2f, () =>
            {
                MenuManager.instance.ResetToggles();
                GameManager.instance.GameOver();
            });
        }
        else        
            return;        
        battleScene.SetActive(false);
        battleCanvasGroup.alpha = 0;
        isBattleActive = false;
        players.Clear();
        enemies.Clear();
        activeBattleCharacters.Clear();
        for (int i = 0; i < playerCanvas.Length; i++)
        {
            playerCanvas[i].SetActive(false);
        }
        for (int i = 0; i < battleCharacters.Length; i++)
        {
            Destroy(battleCharacters[i].gameObject);
        }
        StopAllCoroutines();
        currentTurn = 1;
        AudioManager.instance.PlayBackgroundMusic(musicIndex);
    }

    private void DestroyInstantiator()
    {
        if(battleInstantiator != null)
        Destroy(battleInstantiator.gameObject);
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
            if(battleCharacters[i].characterName == "Elder Fire Dragon")
            activeBattleCharacters.Add(battleCharacters[i]);

            if (characterToStun.Count > 1)
            {
                if (characterToStun.Contains(battleCharacters[i]))
                {
                    activeBattleCharacters.Remove(battleCharacters[i]);
                    characterToStun.Remove(battleCharacters[i]);
                }
            }
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
    }

    private void SortBySpeed()
    {
        activeBattleCharacters.Sort(delegate (BattleCharacters x, BattleCharacters y)
        {
            return x.speed.CompareTo(y.speed);
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
        if(players.Count == 0) { return; }
        BattleCharacters selectedPlayerToAttack = players[Random.Range(0, players.Count)];
        enemyAttacking.hasPlayed = true;
        int selectedAttack = Random.Range(0, enemyAttacking.AttackMovesAvailable().Length);
        int movePower = 0;
        BattleMoves attack;
        int x = 0;
        for (int i = 0; i < battleMovesList.Length; i++)
        {
            if (battleMovesList[i].moveName == enemyAttacking.AttackMovesAvailable()[selectedAttack])
            {
                print(enemyAttacking + " attacking with " + battleMovesList[i].moveName);
                x = i;
                if (Evade(selectedPlayerToAttack, enemyAttacking))
                    return;
                movePower = InstantiatingEffect(selectedPlayerToAttack, enemyAttacking, i);
            }

        }
        attack = battleMovesList[x];
        StartCoroutine(DealDamageToCharacters(selectedPlayerToAttack, enemyAttacking, movePower, attack));
    }

    IEnumerator DealDamageToCharacters(BattleCharacters selectedCharacterToAttack,BattleCharacters characterAttacking, int movePower, BattleMoves attack)
    {        
        print(characterAttacking.characterName + " attacking " + selectedCharacterToAttack.characterName);
        float attackPower = 0;
        if(attack.moveName == "Shockwave")
        {
            attackPower = characterAttacking.dexterity * 0.75f + characterAttacking.weaponPower * 0.65f;
        }
        else if(attack.moveName == "Enhanced Shockwave")
        {
            attackPower = characterAttacking.dexterity * 0.84f + characterAttacking.weaponPower * 0.75f;
        }
        else
        {
            attackPower = characterAttacking.dexterity * 0.62f + characterAttacking.weaponPower * 0.8f;
        }
        if (attack.moveName == "Tentacles")
        {
            if (characterAttacking.IsPlayer())
            {
                foreach (BattleCharacters enemy in enemies)
                {
                    int damage = (int)(attackPower / (enemy.defense + enemy.armorDefense) * movePower * Random.Range(0.9f, 1.1f));                    
                    if(!Evade(enemy,characterAttacking) && !enemy.isDead)
                    {
                        enemy.TakeHpDamage(damage);
                        DamageText(damage, false, enemy.transform);
                        Debug.Log(characterAttacking.characterName + " did " + damage + " damage to " + selectedCharacterToAttack);
                    }
                }
                yield return new WaitForSeconds(0.5f);
                yield return null;
            }
            else if(!characterAttacking.IsPlayer())
            {
                foreach (BattleCharacters player in players)
                {
                    int damage = (int)(attackPower / (player.defense + player.armorDefense) * movePower * Random.Range(0.9f, 1.1f));
                    if (!Evade(player,characterAttacking) && !player.isDead)
                    {
                        player.TakeHpDamage(damage);
                        DamageText(damage, false, player.transform);
                        Debug.Log(characterAttacking.characterName + " did " + damage + " damage to " + selectedCharacterToAttack);
                    }
                }
                yield return new WaitForSeconds(0.5f);
                yield return null;
            }
        }
        else
        {
            float defenseAmount = selectedCharacterToAttack.defense + selectedCharacterToAttack.armorDefense;
            float damageAmount = (attackPower / defenseAmount) * movePower * Random.Range(0.9f, 1.1f);
            int damageToGive = (int)damageAmount;
            damageToGive = CriticalStrike(damageToGive, selectedCharacterToAttack.transform);
            Debug.Log(characterAttacking.characterName + " did " + damageToGive + " damage to " + selectedCharacterToAttack);
            yield return new WaitForSeconds(0.5f);
            if ((attack.moveName == "Shockwave" || attack.moveName == "Enchanced Shockwave") && characterAttacking.IsPlayer())
            {
                if (characterAttacking.lifestealWeap)
                {
                    int heal = damageToGive / 2;
                    characterAttacking.AddHP(heal);
                    print("Player recieved " + heal + " from lifesteal");
                } 
            }
            if (attack.moveName == "Blood drain")
            {
                int heal = damageToGive / 2;
                characterAttacking.AddHP(heal);                
                print("Healed " + characterAttacking + " for " + heal);
            }
            selectedCharacterToAttack.TakeHpDamage(damageToGive);
        }
    }

    private bool Evade(BattleCharacters target,BattleCharacters characterAttacking)
    {
        float chanceToHit = Random.Range(0f, 1f);        
        if ((target.evasion / 100) < chanceToHit)
        {            
            return false;
        }
        else
        {
            Debug.Log(characterAttacking.characterName + " Missed!");
            DamageText(0, false, target.transform);
            return true;
        }
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
    
    private void DamageText(string text,Transform position)
    {
        LeanTween.delayedCall(1f, () =>
        {
            CharacterDamageGUI damageUI = Instantiate(damageGUI, position.transform.position, transform.rotation);
            damageUI.SetDamage(text);
        });
    }

    public void PlayerAttack(string moveName,BattleCharacters selectEnemyTarget)
    {
        if (!waitingForTurn) { return; } 
        waitingForTurn = false;
        MoveName?.Invoke("");
        CheckPlayerButtonHolder();
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
        if (!Evade(selectEnemyTarget, playerAttacking))
            StartCoroutine(DealDamageToCharacters(selectEnemyTarget,playerAttacking, movePower, attack));
        activeBattleCharacters[0].hasPlayed = true;
        activeBattleCharacters.RemoveAt(0);
        border[currentPlayerIndex].SetActive(false);
        enemyTargetPanel.SetActive(false);
        ClearRings();
        returnButton.SetActive(false);
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
                battleMove.transform.localScale = new Vector3(1, 1);
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
                DamageText("Stunned!",selectEnemyTarget.transform);
                if (activeBattleCharacters.Contains(selectEnemyTarget))
                    activeBattleCharacters.Remove(selectEnemyTarget);
                else
                    characterToStun.Add(selectEnemyTarget);
            }
        }
        else if(battleMovesList[i].moveName == "Tentacles")
        {
            if (characterAttacking.IsPlayer())
            {
                foreach (BattleCharacters enemy in enemies)
                {
                    if (!enemy.isDead)
                    {
                       Instantiate(
                       battleMovesList[i].effectToUse,
                       enemy.transform.position,
                       enemy.transform.rotation);
                    }
                }
            }
            else
            {
                foreach (BattleCharacters player in players)
                {
                    if (!player.isDead)
                    {
                        Instantiate(
                        battleMovesList[i].effectToUse,
                        player.transform.position,
                        player.transform.rotation);
                    }
                }
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

    public void AreaOfEffectAttack()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].HideRing();
            if (!enemies[i].IsPlayer() && enemies[i].isDead)
            {
                enemies.RemoveAt(i);
            }
        }
        PlayerAttack("Tentacles", enemies[0]);
        HideRings();
    }

    public void OpenTargetPanel(string moveName)
    {
        if(moveName == "Tentacles") { return; }
        returnButton.GetComponent<RectTransform>().localPosition = new Vector3(-880f, -238, 0);
        enemyTargetPanel.SetActive(true);
        MoveName?.Invoke(moveName);
        ReturnButtonPlacement();
        returnButton.SetActive(true);
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].IsPlayer() && enemies[i].isDead)
            {
                enemies.RemoveAt(i);
            }
        }
        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (enemies.Count > i)
            {
                targetButtons[i].button.gameObject.SetActive(true);
                targetButtons[i].moveName = moveName;
                targetButtons[i].SetTargetName(enemies[i]);
            }
            else
            {
                /*targetButtons[i].moveName = string.Empty;
                targetButtons[i].targetName.text = string.Empty;*/
                targetButtons[i].activeBattleTarget = null;
                targetButtons[i].button.gameObject.SetActive(false);
            }
        }
        battleMenuState = BattleMenuState.characterAttackPanel;

        CheckPlayerButtonHolder();
    }

    public void OpenMagicPanel()
    {
        for (int i = 0; i < magicButtons.Length; i++)
        {
            magicButtons[i].gameObject.SetActive(false);
        }
        magicPanel.SetActive(true);
        ReturnButtonPlacement();
        returnButton.SetActive(true);
        for (int i = 0; i < battleMovesList.Length; i++)
        {          
            for (int x = 0; x < activeBattleCharacters[0].AttackMovesAvailable().Length; x++) 
            {

                if (battleMovesList[i].moveName == activeBattleCharacters[0].AttackMovesAvailable()[x])
                {
                    magicButtons[x].gameObject.SetActive(true);
                    magicButtons[x].spellCost = battleMovesList[i].manaCost;
                    magicButtons[x].spellCostText.text = magicButtons[x].spellCost.ToString();
                    magicButtons[x].spellName = battleMovesList[i].moveName;
                    magicButtons[x].spellNameText.text = magicButtons[x].spellName;
                    if (activeBattleCharacters[0].currentMana < battleMovesList[i].manaCost)
                    {
                        magicButtons[x].GetComponent<Image>().color = new Color(1, 1, 1, 0.75f);
                    }
                }
            }
        }
        EventSystem.current.SetSelectedGameObject(magicButtons[0].gameObject);
        battleMenuState = BattleMenuState.magicPanel;
        CheckPlayerButtonHolder();
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
                StartCoroutine(FadeAndExitBattle());
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
        CheckPlayerButtonHolder();
    }

    IEnumerator FadeAndExitBattle()
    {
        FadeTo(0,1);
        SwitchActiveMap.instance.SwitchToUI();
        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(1f);
        SettingUpBattle();
        yield return new WaitForSeconds(0.7f);
        MenuManager.instance.FadeOut(1.5f);
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
            ItemButton itemButton = itemSlot.GetComponent<ItemButton>();
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
        if(itemSlotContainerParent.childCount == 0)        
            Utilities.SetSelectedAndHighlight(returnButton, returnButton.GetComponent<Button>());        
    }

    public void SelectedItemToUse(ItemsManager item)
    {
        selectedItem = item;
        itemName.text = item.itemName;

        if (selectedItem.itemType == ItemsManager.ItemType.Weapon)
            itemDescription.text = item.itemDescription;
        else if (selectedItem.itemType == ItemsManager.ItemType.Armor)
            itemDescription.text = item.itemDescription;
        else if (selectedItem.affectType == ItemsManager.AffectType.HP || selectedItem.affectType == ItemsManager.AffectType.Mana)
            itemDescription.text = item.itemDescription + "(Restores " + selectedItem.amountOfEffect + " " + selectedItem.affectType + ")";
        else itemDescription.text = item.itemDescription;
    }

    public void UseItem(int index)
    {
        players[index].UseItemInBattle(selectedItem);
        if (selectedItem.itemName == "Golden Axe")
            players[index].lifestealWeap = true;
        else players[index].lifestealWeap = false;
        if (selectedItem.itemName == "Golden Armor")
            players[index].evasion = 11.5f;
        else players[index].evasion = 5f;
        Inventory.instance.RemoveItem(selectedItem);
        AudioManager.instance.PlaySFX(0);
        UpdateText();
        UseItemEndTurn();
        returnButton.SetActive(false);
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
            /*for (int i = 0; i < targetButton.Length; i++)
            {
                targetButton[i].SetActive(false);
            }*/
            bool solo = players.Count == 1;
            if (solo)
                characterChoicePanel.GetComponent<HorizontalLayoutGroup>().childControlWidth = false;
            else
                characterChoicePanel.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].isDead)
                {
                    PlayerStats activePlayer = GameManager.instance.GetPlayerStats()[i];
                    targetButton[i].SetActive(true);
                    targetName[i].text = activePlayer.playerName;
                    bool activePlayerInHierarchy = activePlayer.gameObject.activeInHierarchy;
                    targetButton[i].transform.parent.gameObject.SetActive(activePlayerInHierarchy);
                }
                else if (players[i].isDead)
                {
                    PlayerStats activePlayer = GameManager.instance.GetPlayerStats()[i];
                    targetButton[i].GetComponent<Button>().interactable = false;
                    targetName[i].text = activePlayer.playerName;
                }
            }
            battleMenuState = BattleMenuState.itemCharacterChoicePanel;
            Utilities.SetSelectedAndHighlight(targetButton[0], targetButton[0].GetComponent<Button>());
        }
    }

    public void OpenItemPanel()
    {
        itemPanel.SetActive(true);
        UpdateItemsInInventory();
        battleMenuState = BattleMenuState.itemPanel;
        ReturnButtonPlacement();
        CheckPlayerButtonHolder();
    }

    private void ReturnButtonPlacement()
    {
        RectTransform rect = UIButtonHolder.GetComponent<RectTransform>();
        switch (battleMenuState)
        {
            case BattleMenuState.magicPanel:
                rect = magicPanel.GetComponent<RectTransform>();
                break;
            case BattleMenuState.itemPanel:
                rect = itemPanel.GetComponent<RectTransform>();
                break;
            case BattleMenuState.characterAttackPanel:
                rect = enemyTargetPanel.GetComponent<RectTransform>();
                break;
        }
        var button = returnButton.GetComponent<RectTransform>();
        button.position = new Vector2 (80, rect.rect.height + 24);
        returnButton.SetActive(true);
    }

    public BattleCharacters[] ReturnPlayerPrefabs()
    {
        return playerPrefabs;
    }

    public int GetAbilityCost(string ability)
    {
        foreach(BattleMoves battleMoves in battleMovesList)
        {
            if(battleMoves.moveName == ability)
            {
                return battleMoves.manaCost;
            }
        }
        return 0;
    }

    //Battle Menu Navigation

    private enum BattleMenuState
    {
        mainPanel,
        characterAttackPanel,
        magicPanel,
        magicAttackCharacterPanel,
        itemPanel,
        itemUsePanel,
        itemCharacterChoicePanel,
    }

    public void ChangeFocusToAttackPanel()
    {
        battleMenuState = BattleMenuState.characterAttackPanel;
        var attackTarget = enemyTargetPanel.GetComponentInChildren<Button>().gameObject;
        EventSystem.current.SetSelectedGameObject(attackTarget);
        enemyTargetPanel.GetComponentInChildren<BattleTargetButtons>().OnSelect(null);
    }
    
    public void ChangeFocusToMagicPanel()
    {
        battleMenuState = BattleMenuState.magicPanel;
        EventSystem.current.SetSelectedGameObject(magicButtons[0].gameObject);
    }
    
    public void ChangeFocusToMagicTargetPanel()
    {
        battleMenuState = BattleMenuState.magicAttackCharacterPanel;
        var attackTarget = enemyTargetPanel.GetComponentInChildren<Button>().gameObject;
        EventSystem.current.SetSelectedGameObject(attackTarget);
        enemyTargetPanel.GetComponentInChildren<BattleTargetButtons>().OnSelect(null);
    }

    public void ChangeFocusToItemsPanel()
    {
        battleMenuState = BattleMenuState.itemPanel;
        if (itemSlotContainerParent.childCount > 0) 
        {
            var firstItemObject = itemSlotContainerParent.GetChild(0).gameObject;
            var firstItemButton = firstItemObject.GetComponent<Button>();
            EventSystem.current.SetSelectedGameObject(firstItemObject);
            firstItemButton.OnSelect(null);
            firstItemButton.onClick.Invoke();
        }
    }

    public void ChangeFocusToItemTargetPanel()
    {
        battleMenuState = BattleMenuState.itemCharacterChoicePanel;
        foreach (Button button in useAndCloseButtons)
        {
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
        }
    }
    private void ChangeFocusToItemsUsePanel()
    {
        battleMenuState = BattleMenuState.itemUsePanel;
        if (itemSlotContainerParent.childCount > 0)
        {
            DisableItemNavigation();
            Utilities.SetSelectedAndHighlight(useAndCloseButtons[0].gameObject, useAndCloseButtons[0]);
        }
    }

    public void NavigateToItemsUsePanel(InputAction.CallbackContext context)
    {
        if (battleMenuState == BattleMenuState.itemPanel && context.performed)
        {
            Invoke("ChangeFocusToItemsUsePanel", 0.1f);
        }
    }

    public void CloseItemPanel()
    {
        itemPanel.SetActive(false);
        battleMenuState = BattleMenuState.mainPanel;
        Utilities.SetSelectedAndHighlight(mainPanelButtons[2], mainPanelButtons[2].GetComponent<Button>());
        CheckPlayerButtonHolder();
    }


    public void DisableItemNavigation()
    {
        foreach (Transform item in itemSlotContainerParent)
        {
            var itemButton = item.GetComponent<Button>();
            var navigation = itemButton.navigation;
            navigation.mode = Navigation.Mode.None;
            itemButton.navigation = navigation;
        }
        foreach (Button button in useAndCloseButtons)
        {
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.Vertical;
            button.navigation = navigation;
        }
    }

    public void UnlockItemToNavigate()
    {
        foreach (Transform item in itemSlotContainerParent)
        {
            var itemButton = item.GetComponent<Button>();
            var navigation = itemButton.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            itemButton.navigation = navigation;
        }
        EventSystem.current.SetSelectedGameObject(activeItemObject);
        foreach (Button button in useAndCloseButtons)
        {
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
        }
    }

    public void NavigateItems(InputAction.CallbackContext context)
    {
        if (context.canceled && battleMenuState == BattleMenuState.itemPanel)
        {
            var button = EventSystem.current.currentSelectedGameObject.GetComponent<ItemButton>();
            if (button)
            {
                button.Press();
                activeItemObject = button.gameObject;
            }
        }
    }

    public void NavigateTargets(InputAction.CallbackContext context)
    {
        if (context.started && enemyTargetPanel.activeInHierarchy)
        {
            HideRings();
        }
        if (context.canceled && enemyTargetPanel.activeInHierarchy)
        {
            ShowRing();
        }
    }

    private void ShowRing()
    {
        var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (button.GetComponentInParent<BattleTargetButtons>())
            button.GetComponentInParent<BattleTargetButtons>().OnSelect(null);
    }

    public void ShowRingsOnAll()
    {
        foreach(BattleCharacters enemy in enemies)
        {
            enemy.ShowRing();
        }
    }

    public void HideAllRings()
    {
        foreach (BattleCharacters enemy in enemies)
        {
            enemy.HideRing();
        }
    }

    private void HideRings()
    {
        foreach (BattleTargetButtons targetButton in targetButtons)
        {
            if (targetButton.gameObject != EventSystem.current.currentSelectedGameObject && targetButton.activeBattleTarget)
                targetButton.OnDeselect(null);
        }
    }

    private void ClearRings()
    {
        foreach(BattleTargetButtons targetButtons in targetButtons)
        {
            if(targetButtons.activeBattleTarget)
                targetButtons.OnDeselect(null);
        }
    }

    private void CheckPlayerButtonHolder()
    {
        if (isBattleActive && waitingForTurn && battleMenuState==BattleMenuState.mainPanel)
        {
            UIButtonHolder.SetActive(true);
            returnButton.SetActive(false);
        }
        else
        {
            UIButtonHolder.SetActive(false);
        }
    }

    public void ReturnToPreviousMenu(InputAction.CallbackContext context)
    {
        if (context.canceled && GameManager.instance.battleIsActive)
        {
            ReturnToPreviousMenu();
        }
    }

    public void ReturnToPreviousMenu()
    {
        if (GameManager.instance.battleIsActive)
        {
            Button button;
            switch (battleMenuState)
            {
                case BattleMenuState.characterAttackPanel:
                    battleMenuState = BattleMenuState.mainPanel;
                    enemyTargetPanel.SetActive(false);
                    MoveName?.Invoke("");
                    ClearRings();
                    button = mainPanelButtons[0].GetComponent<Button>();
                    Utilities.SetSelectedAndHighlight(mainPanelButtons[0], button);
                    break;
                case BattleMenuState.magicPanel:
                    battleMenuState = BattleMenuState.mainPanel;
                    magicPanel.SetActive(false);
                    button = mainPanelButtons[1].GetComponent<Button>();
                    Utilities.SetSelectedAndHighlight(mainPanelButtons[1], button);
                    break;
                case BattleMenuState.magicAttackCharacterPanel:
                    battleMenuState = BattleMenuState.magicPanel;
                    MoveName?.Invoke("");
                    enemyTargetPanel.SetActive(false);
                    ClearRings();
                    OpenMagicPanel();
                    button = abilityChosen.GetComponent<Button>();
                    Utilities.SetSelectedAndHighlight(abilityChosen, button);
                    break;
                case BattleMenuState.itemPanel:
                    battleMenuState = BattleMenuState.mainPanel;
                    itemPanel.SetActive(false);
                    button = mainPanelButtons[2].GetComponent<Button>();
                    Utilities.SetSelectedAndHighlight(mainPanelButtons[2], button);
                    break;
                case BattleMenuState.itemUsePanel:
                    battleMenuState = BattleMenuState.itemPanel;
                    UnlockItemToNavigate();
                    ChangeFocusToItemsPanel();
                    break;
                case BattleMenuState.itemCharacterChoicePanel:
                    battleMenuState = BattleMenuState.itemUsePanel;
                    characterChoicePanel.SetActive(false);
                    ChangeFocusToItemsUsePanel();
                    Utilities.SetSelectedAndHighlight(useAndCloseButtons[0].gameObject, useAndCloseButtons[0]);
                    break;
            }
        }
        CheckPlayerButtonHolder();
    }
}