
/* Ʃ�丮�� Step Base Class */
using System.Collections;
using UnityEngine;

public abstract class TutorialStep
{
    public MonoBehaviour coco;       // �ڷ�ƾ ���� ~~
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual bool IsComplete() => false;
    public virtual void Exit() { }
}
