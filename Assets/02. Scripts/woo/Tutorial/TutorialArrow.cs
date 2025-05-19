using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    void Start()
    {
        // 원래 거울조각 방향용 스크립트인데, 튜토리얼에서 출구를 가리켜야해서 이렇게 만듦
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().SetCurSceneMirrorPiece(this.gameObject);
    }
}
