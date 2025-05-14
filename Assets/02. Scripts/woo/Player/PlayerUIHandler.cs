using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private Image Hdu_Image;
    [SerializeField] Sprite[] HduSpriteArr = new Sprite[3];

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
        Hdu_Image.sprite = HduSpriteArr[curHp];
    }
}
