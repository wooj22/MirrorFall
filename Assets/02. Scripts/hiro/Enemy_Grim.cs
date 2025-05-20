using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private bool goback = false;
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
    private Vector2 playerfootPos;
    private Vector2 applePos;

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
            // 플레이어 자식 오브젝트 AIPos 찾기 (없으면 플레이어 위치로 대체)
            Transform foot = player.transform.Find("AIPos");
            if (foot != null)
            {
                playerfootPos = foot.position;
            }
            else
            {
                playerfootPos = playerPos;
            }
            playerdistance = Vector2.Distance(transform.position, playerPos);
        }
        else
        {
            playerPos = Vector2.positiveInfinity;
            playerfootPos = Vector2.positiveInfinity;
            playerdistance = float.MaxValue;
        }
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
                if (!Apple.GetComponent<Apple>().isGround) continue;

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
            else
            {
                nearestApple = null;
                applePos = Vector2.positiveInfinity;
                appledistance = float.MaxValue;
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
        if (isReturning || applefind) return;

        bool isPlayerHidden = player.GetComponent<PlayerController>().isHide;

        if (!playerfind && !goback) // 플레이어 감지x, 복귀 중x
        {
            if (playerdistance < findDistance && !isPlayerHidden && PlayerInSight() && IsPathClearBox(transform.position, playerPos))
            {
                playerfind = true;
            }
            else
            {
                if (patrolPoints.Count == 0) return;
                Vector2 nextPos = patrolPoints[currentPointIndex].position;

                MoveToTarget(nextPos);

                // 순찰 지점 갱신
                if (Vector2.Distance(transform.position, nextPos) < 0.2f)
                {
                    int nextIndex = (currentPointIndex + 1) % patrolPoints.Count;

                    int attempts = 0;
                    while (patrolPoints[nextIndex] == null && attempts < patrolPoints.Count)
                    {
                        nextIndex = (nextIndex + 1) % patrolPoints.Count;
                        attempts++;
                    }

                    currentPointIndex = nextIndex;
                }
            }
        }
        else if (playerfind && !goback) // 플레이어 감지o, 복귀 중x
        {
            // 플레이어 추적
            if (playerdistance > missDistance)
            {
                playerfind = false;
                goback = true;
            }
            else
            {
                MoveToTarget(playerPos);
            }
        }
        else if (!playerfind && goback) // 플레이어 감지x, 복귀 중o
        {
            if (playerdistance < findDistance && !player.GetComponent<PlayerController>().isHide && PlayerInSight() && IsPathClearBox(transform.position, playerPos))
            {
                playerfind = true;
                goback = false;
            }

            if (patrolPoints.Count == 0) return;

            // 가장 가까운 순찰 지점 찾기
            float closestDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < patrolPoints.Count; i++)
            {
                float dist = Vector2.Distance(transform.position, patrolPoints[i].position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestIndex = i;
                }
            }

            // 해당 지점으로 이동
            Vector2 restartPos = patrolPoints[closestIndex].position;
            MoveToTarget(restartPos);

            // 도착하면 순찰 재시작
            if (Vector2.Distance(transform.position, restartPos) < 0.2f)
            {
                currentPointIndex = closestIndex;
                goback = false;
            }
        }
    }


    private void MoveToTarget(Vector2 targetPos)    //목표 타겟으로 이동
    {
        if (IsPathClearBox(transform.position, targetPos))
        {
            // 경로가 막히지 않았으면 직진
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            rb.velocity = dir * speed;
        }
        else
        {
            // 우회 지점 탐색
            Transform bypass = FindClosestBypassPoint(transform.position, targetPos);

            if (bypass != null)
            {
                Vector2 dir = ((Vector2)bypass.position - (Vector2)transform.position).normalized;
                rb.velocity = dir * speed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
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
        Transform respawnPos;

        int tryCount = 0;

        do
        {
            respawnPos = patrolPoints[Random.Range(0, patrolPoints.Count)];

            tryCount++;

            if (tryCount >= patrolPoints.Count)
            {
                respawnPos = patrolPoints.OrderByDescending(p => Vector2.Distance(p.position, playerPos)).First();
                break;
            }

        } while (Vector2.Distance(respawnPos.position, playerPos) < missDistance);

        transform.position = respawnPos.position;
        currentPointIndex = patrolPoints.IndexOf(respawnPos);
        playerfind = false;
        col.enabled = true;
        isAttacking = false;
        isReturning = false;
        goback = false;
    }

    // 사과를 먹게 되면 다시 플레이어를 탐색 시작
    private void ReGoing()
    {
        col.enabled = true;
        isReturning = false;
        isEating = false;
        playerfind = true;
        goback = true;
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
    private bool IsPathClearBox(Vector2 from, Vector2 to)
    {
        if (Vector2.Distance(to, playerPos) < 0.01f)
        {
            to = playerfootPos;
        }
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
    //private Transform FindClosestBypassPoint(Vector2 from, Vector2 to)
    //{
    //    Transform bestPoint = null;
    //    float minDistanceToTarget = float.MaxValue;

    //    foreach (var point in bypassPoints)
    //    {
    //        if (point == null || !IsPathClearBox(from, point.position)) continue;

    //        bool hasClearPathToTarget = !Physics2D.Raycast(
    //            point.position,
    //            (to - (Vector2)point.position).normalized,
    //            Vector2.Distance(point.position, to),
    //            LayerMask.GetMask("Wall")
    //        );

    //        // 첫 번째 패스: 타겟까지 길이 뚫려있는 우회 포인트
    //        // 두 번째 패스: 그냥 가까운 우회 포인트
    //        if (bestPoint == null || hasClearPathToTarget || bestPoint != null && !hasClearPathToTarget)
    //        {
    //            float distToTarget = Vector2.Distance(point.position, to);

    //            if (distToTarget < minDistanceToTarget &&
    //                (hasClearPathToTarget || bestPoint == null))
    //            {
    //                minDistanceToTarget = distToTarget;
    //                bestPoint = point;
    //            }
    //        }
    //    }

    //    return bestPoint;
    //}

    private Transform FindClosestBypassPoint(Vector2 from, Vector2 to)
    {
        Transform bestPoint = null;
        float bestScore = float.MaxValue;

        foreach (var point in bypassPoints)
        {
            if (point == null) continue;

            //  1. from(AI 본체) → 우회 포인트까지는 정확한 충돌 판정 필요
            // => 기존의 BoxCast 기반 검사 함수 사용
            if (!IsPathClearBox(from, point.position)) continue;

            //  2. point → to(플레이어) 는 단순 Raycast로 검사
            bool directToTarget = IsLineClear(point.position, to);
            bool indirectToTarget = false;

            //  3. direct가 막혀있다면, 다른 우회 포인트 경유 가능한지 검사
            if (!directToTarget)
            {
                foreach (var other in bypassPoints)
                {
                    if (other == null || other == point) continue;

                    // 3-1. point → other
                    bool connectable = IsLineClear(point.position, other.position);

                    // 3-2. other → to
                    bool otherToTarget = IsLineClear(other.position, to);

                    if (connectable && otherToTarget)
                    {
                        indirectToTarget = true;
                        break;
                    }
                }
            }

            //  4. 우선순위 분류
            int priority = directToTarget ? 1 :
                           indirectToTarget ? 2 :
                           3;

            // 거리 보정값 (여기선 point → target 거리 기준)
            float distance = Vector2.Distance(point.position, to);
            float score = priority * 1000 + distance;

            //  5. 가장 점수가 낮은 포인트 선택
            if (score < bestScore)
            {
                bestScore = score;
                bestPoint = point;
            }
        }

        return bestPoint;
    }

    private bool IsLineClear(Vector2 from, Vector2 to)
    {
        if (Vector2.Distance(to, playerPos) < 0.01f)
        {
            to = playerfootPos;
        }
        Vector2 dir = (to - from).normalized;
        float dist = Vector2.Distance(from, to);

        RaycastHit2D hit = Physics2D.Raycast(from, dir, dist, LayerMask.GetMask("Wall"));

        //Debug.DrawLine(from, to, hit.collider ? Color.red : Color.green);

        return hit.collider == null;
    }


    // 애니메이션 재생
    private void SetAnimation(string anim)
    {
        if (anim_cur == anim) return;
        anim_cur = anim;
        ani.Play(anim);
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
