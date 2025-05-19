using System.Collections;
using UnityEngine;

public class Step4 : TutorialStep
{
    private bool Step4_Clear;

    public Step4(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {
        manager.narrationText.text = "���� ��տ��� [Shift]Ű�� ������ ���� ������������ ��ٸ�����";
    }

    public override void Update()
    {
        if (manager.player.step4_isHide) Step4_Clear = true;
    }

    public override bool IsComplete()
    {
        return Step4_Clear;
    }

    public override void Exit()
    {
        manager.playerText.text = "";
        manager.aiText.text = "";
        manager.narrationText.text = "";

        manager.zone4.enabled = false;

        Debug.Log("Tutorial Step4 Clear");
    }
}
