using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacters : MonoBehaviour
{
    [SerializeField] bool isPlayer;
    [SerializeField] string[] attacksAvailable;

    public string characterName;
    public int currentHP, maxHP, currentMana, maxMana, dexterity, defence, weaponPower, armorDefence, speed;
    public bool isDead;
    public bool hasPlayed;

    public bool IsPlayer()
    {
        return isPlayer;
    }
}
