using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class LevelManager : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    Vector3 bottomLeftEdge, topRightEdge;
    private void Start()
    {
        SetLimit();
    }

    public void SetLimit()
    {
        bottomLeftEdge = tilemap.localBounds.min + new Vector3(1f, 1f, 0f);
        topRightEdge = tilemap.localBounds.max + new Vector3(-1f, -1f, 0f);
        Player.instance.SetLimit(bottomLeftEdge, topRightEdge);
    }
}
