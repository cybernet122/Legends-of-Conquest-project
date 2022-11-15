using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static LTDescr delay;
    public string header;
    [TextArea]
    public string content;
    public static event UnityAction<Vector3> ShowAtPosition;

    private void OnEnable()
    {
        MenuManager.InspectingStats += EnableButtons;
    }
    private void OnDisable()
    {
        MenuManager.InspectingStats -= EnableButtons;
    }

    private void Update()
    {
    }

    private void EnableButtons(bool condition)
    {
        if(GetComponent<Button>())
        GetComponent<Button>().interactable = condition;
    }

    public void ShowTooltip()
    {
        //SetSelectedGameObject();
        if (EventSystem.current.currentSelectedGameObject.name == gameObject.name)
        {
            TooltipSystem.Show(false, content, header);
            ShowAtPosition?.Invoke(transform.position);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.7f, () =>
        {
            //SetSelectedGameObject();
            TooltipSystem.Show(true,content, header);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }

    private void SetSelectedGameObject()
    {
        Button button;
        if (GetComponent<Button>())
        {
            button = GetComponent<Button>();
            Utilities.SetSelectedAndHighlight(gameObject, button);
        }
    }
}
