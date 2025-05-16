using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [SerializeField] private int BossSceneTimeLimit;
    private int currentTime;
    [SerializeField] private Text timerUI;
    [SerializeField] private GameObject retryPannel;

    private void Start()
    {
        Time.timeScale = 1;
        StartCoroutine(Boss());
        retryPannel.SetActive(false);
    }

    private IEnumerator Boss()
    {
        while(currentTime< BossSceneTimeLimit)
        {
            currentTime++;
            UpdateTimeUI();
            yield return new WaitForSeconds(1f);
        }

        currentTime = 0;
        RetryPannelOn();
    }

    private void UpdateTimeUI()
    {
        timerUI.text = "거울속에 같히기까지  " + (BossSceneTimeLimit-currentTime);
    }

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
