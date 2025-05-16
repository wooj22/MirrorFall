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

    private void Start()
    {
        previousPosition = transform.position;
        Destroy(this.gameObject, 12f);              // 12초 후 자가 소멸
    }

    private void Update()
    {
        if (isGround) return;

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkDelay)
        {
            float distanceMoved = (transform.position - previousPosition).sqrMagnitude;

            if (distanceMoved < stopThreshold * stopThreshold)
            {
                isGround = true;
            }

            previousPosition = transform.position;
            checkTimer = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //GetComponent<CircleCollider2D>().enabled = false;
            //isGround = true;        // 벽에 닿았을때 강제 종료
            Debug.Log("벽에 닿음 사과가 그래서 멈춤");
        }
    }
}
