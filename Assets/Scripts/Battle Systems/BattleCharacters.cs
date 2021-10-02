using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacters : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] string[] attacksAvailable;
    [SerializeField] GameObject facade;
    public string characterName;
    public int currentHP, maxHP, currentMana, maxMana, dexterity, defence, weaponPower, armorDefence, speed;
    public bool isDead;
    public bool hasPlayed;
    bool takeDamage = false;
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
        StartCoroutine(PlayDamageRecievedAnimation());
    }

    IEnumerator PlayDamageRecievedAnimation()
    {
        if (currentHP <= 0)
        {
            StopCoroutine(PlayDamageRecievedAnimation());
            print(characterName + " has died.");
            yield return new WaitForSeconds(1.5f);
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            currentHP = 0;
            isDead = true;
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
