using Mirror;

/// <summary>
/// Класс общения с сервером.
/// </summary>
public class PlayerNetworkTalker : NetworkBehaviour
{
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
    /// Сменить текущую сцену.
    /// </summary>
    /// <param name="sceneName">Название новой сцены.</param>
    [Command]
    public void CmdChangeScene(string sceneName)
    {
        Room.ServerChangeScene(sceneName);
    }

    /// <summary>
    /// Увеличить количество подбираемых предметов.
    /// </summary>
    /// <param name="collName">Название подбираемого предмета.</param>
    [Command]
    public void CmdIncreaseCollectable(string collName)
    {
        Room.ServerIncreaseCollectable(collName);
    }

    /// <summary>
    /// Установить ID уровня.
    /// </summary>
    /// <param name="levelID">ID уровня.</param>
    [Command]
    public void CmdSetLevelID(int levelID)
    {
        Room.ServerSetLevelID(levelID);
    }

    /// <summary>
    /// Сохранить уровень.
    /// </summary>
    [Command]
    public void CmdSaveLevel()
    {
        Room.ServerSaveLevel();
    }

    /// <summary>
    /// Сохранить найденные камни.
    /// </summary>
    [Command]
    public void CmdSaveGems()
    {
        Room.ServerSaveGems();
    }

    /// <summary>
    /// Получить имя игрока.
    /// </summary>
    /// <returns>Имя игрока.</returns>
    public string getPlayerName()
    {
        foreach(NetworkGamePlayer player in Room.GamePlayers)
        {
            if (player.hasAuthority)
            {
                return player.displayName;
            }
        }
        return null;
    }

    /// <summary>
    /// Имеет ли игрок права.
    /// </summary>
    public bool IsLocal()
    {
        return hasAuthority;
    }
}
