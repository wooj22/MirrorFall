using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoEndManager2 : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // ���� ������ �̺�Ʈ ���
        videoPlayer.loopPointReached += OnVideoFinished;
    }
    void OnVideoFinished(VideoPlayer vp)
    {
        SceneSwitch.Instance.SceneSwithcing("01_Start");
    }
    private void OnDestroy()
    {
        // �̺�Ʈ ����
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
