using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{
    public bool isZone1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Zone1_Trigger") isZone1 = true;
        Debug.Log(collision.gameObject.name);
    }
}
