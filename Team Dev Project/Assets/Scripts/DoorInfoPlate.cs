using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInfoPlate : MonoBehaviour
{
    [SerializeField]
    private Door door;
    [SerializeField]
    private GameObject levelStatsUI;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact") || Input.GetButton("Interact"))
        {
            if (collision.GetComponent<PlayerNetworkTalker>().hasAuthority)
            {
                levelStatsUI.GetComponent<LevelStats>().SetLevelName($"Level {door.doorID + 1} Statistics");
                levelStatsUI.GetComponent<LevelStats>().SetGems(door.doorID, collision.GetComponent<PlayerNetworkTalker>().getPlayerName());
                levelStatsUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerNetworkTalker>().hasAuthority)
        {
            levelStatsUI.SetActive(false);
        }
    }
}
