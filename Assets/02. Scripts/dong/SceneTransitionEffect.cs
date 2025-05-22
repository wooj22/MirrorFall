using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionEffect : MonoBehaviour
{
    public Image img1;
    public GameObject img2Group;        // img2 + �ؽ�Ʈ ���� �׷�
    public CanvasGroup img2CanvasGroup; // img2Group�� ���� CanvasGroup
    public Image fadePanel;

    private void Start()
    {
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(PlayTransition());
    }

    private IEnumerator PlayTransition()
    {
        // �ʱ� ����
        img1.color = new Color(1, 1, 1, 1);
        img2CanvasGroup.alpha = 0f;
        img2Group.SetActive(false);
        fadePanel.color = new Color(0, 0, 0, 0);
        SoundManager2.Instance.PlaySFX("SFX_Jumpscare");

        yield return new WaitForSeconds(1f);

        // ���̵� �ƿ�
        yield return StartCoroutine(FadeAlpha(fadePanel, 0f, 1f, 1f));

        // �̹��� ��ü
        img1.gameObject.SetActive(false);
        img2Group.SetActive(true);

        // ���̵� �� + img2 �׷� �Բ�
        yield return StartCoroutine(FadeCanvasGroup(img2CanvasGroup, 0f, 1f, 1f));
        yield return StartCoroutine(FadeAlpha(fadePanel, 1f, 0f, 1f));

        SoundManager2.Instance.PlayOneShotBGM("BGM_Gameover");
    }

    private IEnumerator FadeAlpha(Image image, float start, float end, float duration)
    {
        float time = 0f;
        Color col = image.color;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(start, end, time / duration);
            image.color = new Color(col.r, col.g, col.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        image.color = new Color(col.r, col.g, col.b, end);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            group.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        group.alpha = end;
    }
}
