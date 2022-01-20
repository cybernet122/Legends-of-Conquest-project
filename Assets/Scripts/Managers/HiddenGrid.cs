using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenGrid : MonoBehaviour
{
    [SerializeField] bool inside;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] GameObject[] grids;
    [SerializeField] OutsideGrid outsideGrid;
    [SerializeField] GameObject treasureChest, battleInstantiator;

    private void Start()
    {
        HideHidden();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShowHidden();
        }
    }

    public void ShowHidden()
    {
        foreach(GameObject grid in grids)
        {
            grid.SetActive(true);
        }
        outsideGrid.HideMap();
        treasureChest.SetActive(true);
        battleInstantiator.SetActive(true);
    }

    public void HideHidden()
    {
        foreach (GameObject grid in grids)
        {
            grid.SetActive(false);
        }
        treasureChest.SetActive(false);
        battleInstantiator.SetActive(false);
    }
}
