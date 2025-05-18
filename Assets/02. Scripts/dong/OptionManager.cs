using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public GameObject OptionPanel;
    public Slider bgmSlider;
    public Slider vfxSlider;
    float originalBGM;
    float originalVFX;

    void Start()
    {
        OptionPanel.SetActive(false);
    }
    public void OnClickOptionButton()
    {// 옵션창 켜기 전에 값 백업
        originalBGM = PlayerPrefs.GetFloat("BGM", 0.8f);
        originalVFX = PlayerPrefs.GetFloat("VFX", 0.8f);

        bgmSlider.value = originalBGM;
        vfxSlider.value = originalVFX;
        OptionPanel.SetActive(true);
    }
    public void OnClickOKButton()
    {
        float bgm = bgmSlider.value;
        float vfx = vfxSlider.value;

        PlayerPrefs.SetFloat("BGM", bgm);
        PlayerPrefs.SetFloat("VFX", vfx);
        OptionPanel.SetActive(false);
    }
    public void OnClickCancelButton()
    {
        bgmSlider.value = originalBGM;
        vfxSlider.value = originalVFX;

        OptionPanel.SetActive(false);
    }
}
