using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPiece : MonoBehaviour
{
    [SerializeField] GameObject uiCanvas;
    [SerializeField] int pieceNum;

    void Start()
    {
        // 씬 진입시, 이미 수집한 조각이면 비활성화
        if (GameManager.Instance.HasCollected(pieceNum)) gameObject.SetActive(false);
    }

    public void InteractionUIOn() { uiCanvas.SetActive(true); }
    public void InteratcionUIOff() { uiCanvas.SetActive(false); }
    public int GetPieceNum() { return pieceNum; }
}
