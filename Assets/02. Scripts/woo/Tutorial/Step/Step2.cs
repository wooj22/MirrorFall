using System.Collections;
using UnityEngine;

public class Step2 : TutorialStep
{
    private bool Step2_Clear;

    public Step2(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {
        manager.aiText.text = "�� �ڽ��� �ȴٸ� ���� ��� ����ٱ�?";
        manager.playerText.text = "��, ������?";
        manager.narrationText.text = "��� ����� �ݰ�[F], �þ߸� ��������. [1][2][3]";
    }

    public override void Update()
    {
        if (manager.player.step2_isBright) Step2_Clear = true;
    }

    public override bool IsComplete()
    {
        return Step2_Clear;
    }

    public override void Exit()
    {
        manager.aiText.text = "";
        manager.playerText.text = "";
        manager.narrationText.text = "";
    }
}
