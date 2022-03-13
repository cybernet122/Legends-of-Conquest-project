using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleTargetButtons : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerExitHandler, IDeselectHandler
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

    public void OnSelect(BaseEventData eventData)
    {
        activeBattleTarget.ShowRing();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        activeBattleTarget.ShowRing();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        activeBattleTarget.HideRing();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        activeBattleTarget.HideRing();
    }
}
