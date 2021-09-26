using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] PlayerStats[] playerStats;
    public bool gameMenuOpened, dialogBoxOpened, shopMenuOpened, battleIsActive;
    public int currentGoldCoins;

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
        playerStats = FindObjectsOfType<PlayerStats>();
        for(int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].GetComponent<Player>())
            {
                var playerstat = playerStats[0];
                playerStats[0] = playerStats[i];
                playerStats[i] = playerstat;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("Saving Data");
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            LoadData();
            Debug.Log("Loading Data");
        }
    }

    public PlayerStats[] GetPlayerStats()
    {
        return playerStats;
    }

    public void SaveData()
    {
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
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_Defence", playerStats[i].defence);
            PlayerPrefs.SetString("Player_" + playerStats[i].playerName + "_EquippedWeapon", playerStats[i].equippedWeaponName);
            PlayerPrefs.SetString("Player_" + playerStats[i].playerName + "_EquippedArmor", playerStats[i].equippedArmorName);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_WeaponPower", playerStats[i].weaponPower);
            PlayerPrefs.SetInt("Player_" + playerStats[i].playerName + "_ArmorDefence", playerStats[i].armorDefence);
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
                if(itemToAdd.isStackable && itemAmount > 1)
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
            playerStats[i].defence = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_Defence");
            playerStats[i].equippedWeaponName = PlayerPrefs.GetString("Player_" + playerStats[i].playerName + "_EquippedWeapon");
            playerStats[i].equippedArmorName = PlayerPrefs.GetString("Player_" + playerStats[i].playerName + "_EquippedArmor");
            playerStats[i].weaponPower = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_WeaponPower");
            playerStats[i].armorDefence = PlayerPrefs.GetInt("Player_" + playerStats[i].playerName + "_ArmorDefence");
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
        }
    }
}
