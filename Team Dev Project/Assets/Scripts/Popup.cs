using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Класс всплывающего уведомления.
/// </summary>
/// <remarks>
/// Начинает и заканчивает отсчет перед всплывающим уведомлением. Вызывает всплывающее уведомление.
/// </remarks>
public class Popup : MonoBehaviour
{
    /// <summary>
    /// Значение задержки перед уведомлением
    /// </summary>
    [SerializeField] private float popupDelay;
    /// <summary>
    /// Объект появляющийся при уведомлении.
    /// </summary>
    [SerializeField] private GameObject popupSprite;
    /// <summary>
    /// Счетчик.
    /// </summary>
    private float counter = 0;
    /// <summary>
    /// Состояние начала отчета.
    /// </summary>
    private bool needCount;
    /// <summary>
    /// Объявляет значение состояния счетчика.
    /// </summary>
    private void Start()
    {
        needCount = false;
    }
    /// <summary>
    /// Обновляет значение состояния счетчика.
    /// </summary>
    /// <param name="state">Состояние игрового персонажи</param>
    public void SetPopup(bool state)
    {
        if (state)
        {
            needCount = true;
        }
        else
        {
            popupSprite.SetActive(false);
            needCount = false;
            counter = 0;
        }
    }
    /// <summary>
    /// Обновляет значение счетчика и состояния счетчика.
    /// </summary>
    private void Update()
    {
        if (needCount)
        {
            counter += Time.deltaTime;

            if (counter > popupDelay)
            {
                popupSprite.SetActive(true);
                needCount = false;
                counter = 0;
            }
        }
    }
}
