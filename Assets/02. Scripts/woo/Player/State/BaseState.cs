using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    protected PlayerController player;

    public BaseState(PlayerController player)
    {
        this.player = player;
    }

    public virtual void Enter() { }
    public virtual void ChangeStateLogic() { }
    public virtual void UpdateLigic() { }
    public virtual void Exit() { }
}
