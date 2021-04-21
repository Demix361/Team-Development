using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
/// Класс экрана проигрыша
public class LoseScreen : NetworkBehaviour
{
    /// Объект Overlay - экран проигрыша
    [SerializeField] private GameObject LoseScreenOverlay;
    /// Объект Button - кнопка Hub
    [SerializeField] private Button HubButton;
    /// Объект Button - кнопка повторного запуска уровня
    [SerializeField] private Button RestartButton;

    [ClientRpc]
    /// Метод включения экрана проигрыша
    public void RpcEnableLoseScreen()
    {
        LoseScreenOverlay.SetActive(true);
    }
    /// Метод возвращения в Hub
    public void ReturnToHub()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in a)
        {
            player.GetComponent<PlayerNetworkTalker>().CmdChangeScene("HubScene");
        }
    }
    /// Метод повторного запуска уровня
    public void RestartLevel()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in a)
        {
            player.GetComponent<PlayerNetworkTalker>().CmdChangeScene(SceneManager.GetActiveScene().name);
        }
    }
}
