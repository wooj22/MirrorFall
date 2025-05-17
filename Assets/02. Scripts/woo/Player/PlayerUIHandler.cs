using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private Image hp_Image;
    [SerializeField] Sprite[] hpSpriteArr = new Sprite[3];
    [SerializeField] Image[] mirrorUIArr = new Image[5];
    [SerializeField] SpriteRenderer[] arrowArr = new SpriteRenderer[8];

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

    public void UpdateHpUI(int curHp) { hp_Image.sprite = hpSpriteArr[curHp]; }

    public void UpdateGetMirrorUI(int mirrorNum) { mirrorUIArr[mirrorNum - 1].enabled = true; }

    public void UpdateMissMirrorUI(int missMirrorNum) { mirrorUIArr[missMirrorNum - 1].enabled = false; }

    public void UpdateArrowUI(int way)
    {
        for (int i = 0; i < arrowArr.Length; i++)
        {
            arrowArr[i].enabled = (i == way);
        }
    }
}
