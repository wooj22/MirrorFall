using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmSetter : MonoBehaviour
{
    public AudioClip bgmClip;  // 씬에 맞는 BGM 클립

    void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager가 없습니다!");
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
