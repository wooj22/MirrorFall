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
        manager.ai.transform.position = manager.aiRespawnPos.position;
    }

    public override void Update()
    {
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
