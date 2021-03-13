using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInfoPlate : MonoBehaviour
{
    [SerializeField]
    private Door door;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact") || Input.GetButton("Interact"))
        {
            Debug.Log($"INFO PLATE OF DOOR: {door.doorID}");
        }
    }
}
