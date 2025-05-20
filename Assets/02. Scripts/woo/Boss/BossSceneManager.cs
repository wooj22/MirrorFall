using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [SerializeField] private int BossSceneTimeLimit;
    private int currentTime;
    [SerializeField] private Text timerUI;

    public void BossCountDown() { StartCoroutine(BossCountDownCo()); }

    private IEnumerator BossCountDownCo()
    {
        while(currentTime< BossSceneTimeLimit)
        {
            currentTime++;
            UpdateTimeUI();
            yield return new WaitForSeconds(1f);
        }

        currentTime = 0;
        GameManager.Instance.BossTimeEndDie();      // time end
    }

    private void UpdateTimeUI()
    {
        timerUI.text = "거울속에 같히기까지  " + (BossSceneTimeLimit-currentTime);
    }
}
