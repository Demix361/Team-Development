using UnityEngine;

public class DeathPoint : MonoBehaviour
{
    private void Awake()
    {
        PlayerSpawnSystem.AddDeathPoint(transform);
    }

    void OnDrawGizmos()
    {
        float radius = (float)0.3;
        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
