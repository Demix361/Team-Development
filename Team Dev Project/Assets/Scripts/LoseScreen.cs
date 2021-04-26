using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// Класс экрана проигрыша.
/// </summary>
public class LoseScreen : NetworkBehaviour
{
    /// <summary>
    /// Оверлей экрана проигрыша.
    /// </summary>
    [SerializeField] private GameObject LoseScreenOverlay;
    /// <summary>
    /// Кнопка перехода на сцену хаба.
    /// </summary>
    [SerializeField] private Button HubButton;
    /// <summary>
    /// Кнопка повторного запуска уровня.
    /// </summary>
    [SerializeField] private Button RestartButton;

    /// <summary>
    /// Включение экрана проигрыша.
    /// </summary>
    [ClientRpc]
    public void RpcEnableLoseScreen()
    {
        LoseScreenOverlay.SetActive(true);
    }

    /// <summary>
    /// Переход на сцену хаба.
    /// </summary>
    public void ReturnToHub()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in a)
        {
            player.GetComponent<PlayerNetworkTalker>().CmdChangeScene("HubScene");
        }
    }

    /// <summary>
    /// Повторный запуск уровня.
    /// </summary>
    public void RestartLevel()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in a)
        {
            player.GetComponent<PlayerNetworkTalker>().CmdChangeScene(SceneManager.GetActiveScene().name);
        }
    }
}
