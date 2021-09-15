using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private Sprite itemsImage;
    List<ItemsManager> itemsList = new List<ItemsManager>();
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItems(ItemsManager item, bool shopItem)
    {
        if (item.isStackable)
        {
            bool itemAlreadyInInventory = false;
            foreach (ItemsManager itemInInventory in itemsList)
            {
                if (!item.isStackable) { break; }
                if (itemInInventory.itemName == item.itemName && !shopItem)
                {
                    itemInInventory.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
                else if (itemInInventory.itemName == item.itemName && shopItem && item.isStackable)
                {
                    itemInInventory.amount ++;
                    itemAlreadyInInventory = true;
                }
            }
            if (!itemAlreadyInInventory)
            {
                item.amount = 1;
                itemsList.Add(item);
            }
        }
        else
        {
            itemsList.Add(item);
        }
    }

    public void RemoveItem(ItemsManager item)
    {
        if (item.isStackable)
        {
            ItemsManager invetoryItem = null;
            foreach(ItemsManager itemInInventory in itemsList)
            {
                if(itemInInventory.itemName == item.itemName)
                {
                    itemInInventory.amount--;
                    invetoryItem = itemInInventory;
                }
            }
            if (invetoryItem != null && invetoryItem.amount <= 0)
            {
                itemsList.Remove(invetoryItem);
            }
        }
        else
        {
            itemsList.Remove(item);
        }
        MenuManager.instance.UpdateItemsInventory();
    }

    public List<ItemsManager> GetItemList()
    {
        return itemsList;
    }
}
