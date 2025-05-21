using System.Collections;
using UnityEngine;

public class Step4 : TutorialStep
{
    private bool Step4_Clear;
    private Transform applePos;

    public Step4(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {
        manager.NarrationScriptUpdate("���� ��տ��� [Shift]Ű�� ������ ���� ������������ ��ٸ�����");
    }

    public override void Update()
    {
        // ��� ã��
        if (applePos == null) { applePos = GameObject.FindWithTag("Apple").transform; }
        manager.ai.MoveToPos(applePos);
        if (manager.player.step4_isHide) Step4_Clear = true;
    }

    public override bool IsComplete()
    {
        return Step4_Clear;
    }

    public override void Exit()
    {
        manager.PlayerScriptOff();
        manager.AiScriptOff();
        manager.NarrationScriptOff();
        manager.zone4.enabled = false;

        Debug.Log("Tutorial Step4 Clear");
    }
}
