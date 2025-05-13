using UnityEngine;
using static PlayerController;

public class Hide : BaseState
{
    public Hide(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Hide Enter");

        // hide set
        player.isHide = true;
        player.Invisible(player.invisibleTargetAlpha);
    }

    /// Change State
    public override void ChangeStateLogic()
    {
        if (!player.isInHideZone || !player.isHideKey)
        {
            // walk
            if (player.isMoveLKey || player.isMoveRKey || player.isMoveUpKey || player.isMoveDownKey)
            {
                player.ChangeState(PlayerState.Walk);
                return;
            }
            // idlew
            else
            {
                player.ChangeState(PlayerState.Idle);
                return;
            }
        }
    }

    /// Logic Update
    public override void UpdateLigic()
    {
        // animation blend
        player.ani.SetFloat("Vertical", player.lastDirY);

        // walk
        if (player.isMoveLKey || player.isMoveRKey || player.isMoveUpKey || player.isMoveDownKey)
        {
            player.ani.Play("Walk");
            player.sr.flipX = player.lastDirX == 1 ? false : true;
            player.rb.velocity = new Vector2(player.moveX, player.moveY) * player.speed;
        }
        // idle
        else
        {
            player.ani.Play("Idle");
            player.rb.velocity = Vector2.zero;
        }
    }

    /// Exit
    public override void Exit()
    {
        player.isHide = false;
        player.Invisible(player.originAlpha);

        Debug.Log("Hide Exit");
    }
}