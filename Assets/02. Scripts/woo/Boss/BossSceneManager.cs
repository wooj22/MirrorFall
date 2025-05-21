using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [Header("Boss Controll")]
    [SerializeField] private GameObject ai;
    [SerializeField] private GameObject Walls;
    [SerializeField] private int operationDeviceCount;

    [Header("Boss UI")]
    [SerializeField] private Text timerUI;
    [SerializeField] private Text aiText;
    [SerializeField] private Text narrationText;

    [Header("Data")]
    [SerializeField] private int BossSceneTimeLimit;
    private int currentTime;

    /// Boss Start (�ſ������� �Ծ��� �� ȣ��)
    public void BossStart()
    {
        aiText.text = "�ſ� ������ �� ��Ҵٰ�? �̹��� ������ ���̴�?\n���, ��Ʈ�� �ϳ� �ٰ�. \n�� �ȿ� ������ ��ġ�� �۵����� �ʴ´ٸ� �ⱸ�� ������ ������ ���� �ž�!";
        narrationText.text = "����� ��ġ�� �۵���Ű����";
        Invoke(nameof(TextOff), 6f);
    }

    public void TextOff()
    {
        aiText.text = "";
    }

    /// Device Operation Cheak
    public void DeviceOperationCheak()
    {
        operationDeviceCount++;

        // 4���� ��ġ�� ��� �۵� ���� ��
        if (operationDeviceCount >= 4)
        {
            ai.SetActive(true);
            Walls.SetActive(false);
            StartCoroutine(BossCountDown());
        }
    }

    /// Boos Count Down
    private IEnumerator BossCountDown()
    {
        while (currentTime < BossSceneTimeLimit)
        {
            currentTime++;
            timerUI.text = "�ſ�ӿ� ���������  " + (BossSceneTimeLimit - currentTime);
            yield return new WaitForSeconds(1f);
        }

        currentTime = 0;
        narrationText.text = "";
        GameManager.Instance.BossTimeEndDie();      // time end
    }
}
