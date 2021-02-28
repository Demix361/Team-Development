using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    [SerializeField] private string collectableName;
    [SerializeField] private float speed;
    [SerializeField] private float maxFlightHeight;
    [SerializeField] private Animator animator;
    private float counter = 0;
    private bool collected = false;

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
            collision.GetComponent<PlayerNetworkTalker>().CmdIncreaseCollectable(collectableName);
            animator.SetBool("Collected", true);
            collected = true;
        }
    }

    public void DestroyEvent()
    {
        Destroy(gameObject);
    }
}
