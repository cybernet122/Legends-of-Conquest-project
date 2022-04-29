using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource[] SFX,backgroundMusic;
    bool lerpTo0 = true, finishedLerping = false;
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
        if (PlayerPrefs.HasKey("Music_Volume_"))
        {
            SetVolumeMusic(PlayerPrefs.GetFloat("Music_Volume_"));
            SetVolumeSFX(PlayerPrefs.GetFloat("SFX_Volume_"));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StopMusic();
        }
    }

    public void StopMusic()
    {
        foreach (AudioSource music in backgroundMusic)
        {
            music.Stop();
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
        bool isMusicPlaying = false;
        foreach (AudioSource music in backgroundMusic)
            if (music.isPlaying)
                isMusicPlaying = true;
        if (GetMusicIndex() == musicToPlay && isMusicPlaying)
            return;
        StopMusic();
        if(musicToPlay < backgroundMusic.Length)
        {            
            backgroundMusic[musicToPlay].Play();
        }
    }
    
    public void StopSFX()
    {
        foreach (AudioSource sfx in SFX)
        {
            sfx.Stop();
        }
    }

    public void SetVolumeMusic(float value)
    {
        foreach(AudioSource music in backgroundMusic)
        {
            music.volume = value;
        }
    }
    
    public void SetVolumeSFX(float value)
    {
        foreach (AudioSource sfx in SFX)
        {
            sfx.volume = value;
        }
    }

    public bool IsPlaying()
    {
        foreach(var music in backgroundMusic)
        {
            if (music.isPlaying)
                return true;
        }
        return false;
    }

    public int GetMusicIndex()
    {
        for(int i = 0; i < backgroundMusic.Length; i++)
        {
            if (backgroundMusic[i].isPlaying)
                return i;
        }
        return 0;
    }
}
