using Unity.VisualScripting;
using UnityEngine;

public class Die : BaseState
{
    public Die(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Die Enter");

        // flag
        player.isDie = true;

        // animation
        player.ani.Play("Die");

        // animation blend
        player.ani.SetFloat("Vertical", player.lastDirY);

        // filp, las Dir X
        if (player.moveX < 0) { player.sr.flipX = true; }         // left
        else if (player.moveX > 0) { player.sr.flipX = false; }   // right

        // velocity zero
        player.rb.velocity = Vector2.zero;

        // TODO :: Die 연출 및 로직 추가 (지금은 애니메이션 재생 후 destory
        //AnimatorStateInfo stateInfo = player.ani.GetCurrentAnimatorStateInfo(0);
        //Destroy(player.gameObject, stateInfo.length + 2);
    }

    /// Change State
    public override void ChangeStateLogic() { }

    /// Logic Update
    public override void UpdateLigic() { }

    /// Exit
    public override void Exit() { }
}