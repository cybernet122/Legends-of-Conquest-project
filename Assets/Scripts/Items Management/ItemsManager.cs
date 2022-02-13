using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public enum ItemType { Item,Weapon,Armor}
    public ItemType itemType;
    public string itemName, itemDescription;
    public int valueInCoins;
    public Sprite itemImage;
    public enum AffectType { HP,Mana}
    public int amountOfEffect;
    public AffectType affectType;
    public int weaponDexterity, armorDefense;
    bool inRange = false;
    public bool isStackable;
    public int amount;
    public bool rememberPickup;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("PickedUpItem_" + name))
        {
            if(PlayerPrefs.GetInt("PickedUpItem_" + name) == 1)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            Inventory.instance.AddItems(this,false);
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            AudioManager.instance.PlaySFX(6);
            inRange = true;
            GameManager.instance.DataToSave("PickedUpItem_" + name);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
        }        
    }

    public void UseItem(int characterToUse)
    {
        PlayerStats selectedCharacter = GameManager.instance.GetPlayerStats()[characterToUse];
        if (itemType == ItemType.Item)
        {
            if (affectType == AffectType.HP)
            {
                selectedCharacter.AddHP(amountOfEffect);
            }
            else if (affectType == AffectType.Mana)
            {
                selectedCharacter.AddMana(amountOfEffect);
            }
        }
        else if (itemType == ItemType.Weapon)
        {
            if (selectedCharacter.equippedWeaponName != this.itemName)
            {
                if (selectedCharacter.equippedWeaponName != "")
                {
                    Inventory.instance.AddItems(selectedCharacter.equippedWeapon, false);
                }
                selectedCharacter.EquipWeapon(this);
            }
        }
        else if (itemType == ItemType.Armor)
        {
            if (selectedCharacter.equippedArmorName != this.itemName)
            {
                if (selectedCharacter.equippedArmorName != "")
                {
                    Inventory.instance.AddItems(selectedCharacter.equippedArmor, false);
                }
                selectedCharacter.EquipArmor(this);
            }
        }
    }
}
