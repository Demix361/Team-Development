using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Класс свойства игрока.
/// </summary>
/// <remarks>
/// Назначает имя игроков и изменяет их значения. Назначает расположения интерфейса игроков.
/// </remarks>
public class PlayerProperties : NetworkBehaviour
{
    /// <summary>
    /// Возможность ввода
    /// </summary>
    [Header("Settings")]
    public bool allowInput = true;
    /// <summary>
    /// Состояние общение с сервером
    /// </summary>
    [SerializeField] public PlayerNetworkTalker playerNetworkTalker;
    /// <summary>
    /// Интерфейс игрока.
    /// </summary>
    [Header("Player UI")]
    [SerializeField] GameObject playerUI;
    /// <summary>
    /// Изменение интерфейса игрока
    /// </summary>
    [SerializeField] RectTransform playerUIRectTransform;
    /// <summary>
    /// Текст имени игрока
    /// </summary>
    [SerializeField] TMP_Text playerNameText;
    /// <summary>
    /// Изменение имени игрока
    /// </summary>
    [SerializeField] RectTransform playerNameRectTransform;
    /// <summary>
    /// ID игрока
    /// </summary>
    [SyncVar] public int playerId;
    /// <summary>
    /// Изменение имени игрока
    /// </summary>
    [SyncVar(hook = nameof(UpdatePlayerName))] public string playerName;
    /// <summary>
    /// Назначение расположение интерфейса игроков на экране
    /// </summary>
    private void Start()
    {
        CmdSetPlayerName(GetPlayerName());

        if (playerId == 0)
        {
            playerUIRectTransform.anchorMin = new Vector2(0, 0);
            playerUIRectTransform.anchorMax = new Vector2(0, 0);
            playerUIRectTransform.pivot = new Vector2(0, 0);
            playerUIRectTransform.anchoredPosition = new Vector3(20, 20, 0);
        }
        else if (playerId == 1)
        {
            playerUIRectTransform.anchorMin = new Vector2(1, 0);
            playerUIRectTransform.anchorMax = new Vector2(1, 0);
            playerUIRectTransform.pivot = new Vector2(1, 0);
            playerUIRectTransform.anchoredPosition = new Vector3(-20, 20, 0);
            playerNameRectTransform.anchoredPosition = new Vector2(-playerNameRectTransform.anchoredPosition.x, playerNameRectTransform.anchoredPosition.y);
        }
        else if (playerId == 2)
        {
            playerUIRectTransform.anchorMin = new Vector2(0, 1);
            playerUIRectTransform.anchorMax = new Vector2(0, 1);
            playerUIRectTransform.pivot = new Vector2(0, 1);
            playerUIRectTransform.anchoredPosition = new Vector3(20, -20, 0);
            playerNameRectTransform.anchoredPosition = new Vector2(playerNameRectTransform.anchoredPosition.x, -playerNameRectTransform.anchoredPosition.y);
        }
        else if (playerId == 3)
        {
            playerUIRectTransform.anchorMin = new Vector2(1, 1);
            playerUIRectTransform.anchorMax = new Vector2(1, 1);
            playerUIRectTransform.pivot = new Vector2(1, 1);
            playerUIRectTransform.anchoredPosition = new Vector3(-20, -20, 0);
            playerNameRectTransform.anchoredPosition = new Vector2(-playerNameRectTransform.anchoredPosition.x, -playerNameRectTransform.anchoredPosition.y);
        }

        string curScene = SceneManager.GetActiveScene().name;
        if (curScene.StartsWith("LevelScene"))
        {
            playerUI.SetActive(true);
        }
        else if (curScene.StartsWith("HubScene"))
        {
            playerUI.SetActive(false);
        }
    }

    /// <summary>
    /// Получение имени игрока
    /// </summary>
    /// <returns>Возвращает имя игрока или null</returns>
    private string GetPlayerName()
    {
        foreach (NetworkGamePlayer player in GameObject.FindObjectsOfType<NetworkGamePlayer>())
        {
            if (player.hasAuthority)
            {
                return player.displayName;
            }
        }
        return null;
    }

    /// <summary>
    /// Назначения имени игрока
    /// </summary>
    /// <param name="pName">Имя игрока</param>
    [Command]
    private void CmdSetPlayerName(string pName)
    {
        playerName = pName;
    }

    /// <summary>
    /// Изменение имени игрока (хук)
    /// </summary>
    /// <param name="oldValue">Старое значение</param>
    /// <param name="newValue">Новое значение</param>
    private void UpdatePlayerName(string oldValue, string newValue)
    {
        playerNameText.SetText(playerName);
    }

    public bool IsServer()
    {
        return isServer;
    }
}
