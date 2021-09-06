using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    public string playerName;

    public Sprite characterImage;

    public int maxLevel = 50;
    public int playerLevel = 1;
    public int currentXP;
    public int[] xpForNextLevel;
    [SerializeField] int baseLevelXP = 100;

    public int maxHP = 100;
    public int currentHP;

    public int maxMana = 30;
    public int currentMana;
    [SerializeField] int addXP;
    public int dexterity;
    public int defence;
    int levelsToAdd = 0;

    void Start()
    {
        xpForNextLevel = new int[maxLevel];
        xpForNextLevel[1] = baseLevelXP;
        for(int i=2;i< xpForNextLevel.Length;i++)
        {
            xpForNextLevel[i] = (int)(0.02f * i * i * i + 3.06f * i * i + 105.6f * i);
        }
        currentHP = maxHP;
        currentMana = maxMana;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddXP(addXP);
        }
    }

    public void AddXP(int amount)
    {
        if (playerLevel >= maxLevel) { return; }
        int SumXPToAdd = currentXP + amount;
        levelsToAdd = 0;
        if (SumXPToAdd >= xpForNextLevel[playerLevel]*2)
        {
            for (int i = 1; i < 50 && SumXPToAdd >= xpForNextLevel[playerLevel + i]; i++)
            {
                print(i);
                SumXPToAdd -= xpForNextLevel[playerLevel + i-1];
                print(SumXPToAdd);
                levelsToAdd = i;
            }
            //playerLevel += levelsToAdd;
            currentXP = SumXPToAdd;
        }
        else
        {
            print("Checking for levelup");
            currentXP += amount;
        }
        MenuManager.instance.UpdateStats();
        CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        if(playerLevel >= maxLevel) { return; }
        if (levelsToAdd >= 1 && levelsToAdd <= (maxLevel - playerLevel))
        {
            for (int i = 0; i < levelsToAdd; i++)
            {
                playerLevel++;
                if (playerLevel % 2 == 0)
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
        if(currentXP >= xpForNextLevel[playerLevel])
        {
            currentXP -= xpForNextLevel[playerLevel];
            playerLevel++;
            if (playerLevel % 2 == 0)
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
        MenuManager.instance.UpdateStats();
        MenuManager.instance.StatsMenuUpdate(0);
    }
}
