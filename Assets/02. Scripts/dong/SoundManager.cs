using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("SFX 등록")]
    public AudioClip[] sfxClips;       // 에디터에서 여러 클립 등록
    public string[] sfxNames;          // 각 클립에 대응하는 이름

    private Dictionary<string, AudioClip> sfxDict;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitSFXDictionary(); // 딕셔너리 초기화
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitSFXDictionary()
    {
        sfxDict = new Dictionary<string, AudioClip>();

        for (int i = 0; i < Mathf.Min(sfxClips.Length, sfxNames.Length); i++)
        {
            if (!sfxDict.ContainsKey(sfxNames[i]))
            {
                sfxDict.Add(sfxNames[i], sfxClips[i]);
            }
            else
            {
                Debug.LogWarning($"SFX 이름 중복: \"{sfxNames[i]}\" 무시됨");
            }
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX 이름 \"{name}\" 을(를) 찾을 수 없습니다.");
        }
    }
}
