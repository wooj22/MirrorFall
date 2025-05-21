using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public GameObject OptionPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;
    float originalBGM;
    float originalSFX;

    void Start()
    {
        OptionPanel.SetActive(false);
        float savedBGM = PlayerPrefs.GetFloat("BGM", 0.8f);
        float savedSFX = PlayerPrefs.GetFloat("SFX", 0.8f);

        bgmSlider.value = savedBGM;
        sfxSlider.value = savedSFX;

        if (SoundManager2.Instance != null)
        {
            SoundManager2.Instance.SetBGMVolume(savedBGM);
            SoundManager2.Instance.SetSFXVolume(savedSFX);
        }
    }
    public void OnClickOptionButton()
    {// 옵션창 켜기 전에 값 백업
        originalBGM = PlayerPrefs.GetFloat("BGM", 0.8f);
        originalSFX = PlayerPrefs.GetFloat("SFX", 0.8f);

        bgmSlider.value = originalBGM;
        sfxSlider.value = originalSFX;
        OptionPanel.SetActive(true);
    }
    public void OnClickOKButton()
    {
        float bgm = bgmSlider.value;
        float sfx = sfxSlider.value;

        PlayerPrefs.SetFloat("BGM", bgm);
        PlayerPrefs.SetFloat("SFX", sfx);
        if (SoundManager2.Instance != null)
        {
            SoundManager2.Instance.SetBGMVolume(bgm);
            SoundManager2.Instance.SetSFXVolume(sfx);
        }
        OptionPanel.SetActive(false);
    }
    public void OnClickCancelButton()
    {
        bgmSlider.value = originalBGM;
        sfxSlider.value = originalSFX;
        if (SoundManager2.Instance != null)
        {
            SoundManager2.Instance.SetBGMVolume(originalBGM);
            SoundManager2.Instance.SetSFXVolume(originalSFX);
        }

        OptionPanel.SetActive(false);
    }
}
