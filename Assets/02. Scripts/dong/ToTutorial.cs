using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTutorial : MonoBehaviour
{
    public GameObject SkipButton;

    void Start()
    {
        if (SkipButton != null)
        {
            SkipButton.SetActive(true);
            StartCoroutine(UnShowSkipButton());
        }
    }
    IEnumerator UnShowSkipButton()
    {
        yield return new WaitForSeconds(8f);
        SkipButton.SetActive(false);
    }
    public void OnClickSkipButton()
    {
        SceneSwitch.Instance.SceneSwithcing("03_Tutorial");
    }
}
