using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{
    public bool step1_isZone1;
    public bool step2_isBright;
    public bool step3_isAppleThrow;
    public bool step4_isHide;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Zone1_Trigger") step1_isZone1 = true;
        Debug.Log(collision.gameObject.name);
    }
}
