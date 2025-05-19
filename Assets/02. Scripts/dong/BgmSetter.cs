using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmSetter : MonoBehaviour
{
    public AudioClip bgmClip;  // ���� �´� BGM Ŭ��

    void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager�� �����ϴ�!");
            return;
        }

        if (SoundManager.Instance.bgmSource.clip != bgmClip)
        {
            SoundManager.Instance.bgmSource.clip = bgmClip;
            SoundManager.Instance.bgmSource.loop = true;
            SoundManager.Instance.bgmSource.Play();
        }
    }
}
