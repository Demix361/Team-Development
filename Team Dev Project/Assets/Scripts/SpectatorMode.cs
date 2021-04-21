using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// Класс режима наблюдателя.
/// </summary>
/// <remarks>
/// 
/// </remarks>
public class SpectatorMode : MonoBehaviour
{
    /// <summary>
    /// Переход на предыдущего игрока.
    /// </summary>
    [SerializeField] public Button previousButton;
    /// <summary>
    /// Переход на следующего игрока.
    /// </summary>
    [SerializeField] public Button nextButton;
    /// <summary>
    /// Таблица с интерфейсом
    /// </summary>
    [SerializeField] private GameObject container;
    /// <summary>
    /// Возрождения игрока.
    /// </summary>
    [SerializeField] public Button reviveButton;
    /// <summary>
    /// Включение режима наблюдателя.
    /// </summary>
    /// <param name="state">Состояние режима наблюдателя.</param>
    public void SetSpectatorMode(bool state)
    {
        container.SetActive(state);
    }
}
