using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] List<ItemsManager> shopKeeperItemsForSale;
    bool shopInRange = false;

    private void OnEnable()
    {
        MenuManager.CloseMenu += CheckForShop;
    }
    private void OnDisable()
    {
        MenuManager.CloseMenu += CheckForShop;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            shopInRange = true;
            CheckForShop();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            shopInRange = true;
        }
    }

    public void CheckForShop()
    {
        if (Utilities.ReturnSceneName() == "Shop" && (QuestManager.instance.CheckIfComplete("Return to the Innkeeper") || !GetComponent<DialogHandler>()) && shopInRange)
        {
            ShopManager.instance.canOpenShop = true;
            ShopManager.instance.itemsForSale = shopKeeperItemsForSale;
            MenuManager.instance.ShowInfoText("Interact with shop");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ShopManager.instance.canOpenShop = false;
            ShopManager.instance.itemsForSale.Clear();
            shopInRange = false;
            MenuManager.instance.HideInfoText();
        }
    }
}
