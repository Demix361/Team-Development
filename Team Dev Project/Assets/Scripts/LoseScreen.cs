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

    private MyNetworkManager room;
    private MyNetworkManager Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }

            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }

    private void Start()
    {
        //HubButton.onClick.AddListener(ReturnToHub);
        //RestartButton.onClick.AddListener(RestartLevel);
    }

    [Command]
    public void CmdEnableLoseScreen()
    {
        Debug.Log("Hello?");
        var a = GameObject.FindGameObjectsWithTag("Player");
        int count = 0;
        foreach (GameObject player in a)
        {
            Health health = player.GetComponent<Health>();
            if (!health.IsAlive())
            {
                count += 1;
            }
        }

        Debug.Log($"LOSESCREEN: {count} / {a.Length}");

        if (count == a.Length)
        {
            ServerEnableLoseScreen();
        }
    }

    [Server]
    private void ServerEnableLoseScreen()
    {
        RpcEnableLoseScreen();
    }

    [ClientRpc]
    public void RpcEnableLoseScreen()
    {
        LoseScreenOverlay.SetActive(true);
    }

    public void ReturnToHub()
    {
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerNetworkTalker>().CmdChangeScene("HubScene");
    }

    public void RestartLevel()
    {
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerNetworkTalker>().CmdChangeScene(SceneManager.GetActiveScene().name);
    }
    
    [Command]
    public void CmdChangeScene(string sceneName)
    {
        Room.ServerChangeScene(sceneName);
    }

    public void SetLoseScreen(bool state)
    {
        LoseScreenOverlay.SetActive(state);
    }

}
