using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private string nextSeneName;
    [SerializeField] private GameObject uiCanvas;

    public void InterationUIOn() { uiCanvas.SetActive(true); }
    public void InterationUIOff() { uiCanvas.SetActive(false); }

    // nextSeneName으로 이동
    public void DoorOpen()
    {
        SceneSwitch.Instance.SceneSwithcing(nextSeneName);
    }
}
