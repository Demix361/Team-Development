using Mirror;
using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Класс перемещения камеры
/// </summary>
/// <remarks>
/// Камера передвигается за игроком. Проверка статуса жизни игрока, смена камеры.
/// </remarks>
public class PlayerCameraFollow : NetworkBehaviour
{
    /// <summary>
    /// объект IcinemachineCamera
    /// </summary>
    private ICinemachineCamera iVcam;
    /// <summary>
    /// Запрет на следование камеры за другим игроком
    /// </summary>
    private bool followOther = false;
    /// <summary>
    /// Добавление в список игровых объектов других игроков
    /// </summary>
    private List<GameObject> otherPlayers = new List<GameObject>();
    /// <summary>
    /// Добавление в список здоровья показатели здоровья других игроков
    /// </summary>
    private List<Health> otherPlayersHealth = new List<Health>();
    /// <summary>
    /// Задавание индекса
    /// </summary>
    private int pIndex = 0;
    /// <summary>
    /// Объект спектатор мода
    /// </summary>
    private SpectatorMode spectatorPanel;

    /// <summary>
    /// Старт клиента
    /// </summary>
    void Start()
    {
        iVcam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

        var a = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in a)
        {
            otherPlayers.Add(player);
            otherPlayersHealth.Add(player.GetComponent<Health>());
        }

        if (hasAuthority)
        {
            if (iVcam.Follow == null)
            {
                FollowPlayer();
            }
        }

        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            spectatorPanel = GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>();
            spectatorPanel.previousButton.onClick.AddListener(PreviousPlayerCamera);
            spectatorPanel.nextButton.onClick.AddListener(NextPlayerCamera);
        }
    }

    /// <summary>
    /// Следование камеры за игроком.
    /// </summary>
    public void FollowPlayer()
    {
        iVcam.Follow = transform;
        followOther = false;
    }

    /// <summary>
    /// Перестать наблюдать за игроком
    /// </summary>
    public void StopFollowOnDeath()
    {
        if (followOther == true)
        {
            if (!IsFollowedPlayerAlive())
            {
                iVcam.Follow = null;
            }
        }

        followOther = false;
    }

    /// <summary>
    /// Жив ли игрок, за которым мы наблюдаем
    /// </summary>
    /// <returns>Значение здоровья</returns>
    public bool IsFollowedPlayerAlive()
    {
        return otherPlayers[pIndex].GetComponent<Health>().IsAlive();
    }

    /// <summary>
    /// Жив ли игрок, за которым мы наблюдаем.
    /// </summary>
    public void StopFollow()
    {
        iVcam.Follow = null;
        followOther = false;
    }

    /// <summary>
    /// Переключить камеру на следующего живого игрока, если таких нет, то на себя.
    /// </summary>
    public void NextPlayerCamera()
    {
        int n = otherPlayers.Count;

        for (int i = 0; i < n; i++)
        {
            pIndex += 1;
            if (pIndex > otherPlayers.Count - 1)
            {
                pIndex = 0;
            }

            if (otherPlayers[pIndex].GetComponent<Health>().IsAlive() == true)
            {
                break;
            }
        }

        iVcam.Follow = otherPlayers[pIndex].transform;
        followOther = true;
    }

    /// <summary>
    /// Переключить камеру на предыдущего живого игрока, если таких нет, то на себя.
    /// </summary>
    public void PreviousPlayerCamera()
    {
        int n = otherPlayers.Count;

        for (int i = 0; i < n; i++)
        {
            pIndex -= 1;
            if (pIndex < 0)
            {
                pIndex = n - 1;
            }

            if (otherPlayersHealth[pIndex].IsAlive())
            {
                break;
            }
        }

        iVcam.Follow = otherPlayers[pIndex].transform;
        followOther = true;
    }
}
