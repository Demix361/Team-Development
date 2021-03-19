using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxFlightHeight;
    [SerializeField] private Animator animator;
    private float counter = 0;
    private bool collected = false;
    [SerializeField] private HeartPanel heartPanel;

    

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
        if (!collected)
        {
            heartPanel.AddHeart();
            animator.SetBool("Collected", true);
            collected = true;
        }
    }
}
