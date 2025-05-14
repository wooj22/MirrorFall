using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private GameObject pausePannel;

    public void Start()
    {
        pausePannel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (Time.timeScale == 1) PausePannerOn();
            else PausePannerOff();
        }
    }

    // PausePanner On : �ΰ��� �Ͻ�����
    private void PausePannerOn()
    {
        pausePannel.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    // PausePanner Off : �ΰ��� �̾��ϱ�
    public void PausePannerOff()
    {
        pausePannel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}