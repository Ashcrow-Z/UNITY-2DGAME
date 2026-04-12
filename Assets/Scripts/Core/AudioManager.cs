using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM Clips")]
    public AudioClip menuBGM;
    public AudioClip level1BGM;
    public AudioClip level2BGM;

    [Header("SFX Clips")]
    public AudioClip shootSFX;
    public AudioClip hitSFX;
    public AudioClip explosionSFX;
    public AudioClip pickupSFX;
    public AudioClip hurtSFX;
    public AudioClip victorySFX;
    public AudioClip defeatSFX;
    public AudioClip buttonClickSFX;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ApplySavedVolumes();
    }

    void ApplySavedVolumes()
    {
        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        if (bgmSource != null) bgmSource.volume = bgmVol;
        if (sfxSource != null) sfxSource.volume = sfxVol;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public float GetBGMVolume() => PlayerPrefs.GetFloat("BGMVolume", 0.5f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat("SFXVolume", 0.7f);

    public void PlayMenuMusic()
    {
        if (menuBGM != null) PlayBGM(menuBGM);
    }

    public void PlayLevelMusic(int level)
    {
        AudioClip clip = level == 1 ? level1BGM : level2BGM;
        if (clip != null) PlayBGM(clip);
    }
}
