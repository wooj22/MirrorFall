using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dwarf : MonoBehaviour
{
    public string anim_cur = "Idle";
    public float speed = 2f;            // �̵� �ӵ�
    public float findDistance = 5f;     //�÷��̾� ���� ����
    public float missDistance = 7f;      //�÷��̾� ��ô ���� �Ÿ�
    public float applefindDistance = 10f;     //��� ���� ����
    public float applemissDistance = 15f;     //��� ���� ���� �Ÿ�
    public float attackDistance = 1f;    //���� ����
    public float eatDistance = 1f;      //��� ���� ����
    bool playerfind = false;     //�÷��̾� ���� ���� Ȯ��
    bool goback = false;      //���� �� ���� Ȯ��
    bool applefind = false;     //��� ���� ���� Ȯ��
    bool notmove = false;     //�̵��ڵ� ����
    bool isAttacking = false;    //���� �� Ȯ��
    bool isEating = false;     //���� �� Ȯ��
    float playerdistance;     //�÷��̾���� �Ÿ�
    float appledistance;     //������� �Ÿ�
    Rigidbody2D rb;
    Collider2D col;
    Animator ani;
    SpriteRenderer spr;
    GameObject Player;
    GameObject nearestApple;    //���� ����� ���
    Vector2 PlayerPos;
    Vector2 ApplePos;
    public GameObject allpatrolPoints;     //���� ��ǥ �θ������Ʈ
    private List<Transform> patrolPoints = new List<Transform>(); //���� ��ǥ����Ʈ
    private int currentPointIndex = 0; //��ǥ����Ʈ �ε���
    public GameObject alldirPoints;     //��� ��ǥ �θ������Ʈ
    private List<Transform> dirPoints = new List<Transform>(); //��� ��ǥ ����Ʈ

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
        PlayerCheck();     //�÷��̾� ������Ʈ Ȯ��
        AppleCheck();     //��� ������Ʈ Ȯ��
        Playani();     //�ִϸ��̼� ���
        MovetoApple();     //��������̵�
        NormalMove();     //������ �̵�(�÷��̾� ���� ����)
        if (!notmove && !isAttacking)
            Attack();     //���� ��
        if (!notmove && !isEating)
            Eating();     //��� ���� ��
    }
    void PlayerCheck()     //�÷��̾� ������Ʈ Ȯ��
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
    void AppleCheck()     //��� ������Ʈ Ȯ��
    {
        GameObject[] allApples = GameObject.FindGameObjectsWithTag("Apple");
        List<GameObject> validApples = new List<GameObject>();

        foreach (GameObject apple in allApples)
        {
            Apple appleScript = apple.GetComponent<Apple>();
            if (appleScript != null && appleScript.isGround)   //���� ������ ����� ����
            {
                validApples.Add(apple);
            }
        }

        if (validApples.Count > 0)   //���� ����� ����� nearest Apple�� ����
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
    void NormalMove()     //������ �̵�(�÷��̾� ���� ����)
    {
        if (notmove) return;
        if (applefind) return;
        if (!playerfind && !goback)     //�÷��̾� ����X, ���� ��X
        {
            // �÷��̾� ��ó�� ���� ���� ����
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide && PlayerInSight() && IsPathClear(transform.position, PlayerPos))
            {
                playerfind = true;
            }
            else
            {
                if (patrolPoints.Count == 0) return;
                Vector2 NextPos = patrolPoints[currentPointIndex].position;
                MoveToTarget(NextPos);

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
        else if (playerfind && !goback)     //�÷��̾� ����O, ���� ��X
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
        else if (!playerfind && goback)     //�÷��̾� ����X, ���� ��O
        {
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide && PlayerInSight() && IsPathClear(transform.position, PlayerPos))
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
            MoveToTarget(restartPos);

            // �����ϸ� ���� �����
            if (Vector2.Distance(transform.position, restartPos) < 0.2f)
            {
                currentPointIndex = closestIndex;
                goback = false;
            }
        }
    }
    void MovetoApple()     //��������̵�
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
    void Playani()     //�ִϸ��̼� ���
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
    void Attack()     //���� ��
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
    void Eating()     //��� ���� ��
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

    private void ResetToStart()     //���� �� 0�� ���� ��ǥ�� �����̵�
    {
        transform.position = patrolPoints[0].position;
        currentPointIndex = 0;
        col.enabled = true;
        notmove = false;
        isAttacking = false;
        playerfind = false;
        goback = false;
    }

    private void ReGoing()     //��� ���� �� ���ͻ��·� ��ȯ
    {
        col.enabled = true;
        notmove = false;
        isEating = false;
        playerfind = false;
        goback = true;
    }

    private bool PlayerInSight()     //�÷��̾� ���� ���� 110���� ����
    {
        if (Player == null) return false;
        Vector2 toPlayer = (PlayerPos - (Vector2)transform.position).normalized;
        Vector2 forward = rb.velocity.normalized;
        if (forward == Vector2.zero)
            forward = Vector2.right;
        float angle = Vector2.Angle(forward, toPlayer);
        return angle < 55f;
    }

    private bool IsPathClear(Vector2 from, Vector2 to)     //��� �� �� ������Ʈ Ȯ��
    {
        BoxCollider2D box = col as BoxCollider2D;
        if (box == null) return false;

        Vector2 direction = (to - from).normalized;
        float distance = Vector2.Distance(from, to);
        Debug.DrawLine(from, to, Color.magenta, 0.1f);
        float expandRate = 0.1f;                  //�ݶ��̴��� ���� �� 0.1����
        Vector2 scaledSize = box.size * box.transform.lossyScale;
        Vector2 size = scaledSize * (1f + expandRate);
        Vector2[] offsets = new Vector2[]
        {
        new Vector2(-size.x / 2f, -size.y / 2f),
        new Vector2(-size.x / 2f, size.y / 2f),
        new Vector2(size.x / 2f, -size.y / 2f),
        new Vector2(size.x / 2f, size.y / 2f),
        };

        Vector2 boxCenter = (Vector2)box.transform.TransformPoint(box.offset);

        foreach (Vector2 offset in offsets)
        {
            Vector2 rayOrigin = boxCenter + offset;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, distance, LayerMask.GetMask("Wall"));
            Debug.DrawRay(rayOrigin, direction * distance, Color.red, 0.1f);

            if (hit.collider != null)
                return false;
        }

        return true;
    }

    private Transform FindClosestBypassPoint(Vector2 from, Vector2 to)     //�̵� ������ ���� ��ǥ Ȯ��
    {
        Transform bestPoint = null;
        float minDistanceToTarget = float.MaxValue;

        foreach (var point in dirPoints)
        {
            if (point == null) continue;

            if (IsPathClear(from, point.position) && IsPathClear(point.position, to))
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
    private void MoveToTarget(Vector2 targetPos)    //��ǥ Ÿ������ �̵�
    {
        if (IsPathClear(transform.position, targetPos))
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
