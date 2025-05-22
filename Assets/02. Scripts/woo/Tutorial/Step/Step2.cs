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
        manager.AiScriptUpdate("네 자신이 싫다면 내가 대신 살아줄까?");
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_2");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.AiScriptOff();
        manager.NarrationScriptUpdate("양초를 줍고[E], 시야를 밝히세요. [1][2][3]");
    }
}
