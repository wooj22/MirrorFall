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
    public float speed = 2f;            // �̵� �ӵ�
    public float findDistance = 5f;
    public float missDistance = 7f;
    public float applefindDistance = 10f;
    public float applemissDistance = 15f;
    public float attackDistance = 2f;   // ���� �Ÿ�
    public float eatDistance = 2f;   
    public Vector2 startPos;
    private int upSorting = 150;
    private int downSorting = 50;

    // raycast
    public float rayDistance = 1f; // ������ �ִ� �Ÿ�
    public LayerMask wallLayer;
    private Vector2 rayDirection;

    private bool isAvoiding = false;
    private Vector2 avoidDir = Vector2.zero;
    private float avoidCooldown = 0f;
    public float avoidDuration = 0.5f; // ȸ�� ���� ���� �ð�

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
        new Vector2(1f, 0.5f).normalized,   // ������ ��
        new Vector2(-1f, 0.5f).normalized,  // ���� ��
        new Vector2(-1f, -0.5f).normalized, // ���� �Ʒ�
        new Vector2(1f, -0.5f).normalized   // ������ �Ʒ�
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
                    // ȸ�� ������ ��
                    rb.velocity = avoidDir * speed;
                    avoidCooldown -= Time.deltaTime; // ��ٿ� �ð� ����

                    if (avoidCooldown <= 0f)
                    {
                        isAvoiding = false; // ��ٿ� ������ ȸ�� ����
                    }
                    return; // ȸ�� �߿��� �ٸ� �̵� �� ��
                }

                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToNext, rayDistance, wallLayer);

                if (hit.collider != null)
                {

                    // ���� ��� �켱���� ȸ�� ���� ����Ʈ �����
                    List<Vector2> preferredDirs = new List<Vector2>();

                    // ���� ������ ���� �Ʒ��� �� (-1, -0.5f)
                    if (Approximately(dirToNext, new Vector2(-1f, -0.5f)))
                    {
                        preferredDirs.Add(new Vector2(-1f, 0.5f).normalized); // ���� ��
                        preferredDirs.Add(new Vector2(1f, -0.5f).normalized); // ������ �Ʒ�
                    }
                    else
                    {
                        // �⺻ ȸ�� ����
                        preferredDirs.AddRange(diagonalDirs);
                    }

                    // ȸ�� ���� �˻�
                    foreach (var dir in preferredDirs)
                    {
                        RaycastHit2D check = Physics2D.Raycast(transform.position, dir, rayDistance, wallLayer);
                        if (check.collider == null)
                        {
                            avoidDir = dir;
                            isAvoiding = true;
                            avoidCooldown = avoidDuration;
                            rb.velocity = avoidDir * speed;
                            return;
                        }
                    }

                    // �� ���� ���� ���� ȸ�� ���� ����
                    foreach (var dir in diagonalDirs)
                    {
                        RaycastHit2D check = Physics2D.Raycast(transform.position, dir, rayDistance, wallLayer);
                        if (check.collider == null)
                        {
                            avoidDir = dir;
                            isAvoiding = true;
                            avoidCooldown = avoidDuration;
                            rb.velocity = avoidDir * speed;
                            return;
                        }

                    }

                    rb.velocity = Vector2.zero; // ���� �ִµ� ȸ���� ������ ������ ����
                }
                else
                {
                    // ȸ�� ���°� �ƴϸ� ���������� �̵�
                    rb.velocity = dirToNext * speed;
                }

                // ���� ���� ����
                if (Vector2.Distance(transform.position, nextPos) < 0.2f)
                {
                    currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
                }
            }
        }
        else
        {
            // �÷��̾� ����
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
                    transform.position = startPos; // ��ġ ��Ȯ�� ����
                    playerfind = false;
                }
                else
                {
                    rb.velocity = dirToStart.normalized * speed;
                }
            }
            else
            {
                // �÷��̾� ����
                Vector2 dirToPlayer = (playerPos - (Vector2)transform.position).normalized;
                rb.velocity = dirToPlayer * speed;
            }
        }
    }

    private bool Approximately(Vector2 a, Vector2 b, float threshold = 0.1f)
    {
        return Vector2.Distance(a.normalized, b.normalized) < threshold;
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

            // �� ��� üũ
            randomPos.x = Mathf.Clamp(randomPos.x, minX, maxX);
            randomPos.y = Mathf.Clamp(randomPos.y, minY, maxY);

            safety++;
            if (safety > 100) break; // ���� ���� ����
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

        // ���̰� ���� �浹�ߴ��� üũ
        if (hit.collider != null)
        {
            // ���� ���� ������ ���⿡�� ȸ��
            if (velocity.x >= 0)
            {
                // �������� ȸ��
                rayDirection = new Vector2(-1, velocity.y >= 0 ? 0.5f : -0.5f).normalized;
            }
            else
            {
                // ���������� ȸ��
                rayDirection = new Vector2(1, velocity.y >= 0 ? 0.5f : -0.5f).normalized;
            }

            rb.velocity = rayDirection * speed;
        }

        // ���� �ð�ȭ (����׿�)
        Debug.DrawRay(rayStart, rayDirection * rayDistance, Color.red);
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
