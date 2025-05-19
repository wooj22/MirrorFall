using System.Collections;
using UnityEngine;

public class Step7 : TutorialStep
{
    // Ai에게 죽으면 튜토리얼 재시작
    // 거울 워프시 별 로직 없이 씬 종료

    public Step7(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {
        manager.narrationText.text = "적에게 붙잡히면 시야가 줄어듭니다. \n거울을 찾아 이곳을 벗어나세요";
    }

    public override void Update()
    {
        manager.ai.Trace();    // ai 추격                       
        if (manager.player.step7_isDie) SceneSwitch.Instance.SceneReload();
    }

    public override bool IsComplete() { return false; }

    public override void Exit() { }
}
