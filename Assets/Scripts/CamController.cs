using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamController : MonoBehaviour
{
    Player player;
    CinemachineVirtualCamera virtualCamera;
    private void Start()
    {
        player = FindObjectOfType<Player>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform;
        var virtualCameraConfiner = GetComponent<CinemachineConfiner>();
        virtualCameraConfiner.m_BoundingShape2D = FindObjectOfType<Grid>().GetComponentInChildren<PolygonCollider2D>();
    }
}
