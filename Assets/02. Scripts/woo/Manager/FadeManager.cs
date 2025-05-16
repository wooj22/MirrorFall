using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] Image fadeImage;

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
    }

    public void FadeIn() { StartCoroutine(FadeInCo()); }
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

    /// Screen FadeOut & Goto Scene
    private IEnumerator FadeOutSceneSwitch(string scenename)
    {
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
        yield return null;
    }
}
