using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToOpening : MonoBehaviour
{
    public void OnClickPlayButton()
    {
        SceneSwitch.Instance.SceneSwithcing("02_Opening");
    }
}
