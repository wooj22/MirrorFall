using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPiece : MonoBehaviour
{
    [SerializeField] GameObject uiCanvas;
    [SerializeField] int pieceNum;

    void Start()
    {
        // �� ���Խ�, �̹� ������ �����̶��
        if (GameManager.Instance.HasCollected(pieceNum)) 
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerUIHandler>().OnAdviceUI();
            gameObject.SetActive(false); 
        }
        else
        {
            // �÷��̾�� ���� (��ġ ������Ʈ��)
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().SetCurSceneMirrorPiece(this.gameObject);
        }
    }

    public void InteractionUIOn() { uiCanvas.SetActive(true); }
    public void InteratcionUIOff() { uiCanvas.SetActive(false); }
    public int GetPieceNum() { return pieceNum; }
}
