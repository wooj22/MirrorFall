using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCheck : MonoBehaviour
{
    public float rayDistance = 5f; // ������ �ִ� �Ÿ�
    public LayerMask wallLayer; // ���� �ִ� ���̾� (Wall ���̾�)

    // Update is called once per frame
    void Update()
    {
        Vector2 rayStart = transform.position;

        Vector2 rayDirection = new Vector2(1, 0.5f).normalized;

        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayDistance, wallLayer);

        // ���̰� ���� �浹�ߴ��� üũ
        if (hit.collider != null)
        {
            // ���� �浹���� ��
            Debug.Log("���� ������: " + hit.collider.name);
        }
        else
        {
            // ���� �浹���� �ʾ��� ��
            Debug.Log("���� �������� ����");
        }

        // ���� �ð�ȭ (����׿�)
        Debug.DrawRay(rayStart, rayDirection * rayDistance, Color.red);
    }
}
