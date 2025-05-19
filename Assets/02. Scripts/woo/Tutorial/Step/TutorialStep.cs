
/* 튜토리얼 Step Base Class */
using System.Collections;
using UnityEngine;

public abstract class TutorialStep
{
    public MonoBehaviour coco;       // 코루틴 쓸래 ~~
    public TutorialManager manager;

    public TutorialStep(MonoBehaviour co, TutorialManager manager)
    {
        this.coco = co;
        this.manager = manager;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual bool IsComplete() => false;
    public virtual void Exit() { }
}
