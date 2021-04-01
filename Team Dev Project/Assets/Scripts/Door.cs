﻿using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    [SerializeField] public string sceneName;
    [SerializeField] private string tableString;
    [SerializeField] private TMP_Text tableText;
    [SerializeField] private Animator animator;
    [SerializeField] private bool locked;
    [SerializeField] private GameObject lockSprite;
    [SerializeField] public int doorID;
    [SerializeField] private Popup popup;
    private PlayerProperties playerProperties;

    private void Start()
    {
        tableText.SetText(tableString);
        lockSprite.SetActive(locked);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !locked)
        {
            animator.SetBool("DoorOpen", true);
            popup.SetPopup(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !locked)
        {
            animator.SetBool("DoorOpen", false);
            popup.SetPopup(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerProperties = collision.GetComponent<PlayerProperties>();

        if (playerProperties.allowInput && !locked && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            collision.GetComponent<PlayerNetworkTalker>().CmdSetLevelID(doorID);

            collision.GetComponent<PlayerNetworkTalker>().CmdChangeScene(sceneName);
        }
    }

    public void SetLock(bool state)
    {
        locked = state;
        lockSprite.SetActive(locked);
    }
}
