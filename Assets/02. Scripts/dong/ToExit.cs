using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToExit : MonoBehaviour
{
    public void ExitGame()
    {
        #if UNITY_EDITOR
           UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();  // 빌드된 게임 종료
        #endif
    }
}
