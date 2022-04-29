using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharacters : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] string[] attacksAvailable;
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI nameText,hpValue;
    [SerializeField] GameObject selectionRing;
    public string characterName;
    public int currentHP, maxHP, currentMana, maxMana, dexterity, defense, weaponPower, armorDefense, speed;
    public float evasion;
    public bool lifestealWeap;
    public bool isDead;
    public bool hasPlayed;
    bool fadeOut = false;
    private void Start()
    {
        UpdateBattleStats();
    }

    public bool IsPlayer()
    {
        return isPlayer;
    }

    public string[] AttackMovesAvailable()
    {
        return attacksAvailable;
    }

    private void Update()
    {
        if (fadeOut)
        {
            GetComponent<SpriteRenderer>().color = new Color(
                    Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.r, 1f, 0.3f * Time.deltaTime),
                    Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.g, 0f, 0.3f * Time.deltaTime),
                    Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.b, 0f, 0.3f * Time.deltaTime),
                    Mathf.MoveTowards(GetComponent<SpriteRenderer>().color.a, 0f, 0.3f * Time.deltaTime)
                    );
            if (GetComponent<SpriteRenderer>().color.a == 0)
            {
                fadeOut = false;
            }
        }
    }
    public void TakeHpDamage(int damageToRecieve)
    {
        currentHP -= damageToRecieve;
        BattleManager.instance.UpdateText();
        UpdateBattleStats();
        if(damageToRecieve>0)
            StartCoroutine(PlayDamageRecievedAnimation());
    }

    private void UpdateBattleStats()
    {
        if (!isPlayer)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
            hpValue.text = Mathf.Clamp(currentHP,0,maxHP)+ " / " + maxHP;
            nameText.text = characterName;
        }
    }

    IEnumerator PlayDamageRecievedAnimation()
    {
        if (currentHP <= 0)
        {
            isDead = true;
            if(!isPlayer)
                hpSlider.value = 0;
            StopCoroutine(PlayDamageRecievedAnimation());
            print(characterName + " has died.");
            fadeOut = true;
            yield return new WaitForSeconds(1.5f);
            currentHP = 0;
            if (!isPlayer)
            {
                hpSlider.gameObject.SetActive(false);
                nameText.gameObject.SetActive(false);
            }
            StopCoroutine(PlayDamageRecievedAnimation());
        }
        yield return new WaitForSeconds(1f);
        if (!isDead)
        {
            for (int i = 0; i < 7; i++)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                yield return new WaitForSeconds(0.08f);
                GetComponent<SpriteRenderer>().enabled = true;
                yield return new WaitForSeconds(0.08f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void UseItemInBattle(ItemsManager item)
    {
        if (item.itemType == ItemsManager.ItemType.Item)
        {
            if (item.affectType == ItemsManager.AffectType.HP)
                AddHP(item.amountOfEffect);
            else if (item.affectType == ItemsManager.AffectType.Mana)
                AddMana(item.amountOfEffect);
        }
        else if(item.itemType != ItemsManager.ItemType.Item)
        {
            int type = 0;
            if (item.itemType == ItemsManager.ItemType.Weapon)
                weaponPower = item.weaponDexterity;
            else if (item.itemType == ItemsManager.ItemType.Armor)
            {
                armorDefense = item.armorDefense;
                type = 1;
            }
            var playerStats = GameManager.instance.GetPlayerStats();
            for (int i = 0; i < playerStats.Length; i++)
            {
                if(playerStats[i].playerName == characterName)
                {
                    if(type == 0)
                    {
                        playerStats[i].equippedWeapon = item;
                        playerStats[i].weaponPower = item.weaponDexterity;
                        playerStats[i].equippedWeaponName = item.itemName;
                    }
                    else if(type == 1)
                    {
                        playerStats[i].equippedArmor = item;
                        playerStats[i].armorDefense = item.armorDefense;
                        playerStats[i].equippedArmorName = item.itemName;
                    }
                }
            }
        }
    }

    public void AddHP(int amountOfEffect)
    {
        currentHP += amountOfEffect;
        if (currentHP > maxHP)
            currentHP = maxHP;
        UpdateBattleStats();
    }
    private void AddMana(int amountOfEffect)
    {
        currentMana += amountOfEffect;
        if (currentMana > maxMana)
            currentMana = maxMana;
    }

    public void ExitBattle(int index)
    {
        PlayerStats[] playerStats = GameManager.instance.GetPlayerStats();
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (index == 0) 
            {
                if (playerStats[i].playerName == characterName)
                {
                    playerStats[i].currentHP = currentHP;
                    playerStats[i].currentMana = currentMana;
                }
            }
            else if(index == 1)
            {
                if (playerStats[i].playerName == characterName)
                {
                    playerStats[i].currentHP = (int)(maxHP * 0.35f);
                    playerStats[i].currentMana = (int)(maxMana * 0.35f);
                }
            }
        }
    }

    public void ShowRing()
    { 
        selectionRing.SetActive(true);
    }

    public void HideRing()
    {
        selectionRing.SetActive(false);
    }

}
