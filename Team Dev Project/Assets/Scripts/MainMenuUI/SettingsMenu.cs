using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// Класс меню настроек.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    /// <summary>
    /// AudioMixer.
    /// </summary>
    [SerializeField] private AudioMixer _audioMixer;
    /// <summary>
    /// Выпадающее меню настроек разрешений.
    /// </summary>
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    /// <summary>
    /// Переключатель полного экрана.
    /// </summary>
    [SerializeField] private Toggle _fullscreenToggle;

    /// <summary>
    /// Список разрешений дисплея.
    /// </summary>
    private Resolution[] _resolutions;
    /// <summary>
    /// Объект холстов меню.
    /// </summary>
    private RoomsCanvases _roomsCanvases;

    /// <summary>
    /// Инициализация полей класса.
    /// </summary>
    /// <param name="canvases">Объект RoomsCanvases</param>
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    private void Start()
    {
        _resolutions = Screen.resolutions;
        _resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height + " @" + _resolutions[i].refreshRate;
            options.Add(option);

            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();

        _fullscreenToggle.isOn = Screen.fullScreen;
    }

    /// <summary>
    /// Настройка громкости.
    /// </summary>
    /// <param name="volume">Значение громкости.</param>
    public void OnClick_SetVolume(float volume)
    {
        _audioMixer.SetFloat("volume", volume);
    }

    /// <summary>
    /// Настройка полного экрана.
    /// </summary>
    /// <param name="isFullScreen">Состояние полного экрана.</param>
    public void OnClick_SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    /// <summary>
    /// Настройка разрешения приложения.
    /// </summary>
    /// <param name="resolutionIndex">Индекс разрешения.</param>
    public void OnClick_SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// Закрытие меню настроек.
    /// </summary>
    public void OnClick_Return()
    {
        _roomsCanvases.SettingsCanvas.Hide();
        _roomsCanvases.MainMenuCanvas.Show();
    }
}
