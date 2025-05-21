using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiledItem : MonoBehaviour
{
    [SerializeField] GameObject uiCanvas;
    [SerializeField] public string itemId;

    private void Start()
    {
        if (GameManager.Instance.HasCollectedItem(itemId))
        {
            Destroy(gameObject);
        }
    }

    public void InteractionUIOn() { uiCanvas.SetActive(true); }
    public void InteractionUIOff() { uiCanvas.SetActive(false); }
}
