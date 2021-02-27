using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxFlightHeight;
    [SerializeField] private Animator animator;
    private float counter = 0;

    void Update()
    {
        if (counter > maxFlightHeight || counter < 0)
        {
            speed *= -1;
        }

        transform.Translate(0, speed, 0);
        counter += speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetBool("Collected", true);
    }

    public void PrintEvent()
    {
        Destroy(gameObject);
    }
}
