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
            // ���� ���� ���� �ڵ� �ۼ�
            isOperation = true;
            InteractionUIOff();
            Debug.Log(deviceNum + "�� ��ġ �ߵ�");
        }
            
    }
}
