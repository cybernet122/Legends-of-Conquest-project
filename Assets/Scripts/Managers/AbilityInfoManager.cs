using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityInfoManager : MonoBehaviour
{
    [SerializeField] AttackEffect[] abilitiesAvailable;
    [SerializeField] GameObject[] abilitiesInfoPanel;
    [SerializeField] BattleCharacters[] playerPrefabs;
    [SerializeField] HorizontalLayoutGroup layoutGroup;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAbilitiesOfCharacter(PlayerStats playerSelected)
    {
        ClearAbilitiesSlots();
        FillFirstSlot();
        BattleCharacters battleChar = null;
        foreach (BattleCharacters prefab in playerPrefabs)
        {
            if (prefab.characterName == playerSelected.playerName)
            {
                battleChar = prefab;
            }
        }
        var charAvailableAbilities = battleChar.AttackMovesAvailable();
        for (int o = 0; o < charAvailableAbilities.Length; o++)
        {
            for (int i = 0; i < abilitiesAvailable.Length; i++)
            {
                print(abilitiesAvailable[i].name);
                if (charAvailableAbilities[o] == abilitiesAvailable[i].name)
                {
                    abilitiesInfoPanel[o + 1].SetActive(true);
                    abilitiesInfoPanel[o + 1].GetComponent<TooltipTrigger>().header = abilitiesAvailable[i].name;
                    abilitiesInfoPanel[o + 1].GetComponent<TooltipTrigger>().content = abilitiesAvailable[i].abilityInfo;
                    var image = abilitiesInfoPanel[o + 1].transform.GetChild(0).GetComponentInChildren<Image>();
                    image.gameObject.SetActive(true);
                    image.sprite = abilitiesAvailable[i].GetComponent<SpriteRenderer>().sprite;
                    image.preserveAspect = true;
                }
            }
        }

        if (charAvailableAbilities.Length < 4)
        {
            layoutGroup.childControlWidth = false;
        }
    }

    private void FillFirstSlot()
    {
        abilitiesInfoPanel[0].SetActive(true);
        abilitiesInfoPanel[0].GetComponent<TooltipTrigger>().header = abilitiesAvailable[0].name;
        abilitiesInfoPanel[0].GetComponent<TooltipTrigger>().content = abilitiesAvailable[0].abilityInfo;
        var img = abilitiesInfoPanel[0].transform.GetChild(0).GetComponentInChildren<Image>();
        img.gameObject.SetActive(true);
        img.sprite = abilitiesAvailable[0].GetComponent<SpriteRenderer>().sprite;
        img.preserveAspect = true;
    }

    private void ClearAbilitiesSlots()
    {
        foreach(GameObject ability in abilitiesInfoPanel)
        {
            ability.SetActive(false);
        }
    }
}
