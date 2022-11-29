using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoText : MonoBehaviour
{
    AreaExit areaExit;
    LTDescr countdownToTeleport;
    bool startCounting;

    private void Start()
    {
        areaExit = GetComponentInParent<AreaExit>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            countdownToTeleport = LeanTween.delayedCall(3f, () => { areaExit.ChangingArea(); });
            startCounting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LeanTween.cancel(countdownToTeleport.id);
            MenuManager.instance.HideInfoText();
            startCounting = false;
        }
    }

    private void Update()
    {
        if (startCounting)
        {
            int countdown = (int)(countdownToTeleport.time) - (int)(countdownToTeleport.passed);
            if (countdownToTeleport.delay >= 0f && countdownToTeleport.hasInitiliazed)
            {
                if (!areaExit.teleportAvailable)
                    MenuManager.instance.ShowInfoText("Entering " + areaExit.sceneToLoad + " in " + countdown);
                else
                    MenuManager.instance.ShowInfoText("Moving to upper area in " + countdown);
            }
        }
    }

    public void StopCountdown()
    {
        LeanTween.cancel(countdownToTeleport.id);
        MenuManager.instance.HideInfoText();
        startCounting = false;
    }
}
