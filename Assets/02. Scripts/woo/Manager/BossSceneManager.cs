using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [SerializeField] private int BossSceneTimeLimit;
    private int currentTime;
    [SerializeField] private Text timerUI;

    private void Start()
    {
        StartCoroutine(Boss());
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
    }

    private void UpdateTimeUI()
    {
        timerUI.text = "거울속에 같히기까지  " + (BossSceneTimeLimit-currentTime);
    }
}
