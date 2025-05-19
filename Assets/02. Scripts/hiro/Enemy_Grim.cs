using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy_Grim : MonoBehaviour
{
    // AI
    public string anim_cur = "Idle";
    public float speed = 2f;            // 이동 속도
    public float findDistance = 5f;
    public float missDistance = 7f;
    public float applefindDistance = 10f;
    public float applemissDistance = 15f;
    public float attackDistance = 2f;   // 공격 거리
    public float eatDistance = 2f;
    public Vector2 startPos;
    private int upSorting = 150;
    private int downSorting = 50;

    // AI 이동
    public GameObject allPatrolPoints;
    public GameObject allBypassPoints;
    private List<Transform> patrolPoints = new List<Transform>();
    private List<Transform> bypassPoints = new List<Transform>();
    private int currentPointIndex = 0;

    private bool playerfind = false;
    private bool applefind = false;
    private bool isReturning = false;
    private bool isAttacking = false;
    private bool isEating = false;
    private float playerdistance;
    private float appledistance;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator ani;
    private SpriteRenderer spr;
    private SpriteRenderer playerSpr;
    private GameObject player;
    private GameObject[] Apples;
    private GameObject nearestApple;

    private Vector2 playerPos;
    private Vector2 applePos;

    // map
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        playerSpr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (allPatrolPoints != null)
        {
            foreach (Transform child in allPatrolPoints.transform)
            {
                patrolPoints.Add(child);
            }
        }

        if (allBypassPoints != null)
        {
            foreach (Transform child in allBypassPoints.transform)
            {
                bypassPoints.Add(child);
            }
        }
    }

    void Update()
    {
        PlayerCheck();
        AppleCheck();
        Playani();
        MovetoApple();
        NormalMove();
        if (!isAttacking && !isReturning)
        {
            Attack();
        }
        if (!isReturning && !isEating)
        {
            Eating();
        }

    }

    // 플레이어 탐색
    void PlayerCheck()
    {
        if (player != null)
        {
            playerPos = player.transform.position;
        }

        playerdistance = Vector2.Distance(transform.position, playerPos);
    }

    // 사과 탐색
    void AppleCheck()
    {
        Apples = GameObject.FindGameObjectsWithTag("Apple");
        if (Apples.Length > 0)
        {
            float minDistance = float.MaxValue;
            GameObject closest = null;

            foreach (GameObject Apple in Apples)
            {
                if (!Apple.GetComponent<Apple>().isGround) return;

                float dist = Vector2.Distance(transform.position, Apple.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = Apple;
                }
            }

            if (closest != null)
            {
                nearestApple = closest;
                applePos = nearestApple.transform.position;
                appledistance = minDistance;
            }
        }
        else
        {
            nearestApple = null;
            applePos = Vector2.positiveInfinity;
            appledistance = float.MaxValue;
        }
    }

    // 적 AI 기본 이동
    void NormalMove()
    {
        if (isReturning || applefind || isAttacking) return;

        bool isPlayerHidden = player.GetComponent<PlayerController>().isHide;

        if (!playerfind)
        {
            if (playerdistance < findDistance && !isPlayerHidden && PlayerInSight() && IsPathClearBox(transform.position, playerPos))
            {
                playerfind = true;
            }
            else
            {
                if (patrolPoints.Count == 0) return;
                Vector2 nextPos = patrolPoints[currentPointIndex].position;
                if (IsPathClearBox(transform.position, nextPos))
                {
                    Vector2 dirToNext = (nextPos - (Vector2)transform.position).normalized;
                    rb.velocity = dirToNext * speed;
                }
                else
                {
                    Transform bypassNext = FindClosestBypassPoint(transform.position, nextPos);

                    if (bypassNext != null)
                    {
                        Vector2 bypassPos = bypassNext.position;
                        Vector2 dirToBypassNext = (bypassPos - (Vector2)transform.position).normalized;
                        rb.velocity = dirToBypassNext * speed;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                }

                // 순찰 지점 갱신
                if (Vector2.Distance(transform.position, nextPos) < 0.2f)
                {
                    currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;

                    int attempts = 0;
                    while (patrolPoints[currentPointIndex] == null && attempts < patrolPoints.Count)
                    {
                        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
                        attempts++;
                    }
                }
            }
        }
        else
        {
            // 플레이어 추적
            if (playerdistance > missDistance)
            {
                rb.velocity = Vector2.zero;
                playerfind = false;

                SetNearestPatrolPoint();
            }
            else
            {
                if (IsPathClearBox(transform.position, playerPos))
                {
                    // 플레이어 추적
                    Vector2 dirToPlayer = (playerPos - (Vector2)transform.position).normalized;
                    rb.velocity = dirToPlayer * speed;
                }
                else
                {
                    Transform bypassNext = FindClosestBypassPoint(transform.position, playerPos);

                    if (bypassNext != null)
                    {
                        Vector2 bypassPos = bypassNext.position;
                        Vector2 dirToBypassNext = (bypassPos - (Vector2)transform.position).normalized;
                        rb.velocity = dirToBypassNext * speed;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }

                }
            }
        }
    }

    // 플레이어와 멀어지면 가장 가까운 페트롤포인트로 이동
    void SetNearestPatrolPoint()
    {
        float minDist = float.MaxValue;
        int nearestIndex = 0;

        for (int i = 0; i < patrolPoints.Count; i++)
        {
            float dist = Vector2.Distance(transform.position, patrolPoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestIndex = i;
            }
        }

        currentPointIndex = nearestIndex;

    }

    // 사과로 이동
    void MovetoApple()
    {
        if (isReturning) return;
        if (nearestApple == null)
        {
            applefind = false;
            return;
        }

        if (appledistance < applefindDistance)
        {
            applefind = true;
        }

        if (applefind)
        {
            if (IsPathClearBox(transform.position, applePos))
            {
                Vector2 dirToApole = (applePos - (Vector2)transform.position).normalized;
                rb.velocity = dirToApole * speed;
            }
            else
            {
                Transform bypassNext = FindClosestBypassPoint(transform.position, applePos);

                if (bypassNext != null)
                {
                    Vector2 bypassPos = bypassNext.position;
                    Vector2 dirToBypassNext = (bypassPos - (Vector2)transform.position).normalized;
                    rb.velocity = dirToBypassNext * speed;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
            }

            if (appledistance > applemissDistance)
            {
                applefind = false;
            }
        }
    }

    // 플레이 애니메이션
    void Playani()
    {
        Vector2 velocity = rb.velocity;
        spr.sortingOrder = velocity.y <= 0 ? downSorting : upSorting;

        spr.flipX = velocity.x < 0;
        SetAnimation(velocity.y >= 0 ? "RightTop" : "RightBot");
    }

    // 공격
    void Attack()
    {
        bool isPlayerHidden = player.GetComponent<PlayerController>().isHide;

        if (isPlayerHidden) return;

        if (playerdistance <= attackDistance)
        {
            player.GetComponent<PlayerController>().Hit("K");
            rb.velocity = Vector2.zero;
            col.enabled = false;
            isReturning = true;
            isAttacking = true;
            Invoke(nameof(ResetToStart), 0.5f);
        }
    }

    // 사과를 먹게 됨
    void Eating()
    {
        if (appledistance <= eatDistance && !isEating && !isReturning)
        {
            rb.velocity = Vector2.zero;
            col.enabled = false;
            isReturning = true;
            isEating = true;
            if (nearestApple != null)
            {
                Destroy(nearestApple);
                nearestApple = null;
                applefind = false;
                appledistance = float.MaxValue;
            }
            Invoke(nameof(ReGoing), 0.5f);
        }
    }

    // 리셋
    private void ResetToStart()
    {
        Vector2 newPos = GetRandomPositionAwayFromPlayer(15f, 20f);
        transform.position = newPos;
        playerfind = false;
        col.enabled = true;
        isAttacking = false;
        isReturning = false;
    }

    // 사과를 먹게 되면 다시 플레이어를 탐색 시작
    private void ReGoing()
    {
        col.enabled = true;
        isReturning = false;
        isEating = false;
        playerfind = true;
    }

    // 플레이어를 감지하는 시야
    private bool PlayerInSight()
    {
        if (player == null) return false;
        Vector2 toPlayer = (playerPos - (Vector2)transform.position).normalized;
        Vector2 forward = rb.velocity.normalized;
        float angle = Vector2.Angle(forward, toPlayer);
        return angle < 55f;
    }

    // 레이캐스트, 적이랑 다음 이동할 위치 사이를 확인
    //private bool IsPathClearBox(Vector2 from, Vector2 to)
    //{
    //    // 콜라이더 정보
    //    BoxCollider2D box = GetComponent<BoxCollider2D>();
    //    Bounds bounds = box.bounds;
    //    Vector2 center = bounds.center;

    //    // 꼭짓점들 (왼쪽 위, 오른쪽 위, 오른쪽 아래, 왼쪽 아래)
    //    Vector2[] startPoints = new Vector2[5];
    //    startPoints[0] = bounds.center; // 중앙
    //    startPoints[1] = new Vector2(bounds.min.x, bounds.max.y); // 왼쪽 위
    //    startPoints[2] = new Vector2(bounds.max.x, bounds.max.y); // 오른쪽 위
    //    startPoints[3] = new Vector2(bounds.max.x, bounds.min.y); // 오른쪽 아래
    //    startPoints[4] = new Vector2(bounds.min.x, bounds.min.y); // 왼쪽 아래

    //    Vector2 dir = (to - center).normalized;
    //    float dist = Vector2.Distance(center, to);
    //    float offset = 0.1f;

    //    foreach (Vector2 start in startPoints)
    //    {
    //        Vector2 offsetStart = start + dir * offset;

    //        RaycastHit2D hit = Physics2D.Raycast(offsetStart, dir, dist, LayerMask.GetMask("Wall"));
    //        Debug.DrawRay(offsetStart, dir * dist, Color.red); // 디버깅용

    //        if (hit.collider != null)
    //            return false; // 하나라도 막히면 false
    //    }

    //    return true;
    //}

    private bool IsPathClearBox(Vector2 from, Vector2 to)
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 center = box.bounds.center;
        float raySize = 1.1f;

        Vector2 dir = (to - from).normalized;
        float dist = Vector2.Distance(from, to);

        // 월드 스케일 반영한 실제 크기 계산
        Vector2 worldSize = Vector2.Scale(box.size, transform.lossyScale) * raySize;

        // BoxCast의 중심 위치를 콜라이더 중심으로 맞춤
        // (from과 center가 다르면 center를 기준으로 하는 게 맞음)
        Vector2 castOrigin = center;

        RaycastHit2D hit = Physics2D.BoxCast(
            castOrigin, worldSize, 0f, dir, dist, LayerMask.GetMask("Wall"));

        Debug.DrawRay(castOrigin, dir * dist, Color.yellow);

        return hit.collider == null;
    }


    // 가장 가까운 우회위치 탐색
    private Transform FindClosestBypassPoint(Vector2 from, Vector2 to)
    {
        Transform bestPoint = null;
        float minDistanceToTarget = float.MaxValue;

        foreach (var point in bypassPoints)
        {
            if (point == null || !IsPathClearBox(from, point.position)) continue;

            bool hasClearPathToTarget = !Physics2D.Raycast(
                point.position,
                (to - (Vector2)point.position).normalized,
                Vector2.Distance(point.position, to),
                LayerMask.GetMask("Wall")
            );

            // 첫 번째 패스: 타겟까지 길이 뚫려있는 우회 포인트
            // 두 번째 패스: 그냥 가까운 우회 포인트
            if (bestPoint == null || hasClearPathToTarget || bestPoint != null && !hasClearPathToTarget)
            {
                float distToTarget = Vector2.Distance(point.position, to);

                if (distToTarget < minDistanceToTarget &&
                    (hasClearPathToTarget || bestPoint == null))
                {
                    minDistanceToTarget = distToTarget;
                    bestPoint = point;
                }
            }
        }

        return bestPoint;
    }


    // 애니메이션 재생
    private void SetAnimation(string anim)
    {
        if (anim_cur == anim) return;
        anim_cur = anim;
        ani.Play(anim);
    }

    // 맵을 기준으로 랜덤 좌표로 이동
    Vector2 GetRandomPositionAwayFromPlayer(float minDistance, float maxDistance)
    {
        Vector2 randomPos;
        int safety = 0;

        do
        {
            float angle = Random.Range(0f, 2f * Mathf.PI);
            float distance = Random.Range(minDistance, maxDistance);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            randomPos = (Vector2)player.transform.position + offset;

            // 맵 경계 체크
            randomPos.x = Mathf.Clamp(randomPos.x, minX, maxX);
            randomPos.y = Mathf.Clamp(randomPos.y, minY, maxY);

            safety++;
            if (safety > 100) break; // 무한 루프 방지
        }
        while (Vector2.Distance(randomPos, player.transform.position) < minDistance);

        return randomPos;
    }

    // 기즈모 추가
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // applefindDistance 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, applefindDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, findDistance);

        // 120도 시야각 시각화 (±60도)
        Gizmos.color = Color.yellow;

        Vector2 forward = Application.isPlaying ? rb.velocity.normalized : Vector2.right;
        if (forward == Vector2.zero) forward = Vector2.right; // 디폴트 방향

        float halfAngle = 55f;

        // 시야각 끝 방향 계산
        Vector2 leftDir = Quaternion.Euler(0, 0, -halfAngle) * forward;
        Vector2 rightDir = Quaternion.Euler(0, 0, halfAngle) * forward;

        // 시야각 라인 길이 (findDistance만큼)
        float length = findDistance;

        // 시야각 라인 그리기
        Gizmos.DrawRay(transform.position, leftDir * length);
        Gizmos.DrawRay(transform.position, rightDir * length);
    }
#endif

}
