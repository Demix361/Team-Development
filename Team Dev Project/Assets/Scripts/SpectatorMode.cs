using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpectatorMode : MonoBehaviour
{
    [SerializeField] public Button previousButton;
    [SerializeField] public Button nextButton;
    [SerializeField] private GameObject container;

    public void SetSpectatorMode(bool state)
    {
        container.SetActive(state);
    }
}
