using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CheckPoint : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] public int checkpointID;
    [SerializeField] public Transform spawnPoint;
    [SerializeField] private Popup popup;

    private PlayerProperties playerProperties;
    [SyncVar] public bool unlocked = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerProperties = collision.GetComponent<PlayerProperties>();

        if (!unlocked && playerProperties.allowInput && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            CmdUnlockCheckpoint();
            CmdAddAllHearts();
            collision.GetComponent<Health>().CmdHealAllMax();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !unlocked)
        {
            popup.SetPopup(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            popup.SetPopup(false);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdAddAllHearts()
    {
        GameObject.Find("HeartPanel").GetComponent<HeartPanel>().AddAllHearts();
    }

    [Command(requiresAuthority = false)]
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
