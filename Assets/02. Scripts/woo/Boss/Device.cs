using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    [SerializeField] GameObject uiCanvas;

    public void InteractionUIOn() { uiCanvas.SetActive(true); }
    public void InteractionUIOff() { uiCanvas.SetActive(false); }
}
