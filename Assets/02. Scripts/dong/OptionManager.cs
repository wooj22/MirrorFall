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

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(savedBGM);
            SoundManager.Instance.SetSFXVolume(savedSFX);
        }
    }
    public void OnClickOptionButton()
    {// �ɼ�â �ѱ� ���� �� ���
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
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(bgm);
            SoundManager.Instance.SetSFXVolume(sfx);
        }
        OptionPanel.SetActive(false);
    }
    public void OnClickCancelButton()
    {
        bgmSlider.value = originalBGM;
        sfxSlider.value = originalSFX;
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(originalBGM);
            SoundManager.Instance.SetSFXVolume(originalSFX);
        }

        OptionPanel.SetActive(false);
    }
}
