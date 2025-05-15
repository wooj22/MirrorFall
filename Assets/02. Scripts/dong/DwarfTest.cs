using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DwarfTest : MonoBehaviour
{
    public string anim_cur = "Idle";
    public float moveDistance = 3f;     // �Դٰ����� �Ÿ�
    public float speed = 2f;            // �̵� �ӵ�
    public float findDistance = 5f;
    public float missDistance = 7f;
    public float applefindDistance = 10f;
    public float applemissDistance = 15f;
    public float attackDistance = 1f;
    public float eatDistance = 1f;
    bool playerfind = false;
    bool goback = false;
    bool applefind = false;
    bool isReturning = false;
    bool isAttacking = false;
    bool isEating = false;
    float playerdistance;
    float appledistance;
    Rigidbody2D rb;
    Collider2D col;
    Animator ani;
    SpriteRenderer spr;
    GameObject Player;
    GameObject nearestApple;
    Vector2 PlayerPos;
    Vector2 ApplePos;

    public List<Transform> patrolPoints; //���� ��ǥ����Ʈ
    private int currentPointIndex=0; //��ǥ����Ʈ �ε���
    public GameObject alldirPoints;
    private List<Transform> dirPoints = new List<Transform>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player");
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
        PlayerCheck();
        AppleCheck();
        Playani();
        MovetoApple();
        NormalMove();
        if (!isReturning && !isAttacking)
            Attack();
        if (!isReturning && !isEating)
            Eating();
    }
    void PlayerCheck()
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
    void AppleCheck()
    {
        GameObject[] allApples = GameObject.FindGameObjectsWithTag("Apple");
        List<GameObject> validApples = new List<GameObject>();

        foreach (GameObject apple in allApples)
        {
            Apple appleScript = apple.GetComponent<Apple>();
            if (appleScript != null && appleScript.isGround)
            {
                validApples.Add(apple);
            }
        }

        if (validApples.Count > 0)
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
    void NormalMove()
    {
        if (isReturning) return;
        if (applefind) return;
        if (!playerfind && !goback)
        {
            // �÷��̾� ��ó�� ���� ���� ����
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide &&PlayerInSight())
            {
                playerfind = true;
            }
            else
            {
                if (patrolPoints.Count == 0) return;
                Vector2 NextPos = patrolPoints[currentPointIndex].position;
                if (IsPathClear(transform.position, NextPos))
                {
                    // ��ΰ� ������ �ʾ����� ����
                    Vector2 dirToNext = (NextPos - (Vector2)transform.position).normalized;
                    rb.velocity = dirToNext * speed;
                }
                else
                {
                    // ���� �ִٸ� ��ȸ�� �� �ִ� ���� ����� ����Ʈ Ž��
                    Transform bypassPointToNe = FindClosestBypassPoint(transform.position, NextPos);

                    if (bypassPointToNe != null)
                    {
                        Vector2 bypassPosToNe = bypassPointToNe.position;
                        Vector2 dirToBypassToNe = (bypassPosToNe - (Vector2)transform.position).normalized;
                        rb.velocity = dirToBypassToNe * speed;
                    }
                    else
                    {
                        // ��ȸ ����Ʈ ������ ���
                        rb.velocity = Vector2.zero;
                    }
                }

                // �Ÿ��� ����� ����������� ���� ��������
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
        else if (playerfind && !goback)
        {
            if (playerdistance > missDistance)
            {
                goback = true;
                playerfind = false;
            }
            else
            {
                if (IsPathClear(transform.position, PlayerPos))
                {
                    // ��ΰ� ������ �ʾ����� ����
                    Vector2 dirToPlayer = (PlayerPos - (Vector2)transform.position).normalized;
                    rb.velocity = dirToPlayer * speed;
                }
                else
                {
                    // ���� �ִٸ� ��ȸ�� �� �ִ� ���� ����� ����Ʈ Ž��
                    Transform bypassPointToPr = FindClosestBypassPoint(transform.position, PlayerPos);

                    if (bypassPointToPr != null)
                    {
                        Vector2 bypassPosToPr = bypassPointToPr.position;
                        Vector2 dirToBypassToPr = (bypassPosToPr - (Vector2)transform.position).normalized;
                        rb.velocity = dirToBypassToPr * speed;
                    }
                    else
                    {
                        // ��ȸ�� ����Ʈ���� ���ٸ� ���
                        rb.velocity = Vector2.zero;
                    }
                }
            }
        }
        else if (!playerfind && goback)
        {
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide && PlayerInSight())
            {
                playerfind = true;
                goback = false;
            }
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
            if (IsPathClear(transform.position, restartPos))
            {
                // ��ΰ� ������ �ʾ����� ����
                Vector2 dirToPatrol = (restartPos - (Vector2)transform.position).normalized;
                rb.velocity = dirToPatrol * speed;
            }
            else
            {
                // ���� �ִٸ� ��ȸ�� �� �ִ� ���� ����� ����Ʈ Ž��
                Transform bypassPointToPt = FindClosestBypassPoint(transform.position, restartPos);

                if (bypassPointToPt != null)
                {
                    Vector2 bypassPosToPt = bypassPointToPt.position;
                    Vector2 dirToBypassToPt = (bypassPosToPt - (Vector2)transform.position).normalized;
                    rb.velocity = dirToBypassToPt * speed;
                }
                else
                {
                    // ��ȸ ����Ʈ ������ ���
                    rb.velocity = Vector2.zero;
                }
            }

            // �����ϸ� ���� �����
            if (Vector2.Distance(transform.position, restartPos) < 0.2f)
            {
                currentPointIndex = closestIndex;
                goback = false;
            }
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
            if (IsPathClear(transform.position, ApplePos))
            {
                // ��ΰ� ������ �ʾ����� ����
                Vector2 dirToApple = (ApplePos - (Vector2)transform.position).normalized;
                rb.velocity = dirToApple * speed;
            }
            else
            {
                // ���� �ִٸ� ��ȸ�� �� �ִ� ���� ����� ����Ʈ Ž��
                Transform bypassPointToAp = FindClosestBypassPoint(transform.position, ApplePos);

                if (bypassPointToAp != null)
                {
                    Vector2 bypassPosToAp = bypassPointToAp.position;
                    Vector2 dirToBypassToAp = (bypassPosToAp - (Vector2)transform.position).normalized;
                    rb.velocity = dirToBypassToAp * speed;
                }
                else
                {
                    // ��ȸ�� ����Ʈ���� ���ٸ� ���
                    rb.velocity = Vector2.zero;
                }
            }

            if (appledistance > applemissDistance)
            {
                applefind = false;
            }
        }
    }
    void Playani()
    {
        Vector2 vel = rb.velocity;

        // ���� ���� ��
        if (vel == Vector2.zero && isAttacking)
        {
            bool isRight = transform.position.x <= PlayerPos.x;
            bool isUp = transform.position.y <= PlayerPos.y;

            SetAnimation(isUp ? "Attack" : "Attack1");
            spr.flipX = !isRight;
            return;
        }

        // ��� ����
        if (vel == Vector2.zero)
        {
            SetAnimation("Idle");
            spr.flipX = false;
            return;
        }

        // �̵� ��
        bool movingRight = vel.x >= 0;
        bool movingUp = vel.y >= 0;

        SetAnimation(movingUp ? "Walk1" : "Walk");
        spr.flipX = !movingRight;
    }
    void Attack()
    {
        if (playerdistance <= attackDistance && !isAttacking && !isReturning && playerfind)
        {
            // attack
            Player.GetComponent<PlayerController>().Hit("L");

            rb.velocity = Vector2.zero;
            col.enabled = false;
            isReturning = true;
            isAttacking = true;
            Invoke(nameof(ResetToStart), 0.5f);
        }
    }
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

    private void ResetToStart()
    {
        transform.position = patrolPoints[0].position;
        currentPointIndex = 0;
        col.enabled = true;
        isReturning = false;
        isAttacking = false;
        playerfind = false;
        goback = false;
    }

    private void ReGoing()
    {
        col.enabled = true;
        isReturning = false;
        isEating = false;
        playerfind = false;
        goback = true;
    }

    private bool PlayerInSight()
    {
        if (Player == null) return false;
        Vector2 toPlayer = (PlayerPos - (Vector2)transform.position).normalized;
        Vector2 forward = rb.velocity.normalized;
        float angle = Vector2.Angle(forward, toPlayer);
        return angle < 55f;
    }

    private bool IsPathClear(Vector2 from, Vector2 to)
    {
        RaycastHit2D hit = Physics2D.Raycast(from, (to - from).normalized, Vector2.Distance(from, to), LayerMask.GetMask("Wall"));
        return hit.collider == null;
    }

    private Transform FindClosestBypassPoint(Vector2 from, Vector2 to)
    {
        Transform bestPoint = null;
        float minDistanceToTarget = float.MaxValue;

        foreach (var point in dirPoints)
        {
            if (point == null) continue;

            if (IsPathClear(from, point.position))
            {
                float distToTarget = Vector2.Distance(point.position, to);

                if (distToTarget < minDistanceToTarget)
                {
                    minDistanceToTarget = distToTarget;
                    bestPoint = point;
                }
            }
        }

        return bestPoint;
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
