using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoEndManager : MonoBehaviour
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
        SceneSwitch.Instance.SceneSwithcing("03_Tutorial");
    }
    private void OnDestroy()
    {
        // �̺�Ʈ ����
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
