
/* Ʃ�丮�� Step Base Class */
using System.Collections;
using UnityEngine;

public abstract class TutorialStep
{
    public MonoBehaviour coco;       // �ڷ�ƾ ���� ~~

    public TutorialStep(MonoBehaviour co) {  this.coco = co; }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual bool IsComplete() => false;
    public virtual void Exit() { }
}
