using UnityEngine;
/// <summary>
/// Класс точки появление персонажей игроков.
/// </summary>
/// <remarks>
/// Добавляет и удаляет точки появления персонажей игроков.
/// </remarks>
public class PlayerSpawnPoint : MonoBehaviour
{
    /// <summary>
    /// Добавляет точки появления персонажей игроков.
    /// </summary>
    private void Awake()
    {
        PlayerSpawnSystem.AddSpawnPoint(transform);
    }
    /// <summary>
    /// Удаляет точки появления персонажей игроков.
    /// </summary>
    private void OnDestroy()
    {
        PlayerSpawnSystem.RemoveSpawnPoint(transform);
    }
    
    void OnDrawGizmos()
    {
        float radius = (float)0.3;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
