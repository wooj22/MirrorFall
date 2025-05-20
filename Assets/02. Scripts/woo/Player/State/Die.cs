using Unity.VisualScripting;
using UnityEngine;

public class Die : BaseState
{
    public Die(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Die Enter");

        // controll
        player.isDie = true;
        player.rb.velocity = Vector2.zero;

        // animation
        player.ani.Play("Die");
        player.ani.SetFloat("Vertical", player.lastDirY);
        if (player.moveX < 0) { player.sr.flipX = true; }
        else if (player.moveX > 0) { player.sr.flipX = false; }

        // sound
        SoundManager2.Instance.PlaySFX("SFX_Grimhilde_Die");
        SoundManager2.Instance.PlayOneShotBGM("BGM_Gameover");

        player.Die();
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