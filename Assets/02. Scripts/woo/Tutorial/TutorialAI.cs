using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAI : MonoBehaviour
{
    public Transform playerPos;
    public Transform reSpawnPos;
    public Transform movePos;
    [SerializeField] private float speed;
    [SerializeField] private float attackLimit = 0.5f;
    
    // Player �߰�
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

    // ��ǥ ��ġ�� �̵���Ŵ (apple, �÷��̾� ������ respawnPos)
    public void MoveToPos(Transform pos)
    {
        Vector2 direction = (pos.position - transform.position).normalized;
        transform.position += (Vector3)(direction * speed * 3.5f * Time.deltaTime);
    }
}
