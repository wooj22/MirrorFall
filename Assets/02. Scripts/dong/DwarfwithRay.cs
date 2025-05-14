using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfwithRay : MonoBehaviour
{
    public string anim_cur = "Idle";
    public float moveDistance = 3f;     // 왔다갔다할 거리
    public float speed = 2f;            // 이동 속도
    public float findDistance = 5f;
    public float missDistance = 7f;
    public float applefindDistance = 10f;
    public float applemissDistance = 15f;
    public float attackDistance = 1f;
    public float eatDistance = 1f;
    public Vector2 startPos;
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

    void Start()
    {
        startPos = transform.position;
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
            // 플레이어 근처로 오면 추적 시작
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide)
            {
                playerfind = true;
            }
            else
            {
                Vector2 targetPos = new Vector2(0, 0);
                float xOffset = Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
                float yOffset = Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
                // 왔다갔다 움직임
                targetPos = startPos + new Vector2(xOffset, 0.5f * yOffset);
                Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
                rb.velocity = dir * speed;
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
                // 플레이어 추적
                Vector2 dirToPlayer = (PlayerPos - (Vector2)transform.position).normalized;
                rb.velocity = dirToPlayer * speed;
            }
        }
        else if (!playerfind && goback)
        {
            Vector2 dirToStart = startPos - (Vector2)transform.position;
            if (playerdistance < findDistance && !Player.GetComponent<PlayerController>().isHide)
            {
                playerfind = true;
                goback = false;
            }
            if (dirToStart.magnitude < 0.05f)
            {
                rb.velocity = Vector2.zero;
                transform.position = startPos; // 위치 정확히 맞춤
                goback = false;
            }
            else
            {
                rb.velocity = dirToStart.normalized * speed;
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
        transform.position = startPos;

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
        // applefindDistance 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, applefindDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, findDistance);
    }
#endif

}
