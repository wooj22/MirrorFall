using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCheat : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] KeyCode cheatKey;
    private PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(cheatKey))
        {
            player.isCheatMode = !player.isCheatMode;
            if(player.isCheatMode) text.gameObject.SetActive(true);
            else text.gameObject.SetActive(false);
        }
    }
}
