using UnityEngine;
using static PlayerController;

public class Idle : BaseState
{
    public Idle(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Idle Enter");

        // animation
        player.ani.Play("Idle");

        // filp, las Dir X
        player.sr.flipX = player.lastDirX == 1 ? false : true;

        // velocity zero
        player.rb.velocity = Vector2.zero;
    }

    /// Change State
    public override void ChangeStateLogic()
    {
        // walk 
        if (player.isMoveLKey || player.isMoveRKey || player.isMoveUpKey || player.isMoveDownKey)
        {
            player.ChangeState(PlayerState.Walk);
            return;
        }

        // hide
        if(player.isInHideZone && player.isHideKey)
        {
            player.ChangeState(PlayerState.Hide);
            return;
        }
    }

    /// Logic Update
    public override void UpdateLigic()
    {
        // animation blend
        player.ani.SetFloat("Vertical", player.lastDirY);
    }

    /// Exit
    public override void Exit()
    {
        Debug.Log("Idle Exit");
    }
}