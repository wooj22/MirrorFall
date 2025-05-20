using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialAI : MonoBehaviour
{
    public Transform playerPos;
    public Transform reSpawnPos;
    public Transform movePos;
    [SerializeField] private float speed;
    [SerializeField] private float attackLimit = 0.5f;

    public string anim_cur = "Idle";
    private Animator ani;
    private SpriteRenderer spr;
    private Vector2 lastPosition;
    void Start()
    {
        ani = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
    }
    void Update()
    {
        Playani();
        lastPosition = transform.position;
    }
    // Player 추격
    public void Trace()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerPos.position);

        if (distanceToPlayer > attackLimit)
        {
            Vector2 direction = (playerPos.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
        else
        {
            transform.position = reSpawnPos.position;
            playerPos.gameObject.GetComponent<PlayerController>().Hit("K");
        }
    }

    // 목표 위치로 이동시킴 (apple, 플레이어 은신중 respawnPos)
    public void MoveToPos(Transform pos)
    {
        Vector2 direction = (pos.position - transform.position).normalized;
        transform.position += (Vector3)(direction * speed * 3.5f * Time.deltaTime);
    }

    void Playani()
    {
        Vector2 moveDir = ((Vector2)transform.position - lastPosition).normalized;

        if (moveDir == Vector2.zero) {
            SetAnimation("Idle");
            spr.flipX = true;
            return;
        }

        spr.flipX = moveDir.x < 0;
        SetAnimation(moveDir.y >= 0 ? "RightTop" : "RightBot");
    }

    private void SetAnimation(string anim)
    {
        if (anim_cur == anim) return;
        anim_cur = anim;
        ani.Play(anim);
    }
}
