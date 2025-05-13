using UnityEngine;
using static PlayerController;

public class Lure : BaseState
{
    public Lure(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Lure Enter");

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
        Debug.Log("Lure Exit");
    }
}