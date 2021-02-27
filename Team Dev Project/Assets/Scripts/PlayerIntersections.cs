using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerIntersections : NetworkBehaviour
{
    private GameObject[] doors;

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
        doors = GameObject.FindGameObjectsWithTag("Door");
        //networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
    }

    void Update()
    {
        foreach (GameObject door in doors)
        {
            float radius = door.GetComponent<Door>().radius;
            string sceneName = door.GetComponent<Door>().sceneName;

            if (Mathf.Abs(transform.position.x - door.transform.position.x) < radius &&
                Mathf.Abs(transform.position.y - door.transform.position.y) < radius &&
                Input.GetButtonDown("Interact"))
            {
                Debug.Log("Door");
                CmdChangeScene(sceneName);
            }
        }
    }

    [Command]
    public void CmdChangeScene(string name)
    {
        if (!true)
        {
            return;
        }

        Room.ServerChangeScene(name);
    }


}
