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
    public string characterName;
    public int currentHP, maxHP, currentMana, maxMana, dexterity, defence, weaponPower, armorDefence, speed;
    public bool isDead;
    public bool hasPlayed;

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

    }
    public void TakeHpDamage(int damageToRecieve)
    {
        currentHP -= damageToRecieve;
        BattleManager.instance.UpdateText();
        UpdateBattleStats();
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
            yield return new WaitForSeconds(1.5f);
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            currentHP = 0;
            if (!isPlayer)
            {
                hpSlider.gameObject.SetActive(false);
                nameText.gameObject.SetActive(false);
            }
            StopCoroutine(PlayDamageRecievedAnimation());
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 7; i++)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.08f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.08f);
        }
        //StopCoroutine(PlayDamageRecievedAnimation());
        yield return new WaitForSeconds(1f);
    }
}
