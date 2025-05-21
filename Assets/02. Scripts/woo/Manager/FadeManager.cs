using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    private PlayerController _player;

    public static FadeManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FadeIn();
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void FadeIn() { StartCoroutine(FadeInCo()); }
    public void FadeOut() { StartCoroutine(FadeOutCo()); }
    public void FadeOutSceneChange(string name) { StartCoroutine(FadeOutSceneSwitch(name)); }

    /// FadeIn 
    private IEnumerator FadeInCo()
    {
        float fadeCount = 1;
        fadeImage.gameObject.SetActive(true);

        while (fadeCount > 0.001f)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        fadeImage.gameObject.SetActive(false);
    }

    //. FadeOut
    private IEnumerator FadeOutCo()
    {
        if(_player != null) _player.walkLock = true; // 페이드동안 플레이어가있다면 이동 잠금

        float fadeCount = 0f;
        fadeImage.gameObject.SetActive(true);

        while (fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        if (_player != null) _player.walkLock = false;       // 잠금 해제
    }

    /// Screen FadeOut & Goto Scene
    private IEnumerator FadeOutSceneSwitch(string scenename)
    {
        if (_player != null) _player.walkLock = true;        // 페이드동안 플레이어가있다면 이동 잠금

        fadeImage.gameObject.SetActive(true);
        Time.timeScale = 1;

        float fadeCount = 0;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        SceneSwitch.Instance.SceneSwithcing(scenename);
        if (_player != null) _player.walkLock = false;       // 잠금 해제
        yield return null;
    }
}
