using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private string nextSeneName;
    [SerializeField] private GameObject uiCanvas;

    public void InterationUIOn() { uiCanvas.SetActive(true); }
    public void InterationUIOff() { uiCanvas.SetActive(false); }

    public string DoorGetNextScene() { return nextSeneName; }
}
