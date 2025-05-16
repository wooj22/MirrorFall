using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToPlay : MonoBehaviour
{
    Coroutine coroutineToPlay;

    void Start()
    {
        coroutineToPlay = StartCoroutine(SceneDelay());
    }

    void StopIt()
    {
        if (coroutineToPlay != null)
            StopCoroutine(coroutineToPlay);
    }    

    IEnumerator SceneDelay()
    {
        yield return new WaitForSeconds(10f);
    }

}
