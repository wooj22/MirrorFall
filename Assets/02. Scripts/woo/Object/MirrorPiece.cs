using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPiece : MonoBehaviour
{
    [SerializeField] GameObject uiCanvas;
    [SerializeField] int pieceNum;

    public void InteractionUIOn() { uiCanvas.SetActive(true); }
    public void InteratcionUIOff() { uiCanvas.SetActive(false); }
}
