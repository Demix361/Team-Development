using UnityEngine;

/// <summary>
/// Класс стены смерти
/// </summary>
/// <remarks>
/// Убивает игрока при столкновении с собой
/// </remarks>
public class DeathWall : MonoBehaviour
{
    /// <summary>
    /// BoxCollider2D стены
    /// </summary>
    [SerializeField] private BoxCollider2D boxCollider;

    /// <summary>
    /// Наносит максимальный урон игроку при столкновении.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Health>().CmdDealMaxDamage();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 left = new Vector3(transform.position.x - boxCollider.size.x / 2, transform.position.y, 0);
        Vector3 right = new Vector3(transform.position.x + boxCollider.size.x / 2, transform.position.y, 0);
        Gizmos.DrawLine(left, right);
    }
}
