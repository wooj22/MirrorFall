using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    public GameObject OptionPanel;

    void Start()
    {
        OptionPanel.SetActive(false);
    }

    // Update is called once per frame
    public void OnClickOptionButton()
    {
        OptionPanel.SetActive(true);
    }
    public void OnClickOptionCancelButton()
    {
        OptionPanel.SetActive(false);
    }
}
