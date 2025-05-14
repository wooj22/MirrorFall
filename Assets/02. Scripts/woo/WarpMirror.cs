using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpMirror : MonoBehaviour
{
    // player가 nextSeneName을 get해서 스킬로 씬이동 처리 (그 씬의 거울 앞으로 가야함)
    [SerializeField] public string nextSeneName;
    [SerializeField] private GameObject uiCanvas;

    public void InterationUIOn() { uiCanvas.SetActive(true); }
    public void InterationUIOff() { uiCanvas.SetActive(false); }
}
