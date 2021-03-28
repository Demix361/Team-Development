using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private float popupDelay;
    [SerializeField] private GameObject popupSprite;
    private float counter = 0;
    private bool needCount;

    private void Start()
    {
        needCount = false;
    }

    public void SetPopup(bool state)
    {
        if (state)
        {
            needCount = true;
        }
        else
        {
            popupSprite.SetActive(false);
            needCount = false;
            counter = 0;
        }
        Debug.Log($"POPUP {state}");
    }

    private void Update()
    {
        if (needCount)
        {
            counter += Time.deltaTime;
            Debug.Log(counter);
            if (counter > popupDelay)
            {
                popupSprite.SetActive(true);
                needCount = false;
                counter = 0;
            }
        }
    }
}
