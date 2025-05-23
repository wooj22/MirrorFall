using System.Collections;
using UnityEngine;

public class Step4 : TutorialStep
{
    private bool Step4_Clear;
    private Transform applePos;

    public Step4(MonoBehaviour coco, TutorialManager tutorialManager) :
        base(coco, tutorialManager)
    { }

    public override void Enter()
    {
        manager.NarrationScriptUpdate("은신 기둥에서 [Shift]키를 누르고 적이 지나갈때까지 기다리세요");
    }

    public override void Update()
    {
        // 사과 찾기
        if (applePos == null) { applePos = GameObject.FindWithTag("Apple").transform; }
        if (applePos != null) manager.ai.MoveToPos(applePos);
        if (manager.player.step4_isHide) Step4_Clear = true;
    }

    public override bool IsComplete()
    {
        return Step4_Clear;
    }

    public override void Exit()
    {
        manager.PlayerScriptOff();
        manager.AiScriptOff();
        manager.NarrationScriptOff();
        manager.zone4.enabled = false;

        Debug.Log("Tutorial Step4 Clear");
    }
}
