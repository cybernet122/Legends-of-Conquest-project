using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleTargetButtons : MonoBehaviour
{
    public string moveName;
    public BattleCharacters activeBattleTarget;
    public TextMeshProUGUI targetName;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        targetName = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Press()
    {
        BattleManager.instance.PlayerAttack(moveName,activeBattleTarget);
    }
}
