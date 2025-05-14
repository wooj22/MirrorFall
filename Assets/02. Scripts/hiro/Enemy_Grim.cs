using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy_Grim : MonoBehaviour
{
    // ai
    public string anim_cur = "Idle";
    public float moveDistanceX = 3f;     // 왔다갔다할 거리
    public float moveDistanceY = 3f;
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

    // raycast
    public float rayDistance = 1f; // 레이의 최대 거리
    public LayerMask wallLayer;
    private Vector2 rayDirection;

    private bool isAvoiding = false;
    private Vector2 avoidDir = Vector2.zero;
    private float avoidCooldown = 0f;
    public float avoidDuration = 0.5f; // 회피 방향 유지 시간

    public List<Transform> patrolPoints;
    private int currentPointIndex = 0;

    // map
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

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
    private Vector2 newPos;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        playerSpr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //Debug.Log(playerdistance);
        RaycastCheck();
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
    void PlayerCheck()
    {
        if (player != null)
        {
            playerPos = player.transform.position;
        }

        playerdistance = Vector2.Distance(transform.position, playerPos);
    }

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

    private Vector2[] diagonalDirs = new Vector2[]
    {
        new Vector2(1f, 0.5f).normalized, 
        new Vector2(-1f, 0.5f).normalized,  
        new Vector2(-1f, -0.5f).normalized, 
        new Vector2(1f, -0.5f).normalized 
    };

    void NormalMove()
    {
        if (isReturning || applefind || isAttacking) return;

        bool isPlayerHidden = player.GetComponent<PlayerController>().isHide;

        if (!playerfind)
        {
            if (playerdistance < findDistance && !isPlayerHidden)
            {
                playerfind = true;
            }
            else
            {
                if (patrolPoints.Count == 0) return;

                Vector2 nextPos = patrolPoints[currentPointIndex].position;
                Vector2 dirToNext = (nextPos - (Vector2)transform.position).normalized;

                if (isAvoiding)
                {
                    // 회피 상태일 때
                    rb.velocity = avoidDir * speed;
                    avoidCooldown -= Time.deltaTime; // 쿨다운 시간 감소

                    if (avoidCooldown <= 0f)
                    {
                        isAvoiding = false; // 쿨다운 끝나면 회피 해제
                    }
                    return; // 회피 중에는 다른 이동 안 함
                }

                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToNext, rayDistance, wallLayer);

                if (hit.collider != null)
                {
                    // 벽을 만났을 때 회피
                    foreach (var dir in diagonalDirs)
                    {
                        RaycastHit2D check = Physics2D.Raycast(transform.position, dir, rayDistance, wallLayer);
                        if (check.collider == null)
                        {
                            // 벽이 없다면 회피 시작
                            avoidDir = dir;
                            isAvoiding = true;
                            avoidCooldown = avoidDuration; // 쿨다운 시간 설정
                            rb.velocity = avoidDir * speed;
                            return; // 회피 중에는 이동 안 함
                        }
                    }

                    rb.velocity = Vector2.zero; // 벽이 있는데 회피할 방향이 없으면 멈춤
                }
                else
                {
                    // 회피 상태가 아니면 정상적으로 이동
                    rb.velocity = dirToNext * speed;
                }

                // 순찰 지점 갱신
                if (Vector2.Distance(transform.position, nextPos) < 0.2f)
                {
                    currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
                }
            }
        }
        else
        {
            // 플레이어 추적
            if (playerdistance > missDistance)
            {
                if (isPlayerHidden)
                {
                    rb.velocity = Vector2.zero;
                    playerfind = false;
                    return;
                }

                Vector2 dirToStart = startPos - (Vector2)transform.position;
                if (dirToStart.magnitude < 0.05f)
                {
                    rb.velocity = Vector2.zero;
                    transform.position = startPos; // 위치 정확히 맞춤
                    playerfind = false;
                }
                else
                {
                    rb.velocity = dirToStart.normalized * speed;
                }
            }
            else
            {
                // 플레이어 추적
                Vector2 dirToPlayer = (playerPos - (Vector2)transform.position).normalized;
                rb.velocity = dirToPlayer * speed;
            }
        }
    }

    List<Vector2> GetEvadeDirections(Vector2 currentDir)
    {
        List<Vector2> result = new List<Vector2>();

        // 현재 방향의 반시계 방향과 시계 방향 회전 벡터를 우선 순위로 둠
        Vector2 leftTurn = new Vector2(-currentDir.y, currentDir.x).normalized;
        Vector2 rightTurn = new Vector2(currentDir.y, -currentDir.x).normalized;

        // 대각선으로 약간 비튼 방향 추가
        result.Add((currentDir + leftTurn).normalized);
        result.Add((currentDir + rightTurn).normalized);

        // 마지막 수단: 완전 반대
        result.Add(-currentDir);

        return result;
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
            applePos = nearestApple.transform.position;
            appledistance = Vector2.Distance(transform.position, applePos);

            Vector2 dirToApple = (applePos - (Vector2)transform.position).normalized;
            rb.velocity = dirToApple * speed;

            if (appledistance > applemissDistance)
            {
                applefind = false;
            }
        }
    }

    void Playani()
    {
        Vector2 velocity = rb.velocity;
        spr.sortingOrder = velocity.y <= 0 ? downSorting : upSorting;

        spr.flipX = velocity.x < 0;
        SetAnimation(velocity.y >= 0 ? "RightTop" : "RightBot");
    }

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
        newPos = GetRandomPositionAwayFromPlayer(15f, 20f);
        transform.position = newPos;
        startPos = newPos;
        playerfind = false;
        col.enabled = true;
        isAttacking = false;
        isReturning = false;
    }

    private void ReGoing()
    {
        col.enabled = true;
        isReturning = false;
        isEating = false;
        playerfind = true;
    }
    private void SetAnimation(string anim)
    {
        if (anim_cur == anim) return;
        anim_cur = anim;
        ani.Play(anim);
    }

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

    private void RaycastCheck()
    {
        Vector2 rayStart = transform.position;

        Vector2 velocity = rb.velocity;

        Vector2 rayDirection = new Vector2(
            velocity.x >= 0 ? 1f : -1f,
            velocity.y >= 0 ? 0.5f : -0.5f
        ).normalized;

        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayDistance, wallLayer);

        // 레이가 벽에 충돌했는지 체크
        if (hit.collider != null)
        {
            // 벽과 충돌했을 때
            Debug.Log("벽을 감지함: " + hit.collider.name);

            // 현재 벽을 감지한 방향에서 회전
            if (velocity.x >= 0)
            {
                // 왼쪽으로 회전
                rayDirection = new Vector2(-1, velocity.y >= 0 ? 0.5f : -0.5f).normalized;
            }
            else
            {
                // 오른쪽으로 회전
                rayDirection = new Vector2(1, velocity.y >= 0 ? 0.5f : -0.5f).normalized;
            }

            rb.velocity = rayDirection * speed;
        }

        // 레이 시각화 (디버그용)
        Debug.DrawRay(rayStart, rayDirection * rayDistance, Color.red);
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
