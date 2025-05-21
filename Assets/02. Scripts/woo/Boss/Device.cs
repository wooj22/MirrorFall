using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    [SerializeField] GameObject uiCanvas;
    [SerializeField] bool isOperation;
    private BossSceneManager manager;

    private void Start()
    {
        manager = GameObject.Find("BossSceneManager").GetComponent<BossSceneManager>();
    }

    public void InteractionUIOn() { if(!isOperation) uiCanvas.SetActive(true); }
    public void InteractionUIOff() { uiCanvas.SetActive(false); }
    public void Operation() {
        if (!isOperation)
        {
            manager.DeviceOperationCheak();
            isOperation = true;
            InteractionUIOff();
            SoundManager2.Instance.PlaySFX("SFX_PillarActivatet");
        }     
    }
}
