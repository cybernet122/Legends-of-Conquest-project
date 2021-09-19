using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamController : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    [SerializeField] int muiscToPlay;
    bool musicAlreadyPlaying;
    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = Player.instance.gameObject.transform;
        var virtualCameraConfiner = GetComponent<CinemachineConfiner>();
        virtualCameraConfiner.m_BoundingShape2D = FindObjectOfType<Grid>().GetComponentInChildren<PolygonCollider2D>();
    }

    private void Update()
    {
        if (!musicAlreadyPlaying)
        {
            musicAlreadyPlaying = true;
            AudioManager.instance.PlayBackgroundMusic(muiscToPlay);
        }
    }
}
