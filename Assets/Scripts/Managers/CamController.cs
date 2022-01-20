using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamController : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    [SerializeField] int musicToPlay;
    bool musicAlreadyPlaying;
    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = Player.instance.gameObject.transform;
            var virtualCameraConfiner = GetComponent<CinemachineConfiner>();
            virtualCameraConfiner.m_BoundingShape2D = FindObjectOfType<Grid>().GetComponentInChildren<PolygonCollider2D>();
        }
    }

    private void Update()
    {
        if (!GameManager.instance.battleIsActive && musicToPlay != AudioManager.instance.GetMusicIndex() || !AudioManager.instance.IsPlaying() && musicAlreadyPlaying == false)
        {
            musicAlreadyPlaying = true;
            AudioManager.instance.PlayBackgroundMusic(musicToPlay);
        }
    }

    public int GetMusicIndex()
    {
        return musicToPlay;
    }
}
