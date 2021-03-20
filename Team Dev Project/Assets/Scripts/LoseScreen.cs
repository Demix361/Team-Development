using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class LoseScreen : NetworkBehaviour
{
    [SerializeField] private GameObject LoseScreenOverlay;
    [SerializeField] private Button HubButton;
    [SerializeField] private Button RestartButton;

    [ClientRpc]
    public void RpcEnableLoseScreen()
    {
        LoseScreenOverlay.SetActive(true);
    }

    public void ReturnToHub()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in a)
        {
            player.GetComponent<PlayerNetworkTalker>().CmdChangeScene("HubScene");
        }
    }

    public void RestartLevel()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in a)
        {
            player.GetComponent<PlayerNetworkTalker>().CmdChangeScene(SceneManager.GetActiveScene().name);
        }
    }
}
