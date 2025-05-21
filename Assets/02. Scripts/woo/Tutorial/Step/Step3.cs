using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Step3 : TutorialStep
{
    private bool Step3_Clear;
    private Coroutine step3_co;

    public Step3(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)  { }

    public override void Enter()
    {
        step3_co = coco.StartCoroutine(ScriptDirector());
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
        manager.PlayerScriptOff();
        manager.AiScriptOff();
        manager.NarrationScriptOff();
        manager.zone3.enabled = false;

        coco.StopCoroutine(step3_co);
        Debug.Log("Tutorial Step3 Clear");
    }

    IEnumerator ScriptDirector()
    {
        manager.PlayerScriptUpdate("������? ��... ��...?");
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_3");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.PlayerScriptOff();
        manager.AiScriptUpdate("������ �� ��� ���� ������ �����°Ŵ�?\n�ſ� ������ ������ �� �������ž�");
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_4");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.AiScriptOff();
        manager.PlayerScriptUpdate("���� ������ٰ�?");
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_5");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.PlayerScriptOff();
        manager.NarrationScriptUpdate("���� ����� �ݰ�[F], ����� ���� AI�� �����ϼ���. [1][2][3]");
        manager.apple.SetActive(true);
    }
}
