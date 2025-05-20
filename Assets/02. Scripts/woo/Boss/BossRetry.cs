using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRetry : MonoBehaviour
{
    [SerializeField] GameObject retryPannel;
    public void OnClickBossRetry()
    {
        GameManager.Instance.BossRetry();
    }

    public void RetryPannelOn()
    {
        retryPannel.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void RetryPannelOff()
    {
        retryPannel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
