using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [Header ("Boss UI")]
    [SerializeField] private Text timerUI;
    [SerializeField] private Text aiText;
    [SerializeField] private Text narrationText;

    [Header("Data")]
    [SerializeField] private int BossSceneTimeLimit;
    private int currentTime;
    
    // Boss Start (�ſ������� �Ծ��� �� ȣ��)
    public void BossStart() {
        StartCoroutine(BossCountDown());
        // ai Ȱ��ȭ ���� �߰�
        aiText.text = "�ſ� ������ �� ��Ҵٰ�? �̹��� ������ ���̴�?\n���, ��Ʈ�� �ϳ� �ٰ�. \n�� �ȿ� ������ ��ġ�� �۵����� �ʴ´ٸ� �ⱸ�� ������ ������ ���� �ž�!";
        narrationText.text = "����� ��ġ�� �۵���Ű����";
    }

    // Count Down
    private IEnumerator BossCountDown()
    {
        while(currentTime< BossSceneTimeLimit)
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
