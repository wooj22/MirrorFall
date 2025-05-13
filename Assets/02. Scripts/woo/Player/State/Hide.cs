using UnityEngine;
using static PlayerController;

public class Hide : BaseState
{
    public Hide(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Hide Enter");

    }

    /// Change State
    public override void ChangeStateLogic()
    {

    }

    /// Logic Update
    public override void UpdateLigic()
    {

    }

    /// Exit
    public override void Exit()
    {
        Debug.Log("Hide Exit");
    }
}