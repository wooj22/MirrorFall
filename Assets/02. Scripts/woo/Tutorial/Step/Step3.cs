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
        manager.PlayerScriptUpdate("누구야? 넌... 나...?");
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_3");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.PlayerScriptOff();
        manager.AiScriptUpdate("잡히면 너 대신 내가 밖으로 나가는거다?\n거울 조각이 없으면 넌 무너질거야");
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_4");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.AiScriptOff();
        manager.PlayerScriptUpdate("내가 사라진다고?");
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_5");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.PlayerScriptOff();
        manager.NarrationScriptUpdate("빨간 사과를 줍고[F], 사과를 던져 AI를 유인하세요. [1][2][3]");
        manager.apple.SetActive(true);
    }
}
