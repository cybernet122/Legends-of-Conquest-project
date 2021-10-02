using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource[] SFX,backgroundMusic;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayBackgroundMusic(0);
        }
    }

    public void PlaySFX(int index)
    {
        if (index <= SFX.Length && index >= 0)
        {
            SFX[index].Play();
        }
        else
            Debug.LogWarning("Index for music requested out of bounds");
    }

    public void PlayBackgroundMusic(int musicToPlay)
    {
        StopMusic();
        if(musicToPlay < backgroundMusic.Length)
        {
            backgroundMusic[musicToPlay].Play();
        }
    }

    public void StopMusic()
    {
        foreach(AudioSource music in backgroundMusic)
        {
            music.Stop();
        }
    }
    public void StopSFX()
    {
        foreach (AudioSource sfx in SFX)
        {
            sfx.Stop();
        }
    }

}
