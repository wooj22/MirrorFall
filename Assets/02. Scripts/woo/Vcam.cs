using UnityEngine;
using Cinemachine;

public class Vcam : MonoBehaviour
{
    void Start()
    {
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = GameObject.FindWithTag("Player").transform;
    }
}
