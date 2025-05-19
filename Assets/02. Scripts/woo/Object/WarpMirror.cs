using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpMirror : MonoBehaviour
{
    [SerializeField] private Vector2 nextMirrorPos;
    [SerializeField] private float warpCoolTime;
    [SerializeField] private float warpTimmer;
    [SerializeField] public bool isWarpReady;

    // player가 nextSeneName을 get해서 스킬로 씬이동 처리 (그 씬의 거울 앞으로 가야함)
    [SerializeField] private string nextSeneName;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Text interactionText;

    private void Update()
    {
        warpTimmer += Time.deltaTime;
        if(warpTimmer > warpCoolTime) isWarpReady = true;
        else isWarpReady = false;

        // 1초마다 UI 업데이트
        Invoke(nameof(UpdateWarpTimeUI), 1f);
    }


    public void InterationUIOn() { uiCanvas.SetActive(true); }
    public void InterationUIOff() { uiCanvas.SetActive(false); }

    // 워프 타임 
    public void UpdateWarpTimeUI()
    {
        if(isWarpReady) interactionText.text = "워프 [F]";
        else interactionText.text = (int)warpCoolTime - (int)warpTimmer + "초 뒤\n워프 가능";
    }

    // 워프
    public string WarpGetNextScene()
    {
        if (!isWarpReady) return null;

        warpTimmer = 0f;
        return nextSeneName;
    }

    // 워프할 위치 getter
    public Vector2 GetWarpPosition() { return nextMirrorPos; }
}
