using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_WarpMirror : MonoBehaviour
{
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private string nextSeneName;
    [SerializeField] TutorialManager manager;
    [SerializeField] private bool isInPlayer = false;
    [SerializeField] private bool isInteractionKey = false;

    public void InterationUIOn() { uiCanvas.SetActive(true); }
    public void InterationUIOff() { uiCanvas.SetActive(false); }

    private void Update()
    {
        isInteractionKey = Input.GetKey(KeyCode.F);

        if (isInteractionKey && isInPlayer) 
            manager.TutorialClear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InterationUIOn();
            isInPlayer = true;        
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InterationUIOff();
            isInPlayer = false;
        }
    }
}
