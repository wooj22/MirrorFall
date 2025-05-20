using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> footstepClips;

    [SerializeField] float minInterval = 0.2f;
    [SerializeField] float maxInterval = 0.6f;

    private float timer = 0f;

    public void PlayFootstep(float speed)
    {
        // �ʹ� ������ �߼Ҹ� �ȳ�
        if (speed < 0.1f)
        {
            timer = 0f;
            return;
        }

        float interval = Mathf.Lerp(maxInterval, minInterval, speed / 5f); // �ӵ��� ���� �߼Ҹ� �ֱ� ����
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            PlayRandomClip();
            timer = 0f;
        }
    }

    private void PlayRandomClip()
    {
        if (footstepClips.Count == 0 || audioSource == null) return;

        int index = Random.Range(0, footstepClips.Count);
        audioSource.PlayOneShot(footstepClips[index]);
    }
}
