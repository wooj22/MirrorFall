
/* Æ©Åä¸®¾ó Step Base Class */
public abstract class TutorialStep
{
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual bool IsComplete() => false;
    public virtual void Exit() { }
}
