using UnityEngine;

/// <summary> Класс подбираемой жизни. </summary>
public class Heart : MonoBehaviour
{
    /// <summary> Скорость перемещения. </summary>
    [SerializeField] private float speed;
    /// <summary>  Максимальная высота полета. </summary>
    [SerializeField] private float maxFlightHeight;
    /// <summary> Аниматор. </summary>
    [SerializeField] private Animator animator;

    /// <summary> Cчетчик. </summary>
    private float counter = 0;
    /// <summary> Подобрана ли жизнь. </summary>
    private bool collected = false;

    /// <summary> Перемещение жизни. </summary>
    private void Update()
    {
        if (counter > maxFlightHeight || counter < 0)
        {
            speed *= -1;
        }

        transform.Translate(0, speed, 0);
        counter += speed;
    }

    /// <summary> Увеличение количества командных жизней при подбирании жизни. </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HeartPanel heartPanel = GameObject.Find("HeartPanel").GetComponent<HeartPanel>();

        if (collision.CompareTag("Player") && !collected)
        {
            if (!heartPanel.IsMaxHearts())
            {
                heartPanel.AddHeart();
                animator.SetBool("Collected", true);
                collected = true;
            }
        }
    }
}
