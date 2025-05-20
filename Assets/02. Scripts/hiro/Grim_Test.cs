using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grim_Test : MonoBehaviour
{
    // AI
    public string anim_cur = "Idle";
    public float speed = 2f;            // �̵� �ӵ�
    public float findDistance = 5f;
    public float missDistance = 7f;
    public float applefindDistance = 10f;
    public float applemissDistance = 15f;
    public float attackDistance = 2f;   // ���� �Ÿ�
    public float eatDistance = 2f;
    public Vector2 startPos;
    public LayerMask wallLayer;

    // AI �̵�
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
        // ���� �̵� + �÷��̾� ������ Chase ��ȯ
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

            // ���� ���� ����
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
        // �÷��̾� ����, ���� ���� �� Return or Bypass ��ȯ
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
        // ��ȸ ��� ���� �̵�, ���� �� Return ��ȯ
        if (patrolPoints.Count == 0) return;

        // ���� ����� ���� ���� ã��
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

        // �ش� �������� �̵�
        Vector2 restartPos = patrolPoints[closestIndex].position;
        MoveToTarget(restartPos);

        // �����ϸ� ���� �����
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

        // ���� ������ ���� �������� �ʾҴٸ� ���� ����� ���� ������ ã��
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

        // ������ ���� ������ �����ϸ� Patrol ���·� ����
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

    private void MoveToTarget(Vector2 targetPos)    //��ǥ Ÿ������ �̵�
    {
        if (IsPathClearBox(transform.position, targetPos))
        {
            // ��ΰ� ������ �ʾ����� ����
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            rb.velocity = dir * speed;
        }
        else
        {
            // ��ȸ ���� Ž��
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

    // �÷��̾� Ž��
    void PlayerCheck()
    {
        if (player != null)
        {
            playerPos = player.transform.position;
        }

        playerdistance = Vector2.Distance(transform.position, playerPos);
    }

    // ��� Ž��
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

    // �÷��� �ִϸ��̼�
    void Playani()
    {
        Vector2 velocity = rb.velocity;

        spr.flipX = velocity.x < 0;
        SetAnimation(velocity.y >= 0 ? "RightTop" : "RightBot");
    }

    // ����
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

    // ����� �԰� ��
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

    // ����
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

    // ����� �԰� �Ǹ� �ٽ� �÷��̾ Ž�� ����
    private void ReGoing()
    {
        col.enabled = true;
        isReturning = false;
        isEating = false;
        playerfind = true;
        goback = true;
    }

    // �÷��̾ �����ϴ� �þ�
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

        // ���� ������ �ݿ��� ���� ũ�� ���
        Vector2 worldSize = Vector2.Scale(box.size, transform.lossyScale) * raySize;

        // BoxCast�� �߽� ��ġ�� �ݶ��̴� �߽����� ����
        // (from�� center�� �ٸ��� center�� �������� �ϴ� �� ����)
        Vector2 castOrigin = center;

        RaycastHit2D hit = Physics2D.BoxCast(
            castOrigin, worldSize, 0f, dir, dist, LayerMask.GetMask("Wall"));

        Debug.DrawRay(castOrigin, dir * dist, Color.yellow);

        return hit.collider == null;
    }


    // ���� ����� ��ȸ��ġ Ž��
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

            // ù ��° �н�: Ÿ�ٱ��� ���� �շ��ִ� ��ȸ ����Ʈ
            // �� ��° �н�: �׳� ����� ��ȸ ����Ʈ
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

    // �ִϸ��̼� ���
    private void SetAnimation(string anim)
    {
        if (anim_cur == anim) return;
        anim_cur = anim;
        ani.Play(anim);
    }


    // ����� �߰�
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // applefindDistance �ð�ȭ
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, applefindDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, findDistance);

        // 120�� �þ߰� �ð�ȭ (��60��)
        Gizmos.color = Color.yellow;

        Vector2 forward = Application.isPlaying ? rb.velocity.normalized : Vector2.right;
        if (forward == Vector2.zero) forward = Vector2.right; // ����Ʈ ����

        float halfAngle = 55f;

        // �þ߰� �� ���� ���
        Vector2 leftDir = Quaternion.Euler(0, 0, -halfAngle) * forward;
        Vector2 rightDir = Quaternion.Euler(0, 0, halfAngle) * forward;

        // �þ߰� ���� ���� (findDistance��ŭ)
        float length = findDistance;

        // �þ߰� ���� �׸���
        Gizmos.DrawRay(transform.position, leftDir * length);
        Gizmos.DrawRay(transform.position, rightDir * length);
    }
#endif
}
