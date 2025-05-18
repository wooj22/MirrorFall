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

    // player�� nextSeneName�� get�ؼ� ��ų�� ���̵� ó�� (�� ���� �ſ� ������ ������)
    [SerializeField] private string nextSeneName;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Text interactionText;

    private void Update()
    {
        warpTimmer += Time.deltaTime;
        if(warpTimmer > warpCoolTime) isWarpReady = true;
        else isWarpReady = false;

        // 1�ʸ��� UI ������Ʈ
        Invoke(nameof(UpdateWarpTimeUI), 1f);
    }


    public void InterationUIOn() { uiCanvas.SetActive(true); }
    public void InterationUIOff() { uiCanvas.SetActive(false); }

    // ���� Ÿ�� 
    public void UpdateWarpTimeUI()
    {
        if(isWarpReady) interactionText.text = "���� [F]";
        else interactionText.text = (int)warpCoolTime - (int)warpTimmer + "�� ��\n���� ����";
    }

    // ����
    public string WarpGetNextScene()
    {
        if (!isWarpReady) return null;

        warpTimmer = 0f;
        return nextSeneName;
    }

    // ������ ��ġ getter
    public Vector2 GetWarpPosition() { return nextMirrorPos; }
}
