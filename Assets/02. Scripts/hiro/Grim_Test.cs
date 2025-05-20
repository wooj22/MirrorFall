using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grim_Test : MonoBehaviour
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
    public LayerMask wallLayer;

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
    private bool isPlayerHidden = false;
    private float playerdistance;
    private float appledistance;

    private int currentPatrolIndex = 0;
    private int returnPatrolIndex = -1;

    private Transform currentBypassTarget = null;

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

    enum EnemyState
    {
        Patrol,
        Chase,
        Bypass,
        Return
    } EnemyState currentState;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        playerSpr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        isPlayerHidden = player.GetComponent<PlayerController>().isHide;

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

        if (!isAttacking && !isReturning)
        {
            Attack();
        }
        if (!isReturning && !isEating)
        {
            Eating();
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
            case EnemyState.Bypass:
                BypassState();
                break;
            case EnemyState.Return:
                ReturnState();
                break;
        }
    }

    void PatrolState()
    {
        // 순찰 이동 + 플레이어 감지시 Chase 전환
        if (playerdistance < findDistance && !isPlayerHidden && PlayerInSight() && IsPathClearBox(transform.position, playerPos))
        {
            currentState = EnemyState.Chase;
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

    void ChaseState()
    {
        // 플레이어 추적, 감지 실패 시 Return or Bypass 전환
        if (playerdistance > missDistance)
        {
            playerfind = false;
            goback = true;

            currentState = EnemyState.Return;
        }
        else
        {
            MoveToTarget(playerPos);
        }
    }

    void BypassState()
    {
        // 우회 경로 따라 이동, 도착 시 Return 전환
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

            currentState = EnemyState.Return;
        }
    }

    void ReturnState()
    {
        if (patrolPoints.Count == 0) return;

        // 복귀 지점이 아직 지정되지 않았다면 가장 가까운 순찰 지점을 찾음
        if (returnPatrolIndex == -1)
        {
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

            returnPatrolIndex = closestIndex;
        }

        Vector2 returnPos = patrolPoints[returnPatrolIndex].position;
        MoveToTarget(returnPos);

        // 지정된 복귀 지점에 도착하면 Patrol 상태로 복귀
        if (Vector2.Distance(transform.position, returnPos) < 0.2f)
        {
            currentPointIndex = returnPatrolIndex;
            returnPatrolIndex = -1;
            goback = false;
            playerfind = false;
            isReturning = false;
            isAttacking = false;
            isEating = false;

            currentState = EnemyState.Patrol;
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
        playerfind = false;
        col.enabled = true;
        isAttacking = false;
        isReturning = false;
        goback = false;

        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
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
