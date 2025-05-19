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

        // 영상 끝나는 이벤트 등록
        videoPlayer.loopPointReached += OnVideoFinished;
    }
     void OnVideoFinished(VideoPlayer vp)
    {
        SceneSwitch.Instance.SceneSwithcing("03_Tutorial");
    }
    private void OnDestroy()
    {
        // 이벤트 해제
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
