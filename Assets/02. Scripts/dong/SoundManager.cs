using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("SFX ���")]
    public AudioClip[] sfxClips;       // �����Ϳ��� ���� Ŭ�� ���
    public string[] sfxNames;          // �� Ŭ���� �����ϴ� �̸�

    private Dictionary<string, AudioClip> sfxDict;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitSFXDictionary(); // ��ųʸ� �ʱ�ȭ
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
                Debug.LogWarning($"SFX �̸� �ߺ�: \"{sfxNames[i]}\" ���õ�");
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
            Debug.LogWarning($"SFX �̸� \"{name}\" ��(��) ã�� �� �����ϴ�.");
        }
    }
}
