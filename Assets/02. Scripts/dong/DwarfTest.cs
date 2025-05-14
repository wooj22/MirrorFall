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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player");
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
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide)
            {
                playerfind = true;
            }
            else
            {
                if (patrolPoints.Count == 0) return;
                Vector2 NextPos = patrolPoints[currentPointIndex].position;
                Vector2 dirToNext = (NextPos - (Vector2)transform.position).normalized;
                rb.velocity = dirToNext * speed;

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
                // �÷��̾� ����
                Vector2 dirToPlayer = (PlayerPos - (Vector2)transform.position).normalized;
                rb.velocity = dirToPlayer * speed;
            }
        }
        else if (!playerfind && goback)
        {
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
            Vector2 dirToPatrol = (restartPos - (Vector2)transform.position).normalized;
            rb.velocity = dirToPatrol * speed;

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
            ApplePos = nearestApple.transform.position;
            appledistance = Vector2.Distance(transform.position, ApplePos);

            Vector2 dirToApple = (ApplePos - (Vector2)transform.position).normalized;
            rb.velocity = dirToApple * speed;

            if (appledistance > applemissDistance)
            {
                applefind = false;
            }
        }
    }
    void Playani()
    {
        if (rb.velocity.x == 0 && rb.velocity.y == 0 && isAttacking)
        {
            if (transform.position.x <= PlayerPos.x && transform.position.y <= PlayerPos.y)
            {
                SetAnimation("Attack");
                spr.flipX = false;
            }
            else if (transform.position.x <= PlayerPos.x && transform.position.y > PlayerPos.y)
            {
                SetAnimation("Attack1");
                spr.flipX = false;
            }
            else if (transform.position.x > PlayerPos.x && transform.position.y <= PlayerPos.y)
            {
                SetAnimation("Attack");
                spr.flipX = true;
            }
            else if (transform.position.x > PlayerPos.x && transform.position.y > PlayerPos.y)
            {
                SetAnimation("Attack1");
                spr.flipX = true;
            }
        }
        else if (rb.velocity.x == 0 && rb.velocity.y == 0)
        {
            SetAnimation("Idle");
            spr.flipX = false;
        }
        else if (rb.velocity.x >= 0 && rb.velocity.y >= 0)
        {
            SetAnimation("Walk1");
            spr.flipX = false;
        }
        else if (rb.velocity.x >= 0 && rb.velocity.y < 0)
        {
            SetAnimation("Walk");
            spr.flipX = false;
        }
        else if (rb.velocity.x < 0 && rb.velocity.y >= 0)
        {
            SetAnimation("Walk1");
            spr.flipX = true;
        }
        else if (rb.velocity.x < 0 && rb.velocity.y < 0)
        {
            SetAnimation("Walk");
            spr.flipX = true;
        }
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
    }
#endif

}
