using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public enum ItemType { Item, Weapon, Armor}
    public ItemType itemType;
    public string itemName, itemDescription;
    public int valueInCoins;
    public Sprite itemImage;
    public enum AffectType { HP, Mana, Other}
    public int amountOfEffect;
    public AffectType affectType;
    public int weaponDexterity, armorDefense;
    bool inRange = false;
    public bool isStackable;
    public int amount;
    public bool rememberPickup;
    SpriteRenderer image;
    CircleCollider2D collider2d;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<CircleCollider2D>();
        if (PlayerPrefs.HasKey("PickedUpItem_" + name))
        {
            if(PlayerPrefs.GetInt("PickedUpItem_" + name) == 1)
            {
                //gameObject.SetActive(false);
                image.enabled = false;
                collider2d.enabled = false;
            }
        }
        IncreasePotionPotency();
    }

    private void OnEnable()
    {
        Player.IncreaseHealingPotency += IncreasePotionPotency;
    }

    private void OnDisable()
    {
        Player.IncreaseHealingPotency -= IncreasePotionPotency;
    }

    public void IncreasePotionPotency()
    {
        if(itemName == "HP Potion")
        {
            var player = Player.instance.ReturnPlayerStats();
            amountOfEffect = 60 + (player.playerLevel * 5);
        }
        if (itemName == "Mana Potion")
        {
            var player = Player.instance.ReturnPlayerStats();
            amountOfEffect = 40 + (player.playerLevel * 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            Inventory.instance.AddItems(this,false);
            //gameObject.SetActive(false);
            image.enabled = false;
            collider2d.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            AudioManager.instance.PlaySFX(6);
            inRange = true;
            string stringToSave = "PickedUpItem_" + name;
            GameManager.instance.DataToSave(stringToSave);
            MenuManager.instance.ShowInfoText("Picked up item " + itemName);
            LeanTween.delayedCall(1.5f, () => { MenuManager.instance.HideInfoText(); });
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            inRange = false;
    }

    public void UseItem(int characterToUse)
    {
        PlayerStats selectedCharacter = GameManager.instance.GetPlayerStats()[characterToUse];
        if (itemType == ItemType.Item)
        {
            if (affectType == AffectType.HP)
                selectedCharacter.AddHP(amountOfEffect);
            else if (affectType == AffectType.Mana)
                selectedCharacter.AddMana(amountOfEffect);
            else if (affectType == AffectType.Other)
                GameManager.instance.TeleportScroll();
        }
        else if (itemType == ItemType.Weapon)
        {
            if (selectedCharacter.equippedWeaponName != this.itemName)
            {
                if (selectedCharacter.equippedWeaponName != "")
                    Inventory.instance.AddItems(selectedCharacter.equippedWeapon, false);
                selectedCharacter.EquipWeapon(this);
            }
        }
        else if (itemType == ItemType.Armor)
        {
            if (selectedCharacter.equippedArmorName != this.itemName)
            {
                if (selectedCharacter.equippedArmorName != "")
                    Inventory.instance.AddItems(selectedCharacter.equippedArmor, false);
                selectedCharacter.EquipArmor(this);
            }
        }
    }
}
