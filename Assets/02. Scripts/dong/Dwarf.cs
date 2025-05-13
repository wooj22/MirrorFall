using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dwarf : MonoBehaviour
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
    bool find = false;
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
    GameObject[] Apples;
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
        Apples = GameObject.FindGameObjectsWithTag("Finish");
        if (Apples.Length > 0)
        {
            float minDistance = float.MaxValue;
            GameObject closest = null;

            foreach (GameObject Apple in Apples)
            {
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
        if (!find)
        {
            // 플레이어 근처로 오면 추적 시작
            if (playerdistance < findDistance)
            {
                find = true;
            }
            else
            {
                Vector2 targetPos= new Vector2(0,0);
                float xOffset = Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
                float yOffset = Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
                // 왔다갔다 움직임
                targetPos = startPos + new Vector2(xOffset, 0.5f*yOffset);
                Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
                rb.velocity = dir * speed;
            }
        }
        else
        {
            if (playerdistance > missDistance)
            {
                // 원래 위치로 돌아가기
                Vector2 dirToStart = startPos - (Vector2)transform.position;
                if (dirToStart.magnitude < 0.05f)
                {
                    rb.velocity = Vector2.zero;
                    transform.position = startPos; // 위치 정확히 맞춤
                    find = false;
                }
                else
                {
                    rb.velocity = dirToStart.normalized * speed;
                }
            }
            else
            {
                // 플레이어 추적
                Vector2 dirToPlayer = (PlayerPos - (Vector2)transform.position).normalized;
                rb.velocity = dirToPlayer * speed;
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
        if (rb.velocity.x == 0 && rb.velocity.y == 0&& isAttacking)
        {
            if (transform.position.x <= PlayerPos.x&&transform.position.y<=PlayerPos.y)
            {
                SetAnimation("Attack");
                spr.flipX = false;
            }
            else if(transform.position.x <= PlayerPos.x && transform.position.y > PlayerPos.y)
            {
                SetAnimation("Attack1");
                spr.flipX = false;
            }
            else if(transform.position.x > PlayerPos.x && transform.position.y<=PlayerPos.y)
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
        else if (rb.velocity.x<0 && rb.velocity.y < 0)
        {
            SetAnimation("Walk");
            spr.flipX = true;
        }
    }
    void Attack()
    {
        if (playerdistance <= attackDistance && !isAttacking && !isReturning)
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

        find = false;
        col.enabled = true;
        isReturning = false;
        isAttacking = false;
    }

    private void ReGoing()
    {
        col.enabled = true;
        isReturning = false;
        isEating = false;
        find = true;
    }

    private void SetAnimation(string anim)
    {
        if (anim_cur == anim) return;
        anim_cur = anim;
        ani.Play(anim);
    }

}
