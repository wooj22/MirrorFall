using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dwarf : MonoBehaviour
{
    public string anim_cur = "Idle";
    public float speed = 2f;            // 이동 속도
    public float findDistance = 5f;     //플레이어 감지 범위
    public float missDistance = 7f;      //플레이어 추척 실패 거리
    public float applefindDistance = 10f;     //사과 감지 범위
    public float applemissDistance = 15f;     //사과 추적 실패 거리
    public float attackDistance = 1f;    //공격 범위
    public float eatDistance = 1f;      //사과 섭취 범위
    bool playerfind = false;     //플레이어 추적 상태 확인
    bool goback = false;      //복귀 중 상태 확인
    bool applefind = false;     //사과 추적 상태 확인
    bool notmove = false;     //이동코드 제한
    bool isAttacking = false;    //공격 중 확인
    bool isEating = false;     //섭취 중 확인
    float playerdistance;     //플레이어와의 거리
    float appledistance;     //사과와의 거리
    Rigidbody2D rb;
    Collider2D col;
    Animator ani;
    SpriteRenderer spr;
    GameObject Player;
    GameObject nearestApple;    //가장 가까운 사과
    Vector2 PlayerPos;
    Vector2 ApplePos;
    public GameObject allpatrolPoints;     //순찰 좌표 부모오브젝트
    private List<Transform> patrolPoints = new List<Transform>(); //순찰 좌표리스트
    private int currentPointIndex = 0; //좌표리스트 인덱스
    public GameObject alldirPoints;     //경로 좌표 부모오브젝트
    private List<Transform> dirPoints = new List<Transform>(); //경로 좌표 리스트

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player");
        if (allpatrolPoints != null)
        {
            foreach (Transform child in allpatrolPoints.transform)
            {
                patrolPoints.Add(child);
            }
        }
        if (alldirPoints != null)
        {
            foreach (Transform child in alldirPoints.transform)
            {
                dirPoints.Add(child);
            }
        }
    }

    void Update()
    {
        PlayerCheck();     //플레이어 오브젝트 확인
        AppleCheck();     //사과 오브젝트 확인
        Playani();     //애니메이션 재생
        MovetoApple();     //사과추적이동
        NormalMove();     //통상상태 이동(플레이어 추적 포함)
        if (!notmove && !isAttacking)
            Attack();     //공격 중
        if (!notmove && !isEating)
            Eating();     //사과 섭취 중
    }
    void PlayerCheck()     //플레이어 오브젝트 확인
    {
        if (Player != null)
        {
            PlayerPos = Player.transform.position;
            playerdistance = Vector2.Distance(transform.position, PlayerPos);
        }
        else
        {
            PlayerPos = Vector2.positiveInfinity;
            playerdistance = float.MaxValue;
        }
    }
    void AppleCheck()     //사과 오브젝트 확인
    {
        GameObject[] allApples = GameObject.FindGameObjectsWithTag("Apple");
        List<GameObject> validApples = new List<GameObject>();

        foreach (GameObject apple in allApples)
        {
            Apple appleScript = apple.GetComponent<Apple>();
            if (appleScript != null && appleScript.isGround)   //땅에 떨어진 사과만 감지
            {
                validApples.Add(apple);
            }
        }

        if (validApples.Count > 0)   //가장 가까운 사과를 nearest Apple로 지정
        {
            float minDistance = float.MaxValue;
            GameObject closest = null;

            foreach (GameObject apple in validApples)
            {
                float dist = Vector2.Distance(transform.position, apple.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = apple;
                }
            }

            if (closest != null)
            {
                nearestApple = closest;
                ApplePos = nearestApple.transform.position;
                appledistance = minDistance;
            }
        }
        else
        {
            nearestApple = null;
            ApplePos = Vector2.positiveInfinity;
            appledistance = float.MaxValue;
        }
    }
    void NormalMove()     //통상상태 이동(플레이어 추적 포함)
    {
        if (notmove) return;
        if (applefind) return;
        if (!playerfind && !goback)     //플레이어 감지X, 복귀 중X
        {
            // 플레이어 근처로 오면 추적 시작
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide && PlayerInSight() && IsPathClear(transform.position, PlayerPos))
            {
                playerfind = true;
            }
            else
            {
                if (patrolPoints.Count == 0) return;
                Vector2 NextPos = patrolPoints[currentPointIndex].position;
                MoveToTarget(NextPos);

                // 거리가 충분히 가까워졌으면 다음 지점으로
                if (Vector2.Distance(transform.position, NextPos) < 0.2f)
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
        else if (playerfind && !goback)     //플레이어 감지O, 복귀 중X
        {
            if (playerdistance > missDistance)
            {
                goback = true;
                playerfind = false;
            }
            else
            {
                MoveToTarget(PlayerPos);
            }
        }
        else if (!playerfind && goback)     //플레이어 감지X, 복귀 중O
        {
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide && PlayerInSight() && IsPathClear(transform.position, PlayerPos))
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
    void MovetoApple()     //사과추적이동
    {
        if (notmove) return;
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
            MoveToTarget(ApplePos);

            if (appledistance > applemissDistance)
            {
                applefind = false;
            }
        }
    }
    void Playani()     //애니메이션 재생
    {
        Vector2 vel = rb.velocity;

        // 공격 중일 때
        if (vel == Vector2.zero && isAttacking)
        {
            bool isRight = transform.position.x <= PlayerPos.x;
            bool isUp = transform.position.y <= PlayerPos.y;

            SetAnimation(isUp ? "AttackUp" : "AttackDown");
            spr.flipX = !isRight;
            return;
        }

        // 대기 상태
        if (vel == Vector2.zero)
        {
            SetAnimation("Idle");
            spr.flipX = false;
            return;
        }

        // 이동 중
        bool movingRight = vel.x >= 0;
        bool movingUp = vel.y >= 0;

        SetAnimation(movingUp ? "WalkUp" : "WalkDown");
        spr.flipX = !movingRight;
    }
    void Attack()     //공격 중
    {
        if (playerdistance <= attackDistance && !isAttacking && !notmove && playerfind)
        {
            // attack
            Player.GetComponent<PlayerController>().Hit("L");

            rb.velocity = Vector2.zero;
            col.enabled = false;
            notmove = true;
            isAttacking = true;
            Invoke(nameof(ResetToStart), 0.5f);
        }
    }
    void Eating()     //사과 섭취 중
    {
        if (appledistance <= eatDistance && !isEating && !notmove)
        {
            rb.velocity = Vector2.zero;
            col.enabled = false;
            notmove = true;
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

    private void ResetToStart()     //공격 후 0번 순찰 좌표로 순간이동
    {
        transform.position = patrolPoints[0].position;
        currentPointIndex = 0;
        col.enabled = true;
        notmove = false;
        isAttacking = false;
        playerfind = false;
        goback = false;
    }

    private void ReGoing()     //사과 섭취 후 복귀상태로 전환
    {
        col.enabled = true;
        notmove = false;
        isEating = false;
        playerfind = false;
        goback = true;
    }

    private bool PlayerInSight()     //플레이어 감지 범위 110도로 제한
    {
        if (Player == null) return false;
        Vector2 toPlayer = (PlayerPos - (Vector2)transform.position).normalized;
        Vector2 forward = rb.velocity.normalized;
        if (forward == Vector2.zero)
            forward = Vector2.right;
        float angle = Vector2.Angle(forward, toPlayer);
        return angle < 55f;
    }

    private bool IsPathClear(Vector2 from, Vector2 to)     //경로 상 벽 오브젝트 확인
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

        Debug.DrawRay(castOrigin, dir * dist, Color.red);

        return hit.collider == null;
    }

    private Transform FindClosestBypassPoint(Vector2 from, Vector2 to)
    {
        Transform bestPoint = null;
        float bestScore = float.MaxValue;

        foreach (var point in dirPoints)
        {
            if (point == null) continue;

            //  1. from(AI 본체) → 우회 포인트까지는 정확한 충돌 판정 필요
            // => 기존의 BoxCast 기반 검사 함수 사용
            if (!IsPathClear(from, point.position)) continue;

            //  2. point → to(플레이어) 는 단순 Raycast로 검사
            bool directToTarget = IsLineClear(point.position, to);
            bool indirectToTarget = false;

            //  3. direct가 막혀있다면, 다른 우회 포인트 경유 가능한지 검사
            if (!directToTarget)
            {
                foreach (var other in dirPoints)
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
        Vector2 dir = (to - from).normalized;
        float dist = Vector2.Distance(from, to);

        RaycastHit2D hit = Physics2D.Raycast(from, dir, dist, LayerMask.GetMask("Wall"));

        //Debug.DrawLine(from, to, hit.collider ? Color.red : Color.green);

        return hit.collider == null;
    }
    private void MoveToTarget(Vector2 targetPos)    //목표 타겟으로 이동
    {
        if (IsPathClear(transform.position, targetPos))
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
    private void SetAnimation(string anim)
    {
        if (anim_cur == anim) return;
        anim_cur = anim;
        ani.Play(anim);
    }


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
