using System.Collections;
using UnityEngine;

public class Step5 : TutorialStep
{
    private bool Step5_Clear;

    public Step5(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {

    }

    public override void Update()
    {
        manager.ai.MoveToPos(manager.aiRespawnPos);
        if (manager.player.step5_isZone5) Step5_Clear = true;
    }

    public override bool IsComplete()
    {
        return Step5_Clear;
    }

    public override void Exit()
    {
        Debug.Log("Tutorial Step5 Clear");
    }
}
