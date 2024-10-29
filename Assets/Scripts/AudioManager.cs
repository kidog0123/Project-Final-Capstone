using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AmThanh[] musicSounds, sfxSounds; //mang nhạc nên và nhạc hiệu ứng 

    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bool sfxMuted = PlayerPrefs.GetInt("SfxMuted", 0) == 1;
        sfxSource.mute = sfxMuted;
        bool musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        musicSource.mute = musicMuted;
        PlayMusic("Nen");
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }

    public void PlayMusic(string name)//gọi tên nhạc nền
    {
        AmThanh a = Array.Find(musicSounds, x => x.name == name);
        if (a == null)
        {
            Debug.Log("không tìm thấy âm thanh");
        }
        else
        {
            musicSource.clip = a.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        AmThanh a = Array.Find(sfxSounds, x => x.name == name);
        if (a == null)
        {
            Debug.Log("không tìm thấy âm thanh");
        }
        else
        {
            sfxSource.PlayOneShot(a.clip);

        }
    }//goi tên hiệu ứng
    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }
    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1; // Đặt lại Time.timeScale khi cảnh mới được tải
    }
}
