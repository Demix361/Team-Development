using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    [SerializeField] public int gemID;
    [SerializeField] public string collectableName;
    [SerializeField] private float speed;
    [SerializeField] private float maxFlightHeight;
    [SerializeField] public bool found;
    [SerializeField] private Animator animator;
    private float counter = 0;
    private bool collected = false;
    private SpriteRenderer spriteRenderer;

    /*
    private void Start()
    {
        if (found)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Color color = spriteRenderer.color;
            color.a = (float)0.5;
            spriteRenderer.color = color;
        }
    }
    */

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
            found = true;
        }
    }

    public void SetGem()
    {
        found = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = (float)0.5;
        spriteRenderer.color = color;

    }


}
