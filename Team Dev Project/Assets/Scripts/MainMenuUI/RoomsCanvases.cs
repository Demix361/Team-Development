﻿using UnityEngine;

/// <summary>
/// Класс холстов меню.
/// </summary>
public class RoomsCanvases : MonoBehaviour
{
    [SerializeField] private MainMenuCanvas _mainMenuCanvas;
    /// <summary>
    /// Холст главного меню.
    /// </summary>
    public MainMenuCanvas MainMenuCanvas { get { return _mainMenuCanvas; } }

    [SerializeField] private SettingsCanvas _settingsCanvas;
    /// <summary>
    /// Холст настроек.
    /// </summary>
    public SettingsCanvas SettingsCanvas { get { return _settingsCanvas; } }

    private void Awake()
    {
        FirstInitialize();
    }

    /// <summary>
    /// Инициализация полей класса.
    /// </summary>
    private void FirstInitialize()
    {
        MainMenuCanvas.FirstInitialize(this);
        SettingsCanvas.FirstInitialize(this);
    }

    /// <summary>
    /// Скрытие всех холстов меню.
    /// </summary>
    public void HideAll()
    {
        MainMenuCanvas.Hide();
        SettingsCanvas.Hide();
    }
}
