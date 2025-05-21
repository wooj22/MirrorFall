using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager2 : MonoBehaviour
{
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] List<AudioClip> bgmClipList;
    [SerializeField] List<AudioClip> sfxClipList;
    [SerializeField] float fadeVolumeTime = 3f;

    // ΩÃ±€≈Ê
    public static SoundManager2 Instance { get; private set; }
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

    public AudioSource GetBgmSource()
    {
        return bgmSource;
    }

    /// BGM
    public void SetBGM(string clipName)
    {
        bgmSource.Stop();
        bgmSource.clip = bgmClipList.Find(clip => clip.name == clipName);
    }

    public void PlayBGM()
    {
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayOneShotBGM(string clipName)
    {
        AudioClip clipToPlay = bgmClipList.Find(clip => clip.name == clipName);
        bgmSource.PlayOneShot(clipToPlay);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void FadeInBGM()
    {
        StartCoroutine(FadeInVolume(bgmSource));
    }

    public void FadeOutBGM()
    {
        StartCoroutine(FadeOutVolume(bgmSource));
    }

    /// SFX
    public void PlaySFX(string clipName)
    {
        AudioClip clipToPlay = sfxClipList.Find(clip => clip.name == clipName);

        sfxSource.Stop();
        sfxSource.clip = clipToPlay;
        sfxSource.Play();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public float GetPlayTimeSFX()
    {
        return sfxSource.clip.length;
    }

    /// ∫º∑˝ ∆‰¿ÃµÂ¿Œ
    private IEnumerator FadeInVolume(AudioSource audio)
    {
        float targetVolume = 0.45f;
        float currentTime = 0f;

        audio.volume = 0;
        audio.Play();

        while (currentTime < fadeVolumeTime)
        {
            currentTime += Time.deltaTime;
            audio.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeVolumeTime);
            yield return null;
        }

        audio.volume = targetVolume;
    }

    /// ∫º∑˝ ∆‰¿ÃµÂæ∆øÙ
    private IEnumerator FadeOutVolume(AudioSource audio)
    {
        float startVolume = audio.volume;
        float currentTime = 0f;

        while (currentTime < fadeVolumeTime)
        {
            currentTime += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeVolumeTime);
            yield return null;
        }

        audio.volume = 0f;
        audio.Stop();
    }


    /*-------------- BGM Controll -----------------*/
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "01_Start") 
        {
            SoundManager2.Instance.SetBGM("BGM_Title");
            SoundManager2.Instance.PlayBGM();
        }
        else if(scene.name == "03_Tutorial")
        {
            SoundManager2.Instance.SetBGM("BGM_InGame");
            SoundManager2.Instance.FadeInBGM();
        }
        else if (scene.name == "09_Boss")
        {
            SoundManager2.Instance.SetBGM("BGM_InGame");
            SoundManager2.Instance.FadeInBGM();
        }
        else if (scene.name == "02_Opening" || scene.name == "10_GameClear" ||
            scene.name == "11_GameOver" || scene.name == "12_GameOverBoss")
        {
            SoundManager2.Instance.StopBGM();
        }
        
    }
}