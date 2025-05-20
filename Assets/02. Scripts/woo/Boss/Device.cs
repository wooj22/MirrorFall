using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HDROutputUtils;

public class Device : MonoBehaviour
{
    [SerializeField] GameObject uiCanvas;
    [SerializeField] int deviceNum;
    [SerializeField] bool isOperation;

    public void InteractionUIOn() { if(!isOperation) uiCanvas.SetActive(true); }
    public void InteractionUIOff() { uiCanvas.SetActive(false); }
    public void Operation() {
        if (!isOperation)
        {
            // 여기 제어 연결 코드 작성
            isOperation = true;
            InteractionUIOff();
            Debug.Log(deviceNum + "번 장치 발동");
        }
            
    }
}
