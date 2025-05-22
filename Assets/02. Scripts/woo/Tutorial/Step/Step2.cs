using System.Collections;
using UnityEngine;

public class Step2 : TutorialStep
{
    private bool Step2_Clear;
    private Coroutine step2_co;

    public Step2(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {
        step2_co = coco.StartCoroutine(ScriptDirector());
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
        manager.PlayerScriptOff();
        manager.AiScriptOff();
        manager.NarrationScriptOff();
        manager.zone2.enabled = false;

        coco.StopCoroutine(step2_co);
        Debug.Log("Tutorial Step2 Clear");
    }

    IEnumerator ScriptDirector()
    {
        manager.AiScriptUpdate("�� �ڽ��� �ȴٸ� ���� ��� ����ٱ�?");
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_2");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.AiScriptOff();
        manager.NarrationScriptUpdate("���ʸ� �ݰ�[E], �þ߸� ��������. [1][2][3]");
    }
}
