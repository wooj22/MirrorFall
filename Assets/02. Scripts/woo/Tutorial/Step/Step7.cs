using System.Collections;
using UnityEngine;

public class Step7 : TutorialStep
{
    // Ai���� ������ Ʃ�丮�� �����
    // �ſ� ������ �� ���� ���� �� ����

    public Step7(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {
        manager.narrationText.text = "������ �������� �þ߰� �پ��ϴ�. \n�ſ��� ã�� �̰��� �������";
    }

    public override void Update()
    {
        manager.ai.Trace();    // ai �߰�                       
        if (manager.player.step7_isDie) SceneSwitch.Instance.SceneReload();
    }

    public override bool IsComplete() { return false; }

    public override void Exit() { }
}
