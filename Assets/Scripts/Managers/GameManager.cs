using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] PlayerStats[] playerStats;
    public bool gameMenuOpened, dialogBoxOpened, shopMenuOpened, battleIsActive, count;
    public int currentGoldCoins;
    [SerializeField]SavingFade savingFade;
    string[] playerAssets;
    List<string> stringsToSave = new List<string>();
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
        savingFade = FindObjectOfType<SavingFade>();
        count = true;
        SortPlayerStats();
        UpdatePlayerLevels();
    }

    void Update()
    {
        // Old battle Navigation
        /*if (battleIsActive)
        {
            Player.instance.enableMovement = false;
            if (count)
            {
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                {
                    BattleManager.instance.ButtonNavigation((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));
                }
            }
        }*/
        PurgeData();
        if (dialogBoxOpened || gameMenuOpened || shopMenuOpened || battleIsActive)
        {
            Player.instance.enableMovement = false;
        }
        else
        {
            Player.instance.enableMovement = true;
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveData();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            LoadData();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            QuestManager.instance.PurgeQuestData();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            UpdatePlayerLevels();
        }
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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneWasSwitched;
    }

    private void Awake()
    {
        Invoke("CheckForBattleManager", 0.15f);
        
    }

    private void OnSceneWasSwitched(Scene scene, LoadSceneMode mode)
    {
        savingFade = FindObjectOfType<SavingFade>();
        Invoke("SortPlayerStats", 0.1f);
        Invoke("UpdatePlayerLevels", 0.5f);
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
            var playerStats1 = playerStats[0];
            playerStats = new PlayerStats[1];
            playerStats[0] = playerStats1;
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
        SavePlayerStats();
        SaveItemInventory();
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        
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

    private static void SavePlayerPos()
    {
        PlayerPrefs.SetFloat("Player_Pos_X", Player.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Pos_Y", Player.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Pos_Z", Player.instance.transform.position.z);
    }

    private void SavePlayerStats()
    {
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
                PlayerPrefs.SetInt("Player_" + playerStats[i].name + "_active", 1);
            else
                PlayerPrefs.SetInt("Player_" + playerStats[i].name + "_active", 0);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentXP", playerStats[i].currentXP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentHP", playerStats[i].currentHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_CurrentMana", playerStats[i].currentMana);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_MaxMana", playerStats[i].maxMana);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Dexterity", playerStats[i].dexterity);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Defense", playerStats[i].defense);
            PlayerPrefs.SetString("Player_" + playerStats[i].playerName + "_EquippedWeapon", playerStats[i].equippedWeaponName);
            PlayerPrefs.SetString("Player_" + playerStats[i].playerName + "_EquippedArmor", playerStats[i].equippedArmorName);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_WeaponPower", playerStats[i].weaponPower);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_ArmorDefense", playerStats[i].armorDefense);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Speed", playerStats[i].turnSpeed);
            PlayerPrefs.SetFloat("Player_" + playerStats[i].playerName + "_Evasion", playerStats[i].evasion);
            if (playerStats[i].lifestealWeap)
                PlayerPrefs.SetInt("Player_lifesteal_", 1);
            else
                PlayerPrefs.SetInt("Player_lifesteal_", 0);
        }
        PlayerPrefs.SetInt("Gold_Coins_", currentGoldCoins);
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("Player_Pos_X"))
        {
            LoadPlayerPos();
            QuestManager.instance.LoadQuestData();
            LoadPlayerStats();
            LoadItemInventory();
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
            if (PlayerPrefs.GetInt("Player_" + playerStats[i].name + "_active") == 0)
                playerStats[i].gameObject.SetActive(false);
            else
                playerStats[i].gameObject.SetActive(true);
            playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Level");
            playerStats[i].currentXP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentXP");
            playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentHP");
            playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_MaxHP");
            playerStats[i].currentMana = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_CurrentMana");
            playerStats[i].maxMana = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_MaxMana");
            playerStats[i].dexterity = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Dexterity");
            playerStats[i].defense = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Defense");
            playerStats[i].equippedWeaponName = PlayerPrefs.GetString("Player_" + playerStats[i].playerName + "_EquippedWeapon");
            playerStats[i].equippedWeapon = ItemsAssets.instance.GetItemsAsset(playerStats[i].equippedWeaponName);
            playerStats[i].equippedArmorName = PlayerPrefs.GetString("Player_" + playerStats[i].playerName + "_EquippedArmor");
            playerStats[i].equippedArmor = ItemsAssets.instance.GetItemsAsset(playerStats[i].equippedArmorName);
            playerStats[i].weaponPower = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_WeaponPower");
            playerStats[i].armorDefense = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_ArmorDefense");
            playerStats[i].turnSpeed = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Speed");
            playerStats[i].evasion = PlayerPrefs.GetFloat("Player_" + playerStats[i].playerName + "_Evasion");
            if (PlayerPrefs.GetInt("Player_lifesteal_") == 1)
                playerStats[i].lifestealWeap = true;
            else
                playerStats[i].lifestealWeap = false;
        }
        GameManager.instance.currentGoldCoins = PlayerPrefs.GetInt("Gold_Coins_");
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Purging Data");
            QuestManager.instance.PurgeQuestData();
        }
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

/*    public string ReturnScene()
    {
        return SceneManager.GetActiveScene().name;
    }*/

    public void GameOver()
    {
        StartCoroutine(LoadGameOverScene());
    }

    IEnumerator LoadGameOverScene()
    {
        MenuManager.instance.FadeImage();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameOverScene");    
    }

    public void DataToSave(string stringToSave)
    {
        stringsToSave.Add(stringToSave);
    }

    public void SaveSecondaryData()
    {
        foreach (string data in stringsToSave)
        {
            PlayerPrefs.SetInt(data, 1);
        }
        stringsToSave.Clear();
        SavePlayerStats();
        SaveItemInventory();
    }
}
