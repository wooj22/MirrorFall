using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    // 감정 hp ui
    [SerializeField] private Image hp_Image;
    [SerializeField] Sprite[] hpSpriteArr = new Sprite[3];

    // 거울 조각 ui
    [SerializeField] Image[] mirrorUIArr = new Image[5];

    public static PlayerUIHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update HP UI
    public void UpdateHpUI(int curHp)
    {
        hp_Image.sprite = hpSpriteArr[curHp];
    }

    // Update Mirror UI
    public void UpdateGetMirrorUI(int mirrorNum)
    {
        mirrorUIArr[mirrorNum - 1].enabled = true;
    }

    // 보스전 Init을 위해 5번조각 reset
    public void UpdateMissMirrorUI(int missMirrorNum)
    {
        mirrorUIArr[missMirrorNum - 1].enabled = false;
    }
}
