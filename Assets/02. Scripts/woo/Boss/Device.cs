using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    [SerializeField] Sprite deviceOnImage;
    [SerializeField] GameObject uiCanvas;
    [SerializeField] bool isOperation;
    private SpriteRenderer sr;
    private BossSceneManager manager;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        manager = GameObject.Find("BossSceneManager").GetComponent<BossSceneManager>();
    }

    public void InteractionUIOn() { if(!isOperation) uiCanvas.SetActive(true); }
    public void InteractionUIOff() { uiCanvas.SetActive(false); }
    public void Operation() {
        if (!isOperation)
        {
            manager.DeviceOperationCheak();
            sr.sprite = deviceOnImage;  
            isOperation = true;
            InteractionUIOff();
            SoundManager2.Instance.PlaySFX("SFX_PillarActivatet");
        }     
    }
}
