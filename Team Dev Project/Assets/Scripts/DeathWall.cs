using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Health>().CmdInstantDie();
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Vector3 left = new Vector3(transform.position.x - boxCollider.size.x / 2, transform.position.y, 0);
        Vector3 right = new Vector3(transform.position.x + boxCollider.size.x / 2, transform.position.y, 0);
        Gizmos.DrawLine(left, right);
        
    }
}
