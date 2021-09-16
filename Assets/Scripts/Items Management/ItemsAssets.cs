using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsAssets : MonoBehaviour
{
    public static ItemsAssets instance;
    [SerializeField] ItemsManager[] itemsAvailable;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else instance = this;
    }

    public ItemsManager GetItemsAsset(string itemToGetName)
    {
        foreach(ItemsManager item in itemsAvailable)
        {
            if(item.itemName == itemToGetName)
            {
                return item;
            }
        }
        return null;
    }



    /*    public void SaveInventory()
{
    var itemList = GetItemList();
    foreach (ItemsManager item in itemList)
    {
        if (item.isStackable)
        {
            for (int i = 0; i <= item.amount; i++)
                PlayerPrefs.SetInt("ItemInInventory_" + item, i);
        }
        else
        {
            PlayerPrefs.SetInt("ItemInInventory_" + item, 1);
        }
    }
}
public void LoadInventory()
{
    for(int)
    PlayerPrefs.GetInt("ItemInInventory_")
}*/
}
