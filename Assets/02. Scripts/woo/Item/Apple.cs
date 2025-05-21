using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public bool isGround = false;

    [SerializeField] private float stopThreshold = 0.01f;   // 움직임 판정 민감도
    [SerializeField] private float checkDelay = 0.2f;       // 움직임 체크 주기

    private Vector3 previousPosition;
    private float checkTimer = 0f;
    private Animator effectAni;
    private SpriteRenderer sp;


    private void Start()
    {
        effectAni = GetComponentInChildren<Animator>();
        sp = GetComponent<SpriteRenderer>();
        previousPosition = transform.position;
        Destroy(this.gameObject, 8f);
    }

    private void Update()
    {
        if (isGround) return;

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkDelay)
        {
            float distanceMoved = (transform.position - previousPosition).sqrMagnitude;

            // 1) 바닥에 떨어졌을 경우
            if (distanceMoved < stopThreshold * stopThreshold)
            {
                isGround = true;
                sp.enabled = false;
                effectAni.SetBool("isHit", true);
                SoundManager2.Instance.PlaySFX("SFX_Apple_Fall");
            }

            previousPosition = transform.position;
            checkTimer = 0f;
        }
    }

    // WallFull 이랑 MapBorder랑만 충돌
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 2) 어딘가에 닿았을을 경우 (강제 종료)
        isGround = true;
        sp.enabled = false;
        effectAni.SetBool("isHit", true);
        SoundManager2.Instance.PlaySFX("SFX_Apple_Fall");
    }
}
