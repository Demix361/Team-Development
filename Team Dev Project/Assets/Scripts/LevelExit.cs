using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField]
    private GameObject endScreen;

    private PlayerNetworkTalker playerNetworkTalker;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButton("Interact"))
        {
            playerNetworkTalker = collision.GetComponent<PlayerNetworkTalker>();
            endScreen.SetActive(true);

            // Сохранение
            playerNetworkTalker.CmdSaveLevel();
        }
    }

    public void ButtonOK()
    {
        playerNetworkTalker.CmdChangeScene("HubScene");
    }
}
