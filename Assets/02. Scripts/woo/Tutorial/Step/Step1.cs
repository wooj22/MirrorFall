using System.Collections;
using UnityEngine;

public class Step1 : TutorialStep
{
    private bool Step1_Clear;

    public Step1(MonoBehaviour coco, TutorialManager tutorialManager) : 
        base(coco, tutorialManager) { }

    public override void Enter() 
    {
        manager.playerText.text = "���� ����? ���� �ƴϾ�";
        manager.narrationText.text = "�ſ� ������ ����Ű�� �������� �̵��ϼ���\n(�̵� : WASD)";
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
    //    Debug.Log("FSM�� �ڷ�ƾ ����");
    //}
}
