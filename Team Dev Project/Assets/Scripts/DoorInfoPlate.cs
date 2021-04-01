using UnityEngine;

public class DoorInfoPlate : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private GameObject levelStatsUI;
    [SerializeField] private Popup popup;
    private PlayerProperties playerProperties;

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerProperties = collision.GetComponent<PlayerProperties>();

        if (playerProperties.allowInput && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            playerProperties.allowInput = false;

            if (collision.GetComponent<PlayerNetworkTalker>().hasAuthority)
            {
                levelStatsUI.GetComponent<LevelStats>().SetLevelName($"Level {door.doorID + 1}");
                levelStatsUI.GetComponent<LevelStats>().SetGems(door.doorID, collision.GetComponent<PlayerNetworkTalker>().getPlayerName());
                levelStatsUI.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            popup.SetPopup(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            popup.SetPopup(false);
    }

    public void Close()
    {
        levelStatsUI.SetActive(false);
        playerProperties.allowInput = true;
    }
}
