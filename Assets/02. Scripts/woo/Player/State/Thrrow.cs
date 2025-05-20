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

        player.lineRenderer.enabled = true; // LineRenderer �ѱ�

        // velocity zero
        player.rb.velocity = Vector2.zero;
    }

    /// Change State
    public override void ChangeStateLogic()
    {
        // ��� ������ ���� Idle
        if (!player.isThrrow)
        {
            // ���� �ִϸ��̼��� �������� ��ȯ
            AnimatorStateInfo stateInfo = player.ani.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Throw") && stateInfo.normalizedTime >= 1f)
            {
                player.ChangeState(PlayerState.Idle);
            }
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

            // ������
            DrawParabola(mouseWorldPos);

            // ��Ŭ������ Apple ������
            if (Input.GetMouseButtonDown(0))
            {
                // ���� �ִϸ��̼ǿ��� ������ ����ǵ���
                int testLastDirX = dir.x>=0 ? 1 : -1;
                int testLastDirY = dir.y >= 0 ? 1 : -1;

                player.lastDirX = testLastDirX;
                player.lastDirY = testLastDirY;     

                // ������ �ִϸ��̼�
                player.ani.enabled = true;
                player.ani.SetFloat("Vertical", player.lastDirY);
                player.ani.Play("Throw");

                player.AppleThrrowSkill();
                player.isThrrow = false;

                // sound
                SoundManager2.Instance.PlaySFX("SFX_Apple_throw");
            }
        }
    }

    /// Exit
    public override void Exit()
    {
        player.lineRenderer.enabled = false; // LineRenderer ����
        Debug.Log("Thrrow Exit");
    }

    /// ������ �׸���
    private void DrawParabola(Vector2 targetPos)
    {
        Vector2 startPos = player.appleSpawnPoint.position;

        // �Ÿ��� ���� ���
        float distance = Vector2.Distance(startPos, targetPos);
        float height = Mathf.Max(1f, distance / 2f); // ���̸� �Ÿ� ������� ���� (�ʹ� ���� �ʰ� �ּҰ� 1)

        int resolution = 30;
        Vector3[] points = new Vector3[resolution + 1];

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector2 point = GetParabolaPoint(startPos, targetPos, height, t);
            points[i] = point;
        }

        player.lineRenderer.positionCount = points.Length;
        player.lineRenderer.SetPositions(points);
    }

    /// ������ ����Ʈ ���
    private Vector2 GetParabolaPoint(Vector2 start, Vector2 end, float height, float t)
    {
        // ���� ����
        Vector2 mid = Vector2.Lerp(start, end, t);

        // y ����: ������ ����
        float parabola = 4 * height * t * (1 - t); // 0~1���� �ִ밪�� height
        mid.y += parabola;

        return mid;
    }
}