using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    void Start()
    {
        // ���� �ſ����� ����� ��ũ��Ʈ�ε�, Ʃ�丮�󿡼� �ⱸ�� �����Ѿ��ؼ� �̷��� ����
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().SetCurSceneMirrorPiece(this.gameObject);
    }
}
