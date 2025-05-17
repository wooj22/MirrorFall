using UnityEngine;

public class Ysort_Map : MonoBehaviour
{
    void Awake()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
