using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerProperties : NetworkBehaviour
{
    private Health health;
    [SerializeField] private float spikesHitCD;
    private float spikesHitTimer = 0;
    [SerializeField] Animator animator;
    private Rigidbody2D m_Rigidbody2D;
    private float spikesJumpForce = 600;

    [SyncVar] 
    public int playerId;

    private void Start()
    {
        health = GetComponent<Health>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (spikesHitTimer > 0)
        {
            spikesHitTimer -= Time.deltaTime;
        }
    }

    public void getHitFromSpikes(int damage)
    {
        if (spikesHitTimer <= 0)
        {
            health.CmdDealDamage(damage);
            animator.SetTrigger("Hit");
            spikesHitTimer = spikesHitCD;

            m_Rigidbody2D.AddForce(new Vector2(0f, - m_Rigidbody2D.velocity.y) * 100);
        }
    }
}
