using UnityEngine;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private MyNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    private static string path;

    public void HostLobby()
    {
        networkManager.StartHost();

        landingPagePanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void MenuDeleteAllSaves()
    {
        path = Application.persistentDataPath; // + Path.DirectorySeparatorChar;

        DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }
}
