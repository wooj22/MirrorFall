using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private Image Hdu_Image;
    [SerializeField] Sprite[] HduSpriteArr = new Sprite[3];
    [SerializeField] private Text interation_Text;

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

    /// ���� �̵��ϸ鼭 �� ���� ��ġ�� UI�� ã�ƾ���
    void FindSetUI()
    {
        Hdu_Image = GameObject.Find("HDU_Image").GetComponent<Image>();
    }

    // Update HP UI
    public void UpdateHpUI(int curHp)
    {
        Hdu_Image.sprite = HduSpriteArr[curHp];
    }

    // Interaction UI
    public void InteractionUIOn(string text)
    {
        interation_Text.text = text;
    }

    // Interaction UI
    public void InterationUIOff()
    {
        interation_Text.text = "";
    }
}
