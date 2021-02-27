using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : MonoBehaviour
{
    [SerializeField] public string sceneName;
    [SerializeField] private Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetBool("DoorOpen", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        animator.SetBool("DoorOpen", false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            //CmdChangeScene();
            //Room.ServerChangeScene(sceneName);
            Debug.Log(collision.name);
            Debug.Log(collision.attachedRigidbody);
            Debug.Log(collision.gameObject);
            Debug.Log(collision.GetType());
            Debug.Log(collision.name);
            Debug.Log(collision.name);
            Debug.Log(collision.name);
            Debug.Log(collision.name);

            collision.GetComponent<PlayerNetworkTalker>().CmdChangeScene(sceneName);

            //var pl = collision.pare
        }
    }


}
