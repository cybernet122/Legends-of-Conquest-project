using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] string playerName;

    [SerializeField] int maxLevel = 50;
    [SerializeField] int playerLevel = 1;
    [SerializeField] int currentXP;
    [SerializeField] int[] xpForNextLevel;
    [SerializeField] int baseLevelXP = 100;

    [SerializeField] int maxHP = 100;
    [SerializeField] int currentHP;

    [SerializeField] int maxMana = 30;
    [SerializeField] int currentMana;

    [SerializeField] int dexterity;
    [SerializeField] int defence;
    // Start is called before the first frame update
    void Start()
    {
        xpForNextLevel = new int[maxLevel];
        xpForNextLevel[1] = baseLevelXP;
        for(int i=2;i< xpForNextLevel.Length;i++)
        {
            xpForNextLevel[i] = (int)(0.02f * i * i * i + 3.06f * i * i + 105.6f * i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddXP(400);
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (playerLevel >= maxLevel) { return; }
        if (currentXP > xpForNextLevel[playerLevel])
        {
            currentXP -= xpForNextLevel[playerLevel];
            playerLevel++;
            if(playerLevel % 2 == 0)
            {
                dexterity++;
            }
            else
            {
                defence++;
            }
            maxHP = Mathf.RoundToInt(maxHP * 1.11f);
            currentHP = maxHP;
            maxMana = Mathf.RoundToInt(maxMana * 1.08f);
            currentMana = maxMana;
        }
        
    }
}
