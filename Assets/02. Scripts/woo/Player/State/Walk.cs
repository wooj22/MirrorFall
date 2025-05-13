using UnityEngine;
using static PlayerController;

public class Walk : BaseState
{
    public Walk(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Walk Enter");

        // animation
        player.ani.Play("Walk");
    }

    /// Change State
    public override void ChangeStateLogic()
    {
        // idle
        if (!player.isMoveLKey && !player.isMoveRKey && !player.isMoveUpKey && !player.isMoveDownKey)
        {
            player.ChangeState(PlayerState.Idle);
            return;
        }

        // hide
        if (player.isInHideZone && player.isHideKey)
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

        // filp, las Dir X
        player.sr.flipX = player.lastDirX == 1 ? false : true;

        // move
        player.rb.velocity = new Vector2(player.moveX, player.moveY) * player.curSpeed;
    }

    /// Exit
    public override void Exit()
    {
        Debug.Log("Walk Exit");
    }
}