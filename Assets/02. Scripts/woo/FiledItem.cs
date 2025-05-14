using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiledItem : MonoBehaviour
{
    [SerializeField] GameObject uiCanvas;

    public void InterationUIOn() { uiCanvas.SetActive(true); }
    public void InterationUIOff() { uiCanvas.SetActive(false); }
}
