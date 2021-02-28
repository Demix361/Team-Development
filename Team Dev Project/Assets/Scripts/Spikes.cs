using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float attackCooldown;

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.GetComponent<PlayerProperties>().getHitFromSpikes(damage);
    }
}
