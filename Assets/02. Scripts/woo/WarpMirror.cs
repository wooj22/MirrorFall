using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpMirror : MonoBehaviour
{
    // player�� nextSeneName�� get�ؼ� ��ų�� ���̵� ó�� (�� ���� �ſ� ������ ������)
    [SerializeField] public string nextSeneName;
    [SerializeField] private GameObject uiCanvas;

    public void InterationUIOn() { uiCanvas.SetActive(true); }
    public void InterationUIOff() { uiCanvas.SetActive(false); }
}
