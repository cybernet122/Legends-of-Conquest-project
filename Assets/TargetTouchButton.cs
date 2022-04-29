using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetTouchButton : MonoBehaviour
{
    private string moveName;
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        button.interactable = false;
    }

    private void OnEnable()
    {
        BattleManager.MoveName += SetMoveName;
    }
    private void OnDisable()
    {
        BattleManager.MoveName -= SetMoveName;
    }

    private void SetMoveName(string value)
    {
        button.interactable = true;
        moveName = value;
    }    

    public void Press()
    {
        button.interactable = false;
        if (moveName != "")
        BattleManager.instance.PlayerAttack(moveName, GetComponentInParent<BattleCharacters>());
    }
}
