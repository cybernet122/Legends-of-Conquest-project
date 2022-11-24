using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SwitchActiveMap : MonoBehaviour
{
    private InputActionMap UI, ShopUI, BattleUI, PlayerMap;
    public PlayerInput playerInput;
    public static SwitchActiveMap instance;
    private SwitchInputModule switchInputModule;
    private void GetSwitchInputModule() { switchInputModule = EventSystem.current.GetComponent<SwitchInputModule>(); }

    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
        playerInput = GetComponent<PlayerInput>();
        UI = playerInput.actions.FindActionMap("UI");
        ShopUI = playerInput.actions.FindActionMap("ShopUI");
        BattleUI = playerInput.actions.FindActionMap("BattleUI");
        PlayerMap = playerInput.actions.FindActionMap("Player");
        string sceneName = Utilities.ReturnSceneName();
        if (sceneName != "Main Menu" && sceneName != "Loading Scene")
            if (EventSystem.current.GetComponent<SwitchInputModule>())
                switchInputModule = EventSystem.current.GetComponent<SwitchInputModule>();
    }

    public InputActionMap GetInputAction()
    {
        InputActionMap[] inputActions = new InputActionMap[] { UI, ShopUI, BattleUI, PlayerMap };
        for (int i = 0; i < inputActions.Length; i++)
        {
            if (inputActions[i].enabled)
            {
                return inputActions[i];
            }
        }
        throw new Exception();
    }

    public void SwitchToUI()
    {
        UI.Enable();
        ShopUI.Disable();
        BattleUI.Disable();
        PlayerMap.Disable();
        if (switchInputModule)
            switchInputModule.SwitchToUI();
        else
        {
            GetSwitchInputModule();
            switchInputModule.SwitchToUI();
        }
    }


    public void SwitchToShopUI()
    {
        UI.Disable();
        ShopUI.Enable();
        BattleUI.Disable();
        PlayerMap.Disable();
        if (switchInputModule)
            switchInputModule.SwitchToShopUI();
        else
        {
            GetSwitchInputModule();
            switchInputModule.SwitchToShopUI();
        }
    }

    public void SwitchToBattleUI()
    {
        UI.Disable();
        ShopUI.Disable();
        BattleUI.Enable();
        PlayerMap.Disable();
        if (switchInputModule)
            switchInputModule.SwitchToBattleUI();
        else
        {
            GetSwitchInputModule();
            switchInputModule.SwitchToBattleUI();
        }
    }

    public void SwitchToPlayer()
    {
        UI.Disable();
        ShopUI.Disable();
        BattleUI.Disable();
        PlayerMap.Enable();
    }
}
