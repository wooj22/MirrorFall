using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{
    public bool step1_isZone1;
    public bool step2_isBright;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Zone1_Trigger") step1_isZone1 = true;
        Debug.Log(collision.gameObject.name);
    }
}
