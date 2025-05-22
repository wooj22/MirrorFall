using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickSound : MonoBehaviour
{
    public void PlayButtonSound() { SoundManager2.Instance.PlaySFX("SFX_UIButton"); }   // click
    public void PlayButtonOnSound() { SoundManager2.Instance.PlaySFX("SFX_UIButtonSlide"); }  // cursor on
}
