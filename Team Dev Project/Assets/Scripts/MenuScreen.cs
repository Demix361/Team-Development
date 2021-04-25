using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

/// <summary> Класс внутриигрового меню. </summary>
public class MenuScreen : MonoBehaviour
{
    [SerializeField] GameObject _menuScreenOverlay;

    private PlayerProperties _playerProperties;
    
    private void Start()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].GetComponent<NetworkIdentity>().hasAuthority)
            {
                _playerProperties = a[i].GetComponent<PlayerProperties>();
            }
        }
    }

    private void Update()
    {
        if (Input.GetButton("Cancel") && _playerProperties.allowInput)
        {
            _menuScreenOverlay.SetActive(true);
            _playerProperties.allowInput = false;
        }
    }

    /// <summary> Закрыть окно меню. </summary>
    public void OnClick_CloseMenu()
    {
        _menuScreenOverlay.SetActive(false);
        _playerProperties.allowInput = true;
    }

    /// <summary> Повторный запуск уровня. </summary>
    public void OnClick_RestartLevel()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in a)
        {
            player.GetComponent<PlayerNetworkTalker>().CmdChangeScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary> Переход на сцену хаба. </summary>
    public void OnClick_ReturnToHub()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in a)
        {
            player.GetComponent<PlayerNetworkTalker>().CmdChangeScene("HubScene");
        }
    }

    /// <summary> Выход из игры. </summary>
    public void OnCLick_ExitGame()
    {
        Application.Quit();
    }
}
