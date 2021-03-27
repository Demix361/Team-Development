using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private PlayerProperties playerProperties;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("FLAG FLAG");
        playerProperties = collision.GetComponent<PlayerProperties>();

        if (playerProperties.allowInput && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            animator.SetBool("Set", true);
        }
    }
}
