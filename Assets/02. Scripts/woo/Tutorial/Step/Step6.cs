using System.Collections;
using UnityEngine;

public class Step6 : TutorialStep
{
    private bool Step6_Clear;
    private Vector2 pos;

    public Step6(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {
        pos = manager.player.transform.position;
    }

    public override void Update()
    {
        manager.player.transform.position = pos;        // 못움직임
        manager.ai.Trace();                             // ai 추격
        Step6_Clear = manager.player.step6_isHit;       
    }

    public override bool IsComplete()
    {
        return Step6_Clear;
    }

    public override void Exit()
    {
        Debug.Log("Tutorial Step6 Clear");
    }
}
