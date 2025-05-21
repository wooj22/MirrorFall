using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToStartManager : MonoBehaviour
{
    public GameObject ToStartButton;
    void Start()
    {
        if (ToStartButton != null)
        {
            ToStartButton.SetActive(false);
            StartCoroutine(ShowToStartButton());
        }
    }

    // Update is called once per frame
    IEnumerator ShowToStartButton()
    {
        yield return new WaitForSeconds(3f);
        ToStartButton.SetActive(true);
        yield return new WaitForSeconds(8f);
        ToStartButton.SetActive(false);
    }
    public void OnClickToStartButton()
    {
        SceneSwitch.Instance.SceneSwithcing("01_Start");
    }
}
