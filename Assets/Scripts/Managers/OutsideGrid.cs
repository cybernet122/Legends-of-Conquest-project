using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideGrid : MonoBehaviour
{
    [SerializeField] bool outside;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] GameObject[] grids;
    [SerializeField] HiddenGrid hiddenGrid;
    [SerializeField] GameObject battleInstantiator;

    private void Start()
    {
        ShowMap();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShowMap();
        }
    }

    public void ShowMap()
    {
        foreach (GameObject grid in grids)
        {
            grid.SetActive(true);
        }
        hiddenGrid.HideHidden();
        battleInstantiator.SetActive(true);
    }

    public void HideMap()
    {
        foreach (GameObject grid in grids)
        {
            grid.SetActive(false);
        }
        battleInstantiator.SetActive(false);
    }
}
