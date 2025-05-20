using System.Collections;
using UnityEngine;

public class Step1 : TutorialStep
{
    private bool Step1_Clear;
    private Coroutine step1_co;

    public Step1(MonoBehaviour coco, TutorialManager tutorialManager) : 
        base(coco, tutorialManager) { }

    public override void Enter() 
    {
        step1_co = coco.StartCoroutine(ScriptDirector());
    }

    public override void Update() 
    {
        if (manager.player.step1_isZone1) Step1_Clear = true;
    }

    public override bool IsComplete()
    {
        return Step1_Clear;
    }

    public override void Exit() 
    {
        manager.playerText.text = "";
        manager.narrationText.text = "";

        coco.StopCoroutine(step1_co);
        Debug.Log("Tutorial Step1 Clear");
    }

    IEnumerator ScriptDirector()
    {
        yield return new WaitForSeconds(2f);

        manager.playerText.text = "여긴.. 어디지?    성..?";
        SoundManager2.Instance.PlaySFX("Voice_Tutorial_1");
        yield return new WaitForSeconds(SoundManager2.Instance.GetPlayTimeSFX());

        manager.playerText.text = "";
        manager.narrationText.text = "거울 조각이 가리키는 방향으로 이동하세요\n(이동 : WASD)";
    }
}
