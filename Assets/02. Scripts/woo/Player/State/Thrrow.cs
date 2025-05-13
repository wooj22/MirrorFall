using UnityEngine;
using static PlayerController;

public class Thrrow : BaseState
{
    public Thrrow(PlayerController player) : base(player) { }

    /// Enter
    public override void Enter()
    {
        Debug.Log("Thrrow Enter");

        // 던지기 준비 falg
        player.ani.enabled = false;
        player.isThrrow = true;

        player.lineRenderer.enabled = true; // LineRenderer 켜기

        // velocity zero
        player.rb.velocity = Vector2.zero;
    }

    /// Change State
    public override void ChangeStateLogic()
    {
        // 사과 던지고 나서 Idle
        if (!player.isThrrow)
        {
            // 현재 애니메이션이 끝났을때 전환
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

            // 포물선
            DrawParabola(mouseWorldPos);

            // 좌클릭으로 Apple 던지기
            if (Input.GetMouseButtonDown(0))
            {
                player.ani.enabled = true;
                player.ani.Play("Throw");

                player.AppleThrrow();
                player.isThrrow = false;
            }
        }
    }

    /// Exit
    public override void Exit()
    {
        player.lineRenderer.enabled = false; // LineRenderer 끄기

        Debug.Log("Thrrow Exit");
    }

    // 포물선 그리기
    private void DrawParabola(Vector2 targetPos)
    {
        Vector2 startPos = player.appleSpawnPoint.position;

        // 거리와 방향 계산
        float distance = Vector2.Distance(startPos, targetPos);
        float height = Mathf.Max(1f, distance / 2f); // 높이를 거리 기반으로 조절 (너무 낮지 않게 최소값 1)

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

    // 포물선 포인트 계산
    private Vector2 GetParabolaPoint(Vector2 start, Vector2 end, float height, float t)
    {
        // 선형 보간
        Vector2 mid = Vector2.Lerp(start, end, t);

        // y 보정: 포물선 공식
        float parabola = 4 * height * t * (1 - t); // 0~1에서 최대값이 height
        mid.y += parabola;

        return mid;
    }
}