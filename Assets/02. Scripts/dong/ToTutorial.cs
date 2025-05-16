using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTutorial : MonoBehaviour
{
    public void OnClickSkipButton()
    {
        SceneSwitch.Instance.SceneSwithcing("03_Tutorial");
    }
}
