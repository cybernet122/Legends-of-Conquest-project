using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool enableJoystick;
    public static GameManager instance;
    [SerializeField] PlayerStats[] playerStats;
    public bool gameMenuOpened, dialogBoxOpened, shopMenuOpened, battleIsActive, count;
    public int currentGoldCoins;
    public GameObject joystickControls;
    [SerializeField]SavingFade savingFade;
    [SerializeField] GameObject player;
    string[] playerAssets;
    List<string> stringsToSave = new List<string>();
    public bool newGame, enableMovement,continueGame = false;
    [SerializeField]PlayerInput playerInput;
    public bool loading =false;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)        
            Destroy(this.gameObject);        
        else        
            instance = this;        
        DontDestroyOnLoad(gameObject);
        if(!savingFade)
        savingFade = FindObjectOfType<SavingFade>();
        count = true;
        if (enableJoystick)
            EnableButtons();
        else
            DisableButtons();
        LeanTween.delayedCall(0.1f, () =>
        {
            SortPlayerStats();
            UpdatePlayerLevels();
        });        
    }

    void Update()
    {
        PurgeData();
        bool condition = SceneManager.GetActiveScene().name == "Main Menu" || SceneManager.GetActiveScene().name == "Loading Scene" || SceneManager.GetActiveScene().name == "GameOverScene" || SceneManager.GetActiveScene().name == "Treasure" || SceneManager.GetActiveScene().name == "Options";
        if ((loading || dialogBoxOpened || gameMenuOpened || shopMenuOpened || battleIsActive || condition))
        {
            if (enableMovement != false)
                enableMovement = false;
                DisableButtons();            
        }
        else
        {
            if (enableMovement != true)
                enableMovement = true;
                EnableButtons();
        }
    }

    private void EnableButtons()
    {
        if (enableJoystick && joystickControls && !joystickControls.GetComponentInChildren<Image>().enabled)
        {
            for (int i = 0; i < joystickControls.transform.childCount; i++)
                joystickControls.transform.GetChild(i).GetComponent<Image>().enabled = true;
        }
    }

    private void DisableButtons()
    {
        if (joystickControls && joystickControls.GetComponentInChildren<Image>().enabled)
        { 
            for (int i = 0; i < joystickControls.transform.childCount; i++)        
                joystickControls.transform.GetChild(i).GetComponent<Image>().enabled = false;
        }
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    public PlayerStats[] GetPlayerStats()
    {
        SortPlayerStats();
        return playerStats;
    }

    public void UpdatePlayerStats()
    {
        SortPlayerStats();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneWasSwitched;
    }

    private void FadeOut()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu" && SceneManager.GetActiveScene().name != "Loading Scene")        
            MenuManager.instance.FadeOut(.5f);        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneWasSwitched;
        FadeOut();
    }

    private void Awake()
    {
        Invoke("CheckForBattleManager", 0.15f);        
    }

    public void DestroyPlayer()
    {
        StartCoroutine(CreatePlayerAndSetBorders());
    }

    private IEnumerator CreatePlayerAndSetBorders()
    {
        Destroy(Player.instance.gameObject);
        yield return new WaitForSeconds(0.1f);
        var playerObject = Instantiate(player, SwitchActiveMap.instance.transform);
        playerObject.GetComponent<Player>().playerInput = SwitchActiveMap.instance.GetComponent<PlayerInput>();
        yield return new WaitForSeconds(0.1f);
        FindObjectOfType<LevelManager>().SetLimit();
    }

    private void OnSceneWasSwitched(Scene scene, LoadSceneMode mode)
    {
        savingFade = FindObjectOfType<SavingFade>();
        Invoke("SortPlayerStats", 0.3f);
        Invoke("UpdatePlayerLevels", 0.5f);
        if(scene.name != "Main Menu" && scene.name != "Loading Scene" && scene.name != "GameOverScene")
        Invoke("LoadSecondaryData",0.5f);
        LeanTween.delayedCall(0.15f, () =>
        {
            QuestManager.instance.MountainsQuest();
            savingFade = SavingFade.instance;
            if (scene.name != "Main Menu" && scene.name != "Loading Scene")
                MenuManager.instance.SetFirstSelectedObject(0);
            DialogController.instance.ReturnFromMountains();
            Player.UpdatePotency();
        });
        if(SceneManager.GetActiveScene().name == "Bedroom" && newGame)
        {
            LeanTween.delayedCall(0.45f, () =>
            {
                if (newGame || continueGame)
                {
                    var spawnPoint = FindObjectOfType<SpawnPoint>();
                    Player.instance.transform.position = spawnPoint.gameObject.transform.position;
                    newGame = false;
                    continueGame = false;
                }
            });
        }
    }

    public void TeleportScroll()
    {
        MenuManager.instance.ToggleMenu();
        MenuManager.instance.FadeImage();
        LeanTween.delayedCall(1f, () => {
            SaveSecondaryData();
            SceneManager.LoadScene("Town");
            LeanTween.delayedCall(0.3f, () =>
            {
                var tpPoint = FindObjectsOfType<AreaEnter>();
                foreach (AreaEnter point in tpPoint)
                {
                    if (point.transitionAreaName == "Town 1")
                        TeleportToPoint(point);
                    MenuManager.instance.ResetToggles();
                    Player.instance.FaceDown();
                }
            });
        });
    }

    private void TeleportToPoint(AreaEnter point)
    {
        Player.instance.transform.position = point.transform.position;
    }

    private void CheckForBattleManager()
    {
        if (FindObjectOfType<BattleManager>())
            BattleManager.instance.CheckForResize();
    }

    private void SortPlayerStats()
    {
        FillPlayerStats();
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].GetComponent<Player>())
            {
                var playerstat = playerStats[0];
                playerStats[0] = playerStats[i];
                playerStats[i] = playerstat;
                playerStats[0].playerName = PlayerPrefs.GetString("Players_name_");
            }
        }
        if (QuestManager.instance.CheckIfComplete("Look for the heroes located in the cave and join them"))
            return;
        else
        {
            if (Utilities.ReturnSceneName() != "GameOverScene" && Utilities.ReturnSceneName() != "Loading Scene" && Utilities.ReturnSceneName() != "Main Menu")
            {
                var playerStats1 = playerStats[0];
                playerStats = new PlayerStats[1];
                playerStats[0] = playerStats1;
            }
        }
    }

    private void FillPlayerStats()
    {
        playerStats = new PlayerStats[0];
        playerStats = FindObjectsOfType<PlayerStats>();
    }

    public void EmptyPlayerStats()
    {
        var playerStats1 = playerStats[0];
        playerStats = new PlayerStats[1];
        playerStats[0] = playerStats1;
    }

    public void SaveData()
    {
        savingFade.PlaySaveAnimation();
        SavePlayerPos();
        QuestManager.instance.SaveQuestData();
        SaveSecondaryData();
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt("Gold_coins_", currentGoldCoins);
    }

    public void SaveSecondaryData()
    {
        foreach (string data in stringsToSave)
            PlayerPrefs.SetInt(data, 1);
        SavePlayerStats();
        SaveItemInventory();
    }

    private static void SaveItemInventory()
    {
        PlayerPrefs.SetInt("Number_Of_Items", Inventory.instance.GetItemList().Count);
        for (int i = 0; i < Inventory.instance.GetItemList().Count; i++)
        {
            ItemsManager itemInInventory = Inventory.instance.GetItemList()[i];
            PlayerPrefs.SetString("Item_" + i + "_Name_", itemInInventory.itemName);
            if (itemInInventory.isStackable && itemInInventory.amount > 1)
                PlayerPrefs.SetInt("Items_" + i + "_Amount_", itemInInventory.amount);
            else
                PlayerPrefs.SetInt("Items_" + i + "_Amount_", 1);
        }
    }

    public static void SavePlayerPos()
    {
        PlayerPrefs.SetFloat("Player_Pos_X", Player.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Pos_Y", Player.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Pos_Z", Player.instance.transform.position.z);
    }

    private void SavePlayerStats()
    {
        for (int i = 0; i < playerStats.Length; i++)
        {
            /*if (playerStats[i].gameObject.activeInHierarchy)
                PlayerPrefs.SetInt("Player_" + playerStats[i].name + "_active", 1);
            else
                PlayerPrefs.SetInt("Player_" + playerStats[i].name + "_active", 0);*/
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentXP", playerStats[i].currentXP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentHP", playerStats[i].currentHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentMana", playerStats[i].currentMana);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_MaxMana", playerStats[i].maxMana);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Dexterity", playerStats[i].dexterity);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Defense", playerStats[i].defense);
            SaveEquippedItems(i);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_WeaponPower", playerStats[i].weaponPower);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_ArmorDefense", playerStats[i].armorDefense);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Speed", playerStats[i].turnSpeed);
            PlayerPrefs.SetFloat("Player_" + playerStats[i].playerName + "_Evasion", playerStats[i].evasion);
            if (playerStats[i].lifestealWeap)
                PlayerPrefs.SetInt("Player_lifesteal_", 1);
            else
                PlayerPrefs.SetInt("Player_lifesteal_", 0);
        }
    }

    private void SaveEquippedItems(int i)
    {
        PlayerPrefs.SetString("Player_" + playerStats[i].playerName + "_EquippedWeapon", playerStats[i].equippedWeaponName);
        PlayerPrefs.SetString("Player_" + playerStats[i].playerName + "_EquippedArmor", playerStats[i].equippedArmorName);
    }

    private void LoadEquippedItems(int i)
    {
        playerStats[i].equippedWeaponName = PlayerPrefs.GetString("Player_" + playerStats[i].playerName + "_EquippedWeapon");
        playerStats[i].equippedWeapon = ItemsAssets.instance.GetItemsAsset(playerStats[i].equippedWeaponName);
        playerStats[i].equippedArmorName = PlayerPrefs.GetString("Player_" + playerStats[i].playerName + "_EquippedArmor");
        playerStats[i].equippedArmor = ItemsAssets.instance.GetItemsAsset(playerStats[i].equippedArmorName);
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("Player_Pos_X"))
        {
            LoadPlayerPos();
            QuestManager.instance.LoadQuestData();
            Invoke("LoadPlayerStats",0.4f);
            LoadItemInventory();
            currentGoldCoins = PlayerPrefs.GetInt("Gold_coins_");
        }
    }

    private static void LoadItemInventory()
    {
        Inventory.instance.PurgeInventoryForLoad();
        if (PlayerPrefs.HasKey("Number_Of_Items"))
        {
            for (int i = 0; i < PlayerPrefs.GetInt("Number_Of_Items"); i++)
            {
                string itemString = PlayerPrefs.GetString("Item_" + i + "_Name_");
                ItemsManager itemToAdd = ItemsAssets.instance.GetItemsAsset(itemString);
                int itemAmount = 0;
                if (PlayerPrefs.HasKey("Items_" + i + "_Amount_"))
                {
                    itemAmount = PlayerPrefs.GetInt("Items_" + i + "_Amount_");
                }
                Inventory.instance.AddItems(itemToAdd, false);
                if (itemToAdd.isStackable && itemAmount > 1)
                {
                    itemToAdd.amount = itemAmount;
                }
            }
        }
    }

    private void LoadPlayerStats()
    {
        for (int i = 0; i < playerStats.Length; i++)
        {
            /*if (PlayerPrefs.GetInt("Player_" + playerStats[i].name + "_active") == 0)
                playerStats[i].gameObject.SetActive(false);
            else
                playerStats[i].gameObject.SetActive(true);*/
            playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Level");
            playerStats[i].currentXP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentXP");
            playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentHP");
            playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_MaxHP");
            playerStats[i].currentMana = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentMana");
            playerStats[i].maxMana = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_MaxMana");
            playerStats[i].dexterity = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Dexterity");
            playerStats[i].defense = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Defense");
            LoadEquippedItems(i);
            playerStats[i].weaponPower = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_WeaponPower");
            playerStats[i].armorDefense = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_ArmorDefense");
            playerStats[i].turnSpeed = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Speed");
            playerStats[i].evasion = PlayerPrefs.GetFloat("Player_" + playerStats[i].playerName + "_Evasion");
            if (PlayerPrefs.GetInt("Player_lifesteal_") == 1)
                playerStats[i].lifestealWeap = true;
            else
                playerStats[i].lifestealWeap = false;
        }
    }

    private static void LoadPlayerPos()
    {
        Player.instance.transform.position = new Vector3(
            PlayerPrefs.GetFloat("Player_Pos_X"),
            PlayerPrefs.GetFloat("Player_Pos_Y"),
            PlayerPrefs.GetFloat("Player_Pos_Z"));
    }

    private void PurgeData()
    {
        /*if (Input.GetKeyDown(KeyCode.K) && SceneManager.GetActiveScene().name != "Main Menu")
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Purging Data");
            QuestManager.instance.PurgeQuestData();
        }*/
    }

    public void UpdatePlayerLevels()
    {
        bool condition = QuestManager.instance.CheckIfComplete("Look for the heroes located in the cave and join them");
        if (condition)
        {
            foreach (PlayerStats player in playerStats)
            {
                player.MatchPlayerLevel();
            }
        }
    }

    public void GameOver()
    {
        StartCoroutine(LoadGameOverScene());
    }

    IEnumerator LoadGameOverScene()
    {
        SwitchActiveMap.instance.SwitchToUI();
        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameOverScene");    
    }

    public void DataToSave(string stringToSave)
    {
        stringsToSave.Add(stringToSave);
    }
    
    public void LoadSecondaryData()
    {
        if (PlayerPrefs.HasKey("Player_Pos_X"))
        {
            foreach (string data in stringsToSave)
                PlayerPrefs.GetInt(data, 1);
            stringsToSave.Clear();
            if (PlayerPrefs.HasKey("Player_" + playerStats[0].playerName + "_CurrentHP"))            
                LoadPlayerStats();            
            LoadItemInventory();                
        }
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        if (enableMovement)
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
            print(input.x + " " + input.y);
            Player.instance.MovePlayer(input.x, input.y);
        }
    }
}
