using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    private void Start()
    {
        Destroy(this.gameObject, 12f);  // ÀÚ°¡¼Ò¸ê
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}
