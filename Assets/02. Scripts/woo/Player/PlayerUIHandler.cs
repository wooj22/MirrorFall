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

    /// 씬을 이동하면서 그 씬에 배치된 UI를 찾아야함
    void FindSetUI()
    {
        Hdu_Image = GameObject.Find("HDU_Image").GetComponent<Image>();
    }

    // Update HP UI
    public void UpdateHduUI(int curHp)
    {
        Hdu_Image.sprite = HduSpriteArr[curHp];
    }
}
