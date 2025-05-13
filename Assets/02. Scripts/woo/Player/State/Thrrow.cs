using UnityEngine;
using static PlayerController;

public class Thrrow : BaseState
{
    public Thrrow(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Thrrow Enter");

        // ������ �غ� falg
        player.ani.enabled = false;
        player.isThrrow = true;

        // velocity zero
        player.rb.velocity = Vector2.zero;
    }

    /// Change State
    public override void ChangeStateLogic()
    {
        // ��� ������ ���� Idle
        if (!player.isThrrow)
        {
            player.ChangeState(PlayerState.Idle);
        }
    }

    /// Logic Update
    public override void UpdateLigic()
    {
        if (player.isThrrow)
        {
            // mouse pos
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            Vector3 dir = mouseWorldPos - player.transform.position;

            // Top/ Down Sprite update set
            if (dir.y >= 0) player.sr.sprite = player.appleOnUpSprite;
            else player.sr.sprite = player.appleOnDownSprite;

            // Left/ Right Filp
            player.sr.flipX = dir.x < 0;

            // ��Ŭ������ Apple ������
            if (Input.GetMouseButtonDown(0))
            {
                player.ani.enabled = true;
                player.ani.Play("Throw");

                // TODO: ��� ������ �Լ� ȣ��
                Debug.Log("��� ����");

                player.isThrrow = false;
            }
        }
    }

    /// Exit
    public override void Exit()
    {
        Debug.Log("Thrrow Exit");
    }
}