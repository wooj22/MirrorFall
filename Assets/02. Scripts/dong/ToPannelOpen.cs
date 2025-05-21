using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToPannelOpen : MonoBehaviour
{
    public GameObject PannelOpenButton;
    public GameObject RetryPannel;
    void Start()
    {
        PannelOpenButton.SetActive(false);
        RetryPannel.SetActive(false);
        StartCoroutine(ShowPannelOpenButton());
    }
    IEnumerator ShowPannelOpenButton()
    {
        yield return new WaitForSeconds(3f);
        PannelOpenButton.SetActive(true);
        yield return new WaitForSeconds(8f);
        PannelOpenButton.SetActive(false);
    }
    public void OnClickPannelOpenButton()
    {
        RetryPannel.SetActive(true);
    }
}
