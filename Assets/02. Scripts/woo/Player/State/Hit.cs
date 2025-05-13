using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Hit : BaseState
{
    public Hit(PlayerController player) : base(player) { }
    private float timer;

    /// Enter
    public override void Enter()
    {
        Debug.Log("Hit Enter");

        // falg
        player.isHit = true;

        // 라이트 연출
        player.flashLight.HitLightLogic();

        // velocity zero
        player.rb.velocity = Vector2.zero;
    }

    /// Change State
    public override void ChangeStateLogic() { }

    /// Logic Update
    public override void UpdateLigic() 
    {
        timer += Time.deltaTime;
        if (timer > player.hitDurationTime)
        {
            // ide
            player.ChangeState(PlayerState.Idle);
            timer = 0;
        }
    }

    /// Exit
    public override void Exit()
    {
        // flag
        player.isHit = false;

        Debug.Log("Hit Exit");
    }
}