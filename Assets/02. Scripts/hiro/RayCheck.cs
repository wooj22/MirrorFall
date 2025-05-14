using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCheck : MonoBehaviour
{
    public float rayDistance = 5f; // 레이의 최대 거리
    public LayerMask wallLayer; // 벽이 있는 레이어 (Wall 레이어)

    // Update is called once per frame
    void Update()
    {
        Vector2 rayStart = transform.position;

        Vector2 rayDirection = new Vector2(1, 0.5f).normalized;

        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayDistance, wallLayer);

        // 레이가 벽에 충돌했는지 체크
        if (hit.collider != null)
        {
            // 벽과 충돌했을 때
            Debug.Log("벽을 감지함: " + hit.collider.name);
        }
        else
        {
            // 벽과 충돌하지 않았을 때
            Debug.Log("벽을 감지하지 못함");
        }

        // 레이 시각화 (디버그용)
        Debug.DrawRay(rayStart, rayDirection * rayDistance, Color.red);
    }
}
