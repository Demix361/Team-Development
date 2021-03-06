using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    [SerializeField] public string sceneName;
    [SerializeField] private string tableString;
    [SerializeField] private TMP_Text tableText;
    [SerializeField] private Animator animator;
    [SerializeField] private bool locked;
    [SerializeField] private GameObject lockSprite;
    [SerializeField] private int doorID;

    private void Start()
    {
        tableText.SetText(tableString);
        lockSprite.SetActive(locked);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!locked)
        {
            animator.SetBool("DoorOpen", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!locked)
        {
            animator.SetBool("DoorOpen", false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!locked && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            collision.GetComponent<PlayerNetworkTalker>().CmdChangeScene(sceneName);
            if (doorID != -1)
            {
                collision.GetComponent<PlayerNetworkTalker>().SetLevelId(doorID);
            }
        }
    }

    public void SetLock(bool state)
    {
        locked = state;
    }

    public void Unlock()
    {
        locked = false;
    }

    public void Lock()
    {
        locked = true;
    }
}
