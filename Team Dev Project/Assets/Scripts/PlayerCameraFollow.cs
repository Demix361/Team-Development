using Mirror;
using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerCameraFollow : NetworkBehaviour
{
    private ICinemachineCamera iVcam;
    private Health health;
    private bool followOther = false;
    private PlayerNetworkTalker playerNetworkTalker;
    private List<GameObject> otherPlayers = new List<GameObject>();
    private List<Health> otherPlayersHealth = new List<Health>();
    private int pIndex = 0;
    private bool cameraSet = false;
    private SpectatorMode spectatorPanel;

    //[Client]
    void Start()
    {
        iVcam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        health = gameObject.GetComponent<Health>();
        playerNetworkTalker = gameObject.GetComponent<PlayerNetworkTalker>();

        var a = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in a)
        {
            //if (player.GetComponent<PlayerProperties>().playerId != gameObject.GetComponent<PlayerProperties>().playerId)
            //{
                otherPlayers.Add(player);
                otherPlayersHealth.Add(player.GetComponent<Health>());
            //}
        }

        if (hasAuthority)
        {
            if (iVcam.Follow == null)
            {
                iVcam.Follow = transform;
            }
        }

        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            spectatorPanel = GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>();
            spectatorPanel.previousButton.onClick.AddListener(PreviousPlayerCamera);
            spectatorPanel.nextButton.onClick.AddListener(NextPlayerCamera);
        }
    }

    private void FixedUpdate()
    {
        if (otherPlayers.Count > 0 && !health.alive)
        {
            if (!cameraSet)
            {
                Debug.Log("HERE");
                NextPlayerCamera();
                cameraSet = true;
            }
        }
    }

    //[Client]
    public void NextPlayerCamera()
    {
        int n = otherPlayers.Count;
        Debug.Log($"OTHER PLAYERS LEN: {n}");

        for (int i = 0; i < n; i++)
        {
            pIndex += 1;
            if (pIndex > otherPlayers.Count - 1)
            {
                pIndex = 0;
            }

            if (otherPlayersHealth[pIndex].alive)
            {
                break;
            }
        }
        Debug.Log($"PINDEX: {pIndex}");

        iVcam.Follow = otherPlayers[pIndex].transform;
    }

    //[Client]
    public void PreviousPlayerCamera()
    {
        int n = otherPlayers.Count;

        for (int i = 0; i < n; i++)
        {
            pIndex -= 1;
            if (pIndex < 0)
            {
                pIndex = n - 1;
            }

            if (otherPlayersHealth[pIndex].alive)
            {
                break;
            }
        }

        iVcam.Follow = otherPlayers[pIndex].transform;
    }
}
