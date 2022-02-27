using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem instance;
    public Tooltip tooltip;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (!GameManager.instance.gameMenuOpened && instance.tooltip.isActiveAndEnabled) { Hide(); }
    }

    public static void Show(string content, string header = "")
    {
        instance.canvasGroup.LeanAlpha(1, 0.5f);
        instance.tooltip.SetText(content, header);
        instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        instance.tooltip.gameObject.SetActive(false);
        instance.tooltip.ClearText();
        instance.canvasGroup.alpha = 0;
    }
}
