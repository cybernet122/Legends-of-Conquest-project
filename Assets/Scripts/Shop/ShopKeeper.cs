using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    private bool canOpenShop;
    [SerializeField] List<ItemsManager> shopKeeperItemsForSale;

    private void Update()
    {
        if (canOpenShop && Input.GetButtonDown("Fire1") && Player.instance.enableMovement &&
            !ShopManager.instance.shopMenu.activeInHierarchy)
        {
            ShopManager.instance.OpenShopMenu();
            ShopManager.instance.itemsForSale = shopKeeperItemsForSale;
        }
        else if(Input.GetButtonDown("Cancel") && ShopManager.instance.shopMenu.activeInHierarchy)
        {
            ShopManager.instance.CloseShopMenu();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(QuestManager.instance.CheckIfComplete("Tell the Innkeeper that you found heroes to help you"))
            {
                GetComponent<DialogHandler>().enabled = false;
                canOpenShop = true;
            }            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canOpenShop = false;
        }
    }
}
