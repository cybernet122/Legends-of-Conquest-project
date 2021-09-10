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
    public int weaponDexterity, armorDefence;
    bool inRange = false;
    public bool isStackable;
    public int amount;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            Inventory.instance.AddItems(this);
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            inRange = true;
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
            if (selectedCharacter.equippedWeaponName != "")
            {
                Inventory.instance.AddItems(selectedCharacter.equippedWeapon);
            }
            selectedCharacter.EquipWeapon(this);
        }
        else if (itemType == ItemType.Armor)
        {
            if (selectedCharacter.equippedArmorName != "")
            {
                Inventory.instance.AddItems(selectedCharacter.equippedArmor);
            }
            selectedCharacter.EquipArmor(this);
        }
    }
}
