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

    // PausePanner On : 인게임 일시정지
    private void PausePannerOn()
    {
        pausePannel.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    // PausePanner Off : 인게임 이어하기
    public void PausePannerOff()
    {
        pausePannel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}