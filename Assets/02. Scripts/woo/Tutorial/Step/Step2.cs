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
        manager.playerText.text = "누, 누구야?";
        manager.aiText.text = "네 자신이 싫다면 내가 대신 살아줄까?";
        manager.narrationText.text = "노란 사과를 줍고[F], 시야를 밝히세요. [1][2][3]";
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
        manager.playerText.text = "";
        manager.aiText.text = "";
        manager.narrationText.text = "";
    }
}
