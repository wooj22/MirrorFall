using UnityEngine;
using Cinemachine;

public class Vcam : MonoBehaviour
{
    CinemachineVirtualCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = GameObject.FindWithTag("Player").transform;

        Invoke(nameof(Test), 0.5f);
    }

    void Test()
    {
        vcam.Follow = GameObject.FindWithTag("Player").transform;
    }
}
