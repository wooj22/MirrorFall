using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [Header("Boss Controll")]
    [SerializeField] private GameObject Walls;
    [SerializeField] private int operationDeviceCount;


    [Header("Boss UI")]
    [SerializeField] private Text timerUI;
    [SerializeField] private Text aiText;
    [SerializeField] private Text narrationText;

    [Header("Data")]
    [SerializeField] private int BossSceneTimeLimit;
    private int currentTime;

    /// Boss Start (거울조각을 먹었을 때 호출)
    public void BossStart()
    {
        // ai 활성화 여기 추가
        aiText.text = "거울 조각을 다 모았다고? 이번에 잡히면 끝이다?\n대신, 힌트를 하나 줄게. \n방 안에 숨겨진 장치를 작동하지 않는다면 출구는 영원히 보이지 않을 거야!";
        narrationText.text = "기둥의 장치를 작동시키세요";
    }

    /// 장치 발동 체크
    public void DeviceOperationCheak()
    {
        operationDeviceCount++;
        if (operationDeviceCount >= 4)
        {
            Walls.SetActive(false);
            StartCoroutine(BossCountDown());
        }
    }

    /// Count Down
    private IEnumerator BossCountDown()
    {
        while (currentTime < BossSceneTimeLimit)
        {
            currentTime++;
            timerUI.text = "거울속에 갇히기까지  " + (BossSceneTimeLimit - currentTime);
            yield return new WaitForSeconds(1f);
        }

        currentTime = 0;
        narrationText.text = "";
        GameManager.Instance.BossTimeEndDie();      // time end
    }
}
