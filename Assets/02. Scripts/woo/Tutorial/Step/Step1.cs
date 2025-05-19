using System.Collections;
using UnityEngine;

public class Step1 : TutorialStep
{
    private bool Step1_Clear;

    public Step1(MonoBehaviour coco, TutorialManager tutorialManager) : 
        base(coco, tutorialManager) { }

    public override void Enter() 
    {
        manager.playerText.text = "여긴 어디야? 방이 아니야";
        manager.narrationText.text = "거울 조각이 가리키는 방향으로 이동하세요\n(이동 : WASD)";
    }

    public override void Update() 
    {
        if (manager.player.isZone1) Step1_Clear = true;
    }

    public override bool IsComplete()
    {
        return Step1_Clear;
    }

    public override void Exit() 
    {
        manager.playerText.text = "";
        manager.narrationText.text = "";
    }

    //coco.StartCoroutine(Test());
    //IEnumerator Test()
    //{
    //    yield return new WaitForSeconds(1);
    //    Debug.Log("FSM도 코루틴 ㄱㄴ");
    //}
}
