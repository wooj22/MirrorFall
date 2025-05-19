using System.Collections;
using UnityEngine;

public class Step1 : TutorialStep
{
    public override void Enter() 
    {
        coco.StartCoroutine(Test());
    }

    public override void Update() 
    { 
    
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override void Exit() 
    { 

    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("FSM도 코루틴 ㄱㄴ");
    }
}
