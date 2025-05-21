using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToPannelOpen : MonoBehaviour
{
    public GameObject RetryPannel;
    void Start()
    {
        RetryPannel.SetActive(false);
    }
    public void OnClickPannelOpenButton()
    {
        RetryPannel.SetActive(true);
    }
}
