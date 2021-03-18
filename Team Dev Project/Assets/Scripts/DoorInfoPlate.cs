using UnityEngine;

public class DoorInfoPlate : MonoBehaviour
{
    [SerializeField]
    private Door door;
    [SerializeField]
    private GameObject levelStatsUI;
    private PlayerProperties playerProperties;

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerProperties = collision.GetComponent<PlayerProperties>();

        if (playerProperties.allowInput && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            playerProperties.allowInput = false;

            if (collision.GetComponent<PlayerNetworkTalker>().hasAuthority)
            {
                levelStatsUI.GetComponent<LevelStats>().SetLevelName($"Level {door.doorID + 1} Statistics");
                levelStatsUI.GetComponent<LevelStats>().SetGems(door.doorID, collision.GetComponent<PlayerNetworkTalker>().getPlayerName());
                levelStatsUI.SetActive(true);
            }
        }
    }

    public void Close()
    {
        levelStatsUI.SetActive(false);
        playerProperties.allowInput = true;
    }
}
