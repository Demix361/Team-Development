using UnityEngine;

/// <summary>
/// Класс алмаза
/// </summary>
public class Diamond : MonoBehaviour
{
    /// <summary>
    /// ID алмаза, должен быть уникальным.
    /// </summary>
    [SerializeField] public int gemID;
    /// <summary>
    /// Название алмаза.
    /// </summary>
    [SerializeField] public string collectableName;
    /// <summary>
    /// Скорость перемещения алмаза.
    /// </summary>
    [SerializeField] private float speed;
    /// <summary>
    /// Максимальный предел перемещения алмаза.
    /// </summary>
    [SerializeField] private float maxFlightHeight;
    /// <summary>
    /// Найден ли алмаз.
    /// </summary>
    [SerializeField] public bool found;
    /// <summary>
    /// Аниматор алмаза.
    /// </summary>
    [SerializeField] private Animator animator;
    
    /// <summary>
    /// Таймер.
    /// </summary>
    private float counter = 0;
    /// <summary>
    /// Найден ли алмаз.
    /// </summary>
    private bool collected = false;
    /// <summary>
    /// SpriteRenderer алмаза.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Делает уже найденные алмазы полупрозрачными.
    /// </summary>
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

    /// <summary>
    /// Перемещает алмаз по вертикали.
    /// </summary>
    void Update()
    {
        if (counter > maxFlightHeight || counter < 0)
        {
            speed *= -1;
        }

        transform.Translate(0, speed, 0);
        counter += speed;
    }

    /// <summary>
    /// При столкновении с игроком делает алмаз невидимым.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collected)
        {
            collision.GetComponent<PlayerNetworkTalker>().CmdIncreaseCollectable(collectableName);
            animator.SetBool("Collected", true);
            collected = true;
            found = true;
        }
    }

    /// <summary>
    /// Делает уже найденные алмазы полупрозрачными.
    /// </summary>
    public void SetGem()
    {
        found = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = (float)0.5;
        spriteRenderer.color = color;
    }
}
