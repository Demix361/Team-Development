using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CheckPoint : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] public int checkpointID;
    [SerializeField] public Transform spawnPoint;

    private PlayerProperties playerProperties;
    [SyncVar] public bool unlocked = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerProperties = collision.GetComponent<PlayerProperties>();

        if (!unlocked && playerProperties.allowInput && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            CmdUnlockCheckpoint();
        }
    }

    [Command(ignoreAuthority = true)]
    private void CmdUnlockCheckpoint()
    {
        unlocked = true;
        RpcUnlockChecpoint();
    }

    [ClientRpc]
    private void RpcUnlockChecpoint()
    {
        animator.SetBool("Set", true);
    }
}
