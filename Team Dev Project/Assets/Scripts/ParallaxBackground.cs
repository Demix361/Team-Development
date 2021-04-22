using UnityEngine;

/// <summary>
/// Класс заднего фона.
/// </summary>
/// <remarks>
/// Перемещает задний фон за камерой.
/// </remarks>
public class ParallaxBackground : MonoBehaviour
{
    /// <summary>
    /// Transform камеры.
    /// </summary>
    public Transform cameraTransform;
    /// <summary>
    /// Transform объекта.
    /// </summary>
    public Transform objectTransform;
    /// <summary>
    /// Vector2 коэффиицентов умножения перемещения фона.
    /// </summary>
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    /// <summary>
    /// Vector3 прошлой позиции камеры.
    /// </summary>
    private Vector3 lastCameraPosition;
    /// <summary>
    /// Размер текстуры по горизонтали.
    /// </summary>
    private float textureUnitSizeX;


    private void Start()
    {
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width * objectTransform.localScale.x / sprite.pixelsPerUnit;
    }

    private void FixedUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
        lastCameraPosition = cameraTransform.position;

        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }
    }
}
