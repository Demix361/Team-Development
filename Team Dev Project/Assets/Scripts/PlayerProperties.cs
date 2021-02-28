using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    [SerializeField] private int maxHP;
    private int currentHP;
    [SerializeField] private float spikesHitCD;
    private float spikesHitTimer = 0;
    [SerializeField] Animator animator;
    private Rigidbody2D m_Rigidbody2D;
    private float spikesJumpForce = 200;

    private void Start()
    {
        currentHP = maxHP;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (spikesHitTimer > 0)
        {
            spikesHitTimer -= Time.deltaTime;
        }
    }

    private void DecreaseHP(int delta)
    {
        currentHP -= delta;
        Debug.Log($"HP: {currentHP}");
    }

    public void getHitFromSpikes(int damage)
    {
        if (spikesHitTimer <= 0)
        {
            DecreaseHP(damage);
            animator.SetTrigger("Hit");
            spikesHitTimer = spikesHitCD;

            m_Rigidbody2D.AddForce(new Vector2(0f, spikesJumpForce));
        }
    }
}
