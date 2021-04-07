using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsCanvases : MonoBehaviour
{
    [SerializeField] private MainMenuCanvas _mainMenuCanvas;
    public MainMenuCanvas MainMenuCanvas { get { return _mainMenuCanvas; } }

    [SerializeField] private SettingsCanvas _settingsCanvas;
    public SettingsCanvas SettingsCanvas { get { return _settingsCanvas; } }

    private void Awake()
    {
        FirstInitialize();
    }

    private void FirstInitialize()
    {
        MainMenuCanvas.FirstInitialize(this);
        SettingsCanvas.FirstInitialize(this);
    }

    public void HideAll()
    {
        MainMenuCanvas.Hide();
        SettingsCanvas.Hide();
    }
}
