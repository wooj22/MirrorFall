using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public bool isGround = false;
    private Vector3 previousPosition;
    private float stopThreshold = 0.001f;

    private void Start()
    {
        previousPosition = transform.position;
        Destroy(this.gameObject, 12f);  // 자가소멸
    }

    private void Update()
    {
        // 더이상 안움직일떄 isGournd true
        if (!isGround)
        {
            float distanceMoved = (transform.position - previousPosition).sqrMagnitude;
            if (distanceMoved < stopThreshold * stopThreshold) isGround = true;

            previousPosition = transform.position;
        }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}
