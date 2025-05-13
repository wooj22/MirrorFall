using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDirector : MonoBehaviour
{
    public static SceneDirector Instance { get; private set; }
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

    public void ToPlay() { StartCoroutine(SceneDelay()); }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    IEnumerator SceneDelay()
    {
        yield return new WaitForSeconds(10f);
        //SceneSwitch.Instance.SceneSwithcing(sceneName);
    }
}
