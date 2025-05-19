using System.Collections;
using UnityEngine;

public class Step3 : TutorialStep
{
    private bool Step3_Clear;

    public Step3(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)  { }

    public override void Enter()
    {
        manager.playerText.text = "�ʴ� ��?";
        manager.aiText.text = "������ �� ��� ���� ������ �����°Ŵ�?";
        manager.narrationText.text = "���� ����� �ݰ�[F], ����� ���� AI�� �����ϼ���. [1][2][3]";

        manager.apple.SetActive(true);
    }

    public override void Update()
    {
        if (manager.player.step3_isAppleThrow) Step3_Clear = true;
    }

    public override bool IsComplete()
    {
        return Step3_Clear;
    }

    public override void Exit()
    {
        manager.playerText.text = "";
        manager.aiText.text = "";
        manager.narrationText.text = "";

        manager.zone3.enabled = false;

        Debug.Log("Tutorial Step3 Clear");
    }
}
