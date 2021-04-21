using UnityEngine;

/// <summary>
/// Класс точки смерти.
/// </summary>
/// <remarks>
/// Точка, куда будет перемещен игрок после смерти.
/// </remarks>
public class DeathPoint : MonoBehaviour
{
    /// <summary>
    /// Добавляет данную точку в <see cref="PlayerSpawnSystem"/>
    /// </summary>
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
