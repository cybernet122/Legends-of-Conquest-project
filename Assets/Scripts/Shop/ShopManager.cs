using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopMenu, buyPanel, sellPanel;
    [SerializeField] TextMeshProUGUI currentGoldCoinText;
    // Start is called before the first frame update
    void Start()
    {
        shopMenu.SetActive(false);
        buyPanel.SetActive(false);
        sellPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            OpenShopMenu();
    }

    public void OpenShopMenu()
    {
        shopMenu.SetActive(true);
        GameManager.instance.shopMenuOpened = true;
        currentGoldCoinText.text = "Gold coins: " + GameManager.instance.currentGoldCoins;
        buyPanel.SetActive(true);
    }

    public void CloseShopMenu()
    {
        shopMenu.SetActive(false);
        GameManager.instance.shopMenuOpened = false;
    }

    public void OpenBuyPanel()
    {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);
    }

    public void OpenSellPanel()
    {
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);
    }
}
