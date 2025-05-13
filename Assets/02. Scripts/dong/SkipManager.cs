using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipManager : MonoBehaviour
{
    public GameObject SkipButton;
    void Start()
    {
        SkipButton.SetActive(false);
        StartCoroutine(ShowSkipButton());
        
    }
    IEnumerator ShowSkipButton()
    {
        yield return new WaitForSeconds(5f);
        SkipButton.SetActive(true);
    }
}
