using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// Класс панели командных жизней.
/// </summary>
public class HeartPanel : NetworkBehaviour
{
    /// <summary>
    /// Спрайт полной жизни.
    /// </summary>
    [SerializeField] private Sprite fullHeart;
    /// <summary>
    /// Спрайт пустой жизни.
    /// </summary>
    [SerializeField] private Sprite emptyHeart;
    /// <summary>
    /// Контейнер командных жизней.
    /// </summary>
    [SerializeField] private GameObject container;
    /// <summary>
    /// Объект режима наблюдателя.
    /// </summary>
    [SerializeField] private SpectatorMode spectatorMode;
    /// <summary>
    /// Объект командной жизни.
    /// </summary>
    [SerializeField] private GameObject heartObject;
    /// <summary>
    /// Количество жизней на каждого игрока.
    /// </summary>
    [SerializeField] private int heartPerPlayer;
    /// <summary>
    /// Список изображений жизней.
    /// </summary>
    private List<Image> heartImages = new List<Image>();
    /// <summary>
    /// Максимальное количество жизней.
    /// </summary>
    private int maxHearts;
    /// <summary>
    /// Текущее количество жизней.
    /// </summary>
    [SyncVar(hook = nameof(UpdateHearts))] public int curHearts;

    private MyNetworkManager room;
    private MyNetworkManager Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }

            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }

    /// <summary>
    /// Заполнение командных жизней.
    /// </summary>
    private void Start()
    {
        maxHearts = Room.GamePlayers.Count * heartPerPlayer;
        float size = heartObject.GetComponent<RectTransform>().rect.width + 8;
        container.GetComponent<RectTransform>().sizeDelta = new Vector2(size * maxHearts - 2, size);

        for (int i = 0; i < maxHearts; i++)
        {
            heartImages.Add(GameObject.Instantiate(heartObject,container.transform).GetComponent<Image>());
            heartImages[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(size * i, 0, 0);
        }
    }

    /// <summary>
    /// Установка кнопки возрождения.
    /// </summary>
    /// <param name="state">Состояние кнопки.</param>
    [ClientRpc]
    private void SetReviveButton(bool state)
    {
        spectatorMode.reviveButton.interactable = state;
    }

    /// <summary>
    /// Hook. Обновление командных жизней.
    /// </summary>
    /// <param name="oldValue">Старое значение.</param>
    /// <param name="newValue">Новое значение.</param>
    private void UpdateHearts(int oldValue, int newValue)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < curHearts)
            {
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }
        }
    }

    /// <summary>
    /// Добавление командной жизни.
    /// </summary>
    [Server]
    public void AddHeart()
    {
        curHearts += 1;
        SetReviveButton(true);
    }

    /// <summary>
    /// Удалить командную жизнь.
    /// </summary>
    [Server]
    public void RemoveHeart()
    {
        curHearts -= 1;

        if (curHearts < 0)
        {
            curHearts = 0;
        }

        if (curHearts == 0)
        {
            SetReviveButton(false);
        }
    }

    /// <summary>
    /// Удалить все командные жизни.
    /// </summary>
    [Server]
    public void RemoveAllHearts()
    {
        curHearts = 0;
        SetReviveButton(false);
    }

    /// <summary>
    /// Добавить все командные жизни.
    /// </summary>
    [Server]
    public void AddAllHearts()
    {
        curHearts = maxHearts;
        SetReviveButton(true);
    }

    /// <summary>
    /// Возвращает true, если текущее количество командных жизней является максимальным, иначе false.
    /// </summary>
    public bool IsMaxHearts()
    {
        if (curHearts == maxHearts)
        {
            return true;
        }
        return false;
    }

}
