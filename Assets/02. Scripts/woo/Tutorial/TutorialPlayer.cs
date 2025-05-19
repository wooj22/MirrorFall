using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{
    public bool step1_isZone1;
    public bool step2_isBright;
    public bool step3_isAppleThrow;
    public bool step4_isHide;
    public bool step5_isZone5;
    public bool step6_isHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Zone1_Trigger") step1_isZone1 = true;
        if (collision.gameObject.name == "Zone5_Trigger") step5_isZone5 = true;
    }
}
