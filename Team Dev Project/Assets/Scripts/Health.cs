using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class Health : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Image healthBarImage = null;
    [SerializeField] private GameObject HB = null;
    [SerializeField] private RectTransform m_RectTransform;

    [SyncVar(hook =nameof(UpdateHealthBar))]
    private int currentHealth;

    private HeartPanel heartPanel;

    //[SyncVar]
    public bool alive = true;
    
    [ClientRpc]
    private void SetDeath()
    {
        //if (hasAuthority)
        //{
            gameObject.GetComponent<PlayerProperties>().allowInput = false;
            alive = false;
            gameObject.transform.position = PlayerSpawnSystem.deathPoint.position;
            GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>().SetSpectatorMode(true);

            gameObject.GetComponent<PlayerCameraFollow>().StopFollow();
            //gameObject.GetComponent<PlayerCameraFollow>().NextPlayerCamera();
            Debug.Log("пользователь умер");
        //}
    }


    #region Server
    [Server]
    private void SetHealth(int value)
    {
        if (value == 0)
        {
            if (heartPanel.curHearts == 0)
            {
                SetDeath();
            }   
            else
            {
                heartPanel.RemoveHeart();
                value = maxHealth;
            }
        }
        currentHealth = value;
    }

    public override void OnStartServer()
    {
        SetHealth(maxHealth);
    }

    [Command]
    public void CmdDealDamage(int damage)
    {
        SetHealth(Mathf.Max(currentHealth - damage, 0));
    }
    
    #endregion

    #region Client
    private void Start()
    {
        int id = gameObject.GetComponent<PlayerProperties>().playerId;

        if (id == 0)
        {
            m_RectTransform.anchorMin = new Vector2(0, 0);
            m_RectTransform.anchorMax = new Vector2(0, 0);
            m_RectTransform.pivot = new Vector2(0, 0);
            m_RectTransform.anchoredPosition = new Vector3(20, 20, 0);
        }
        else if (id == 1)
        {
            m_RectTransform.anchorMin = new Vector2(1, 0);
            m_RectTransform.anchorMax = new Vector2(1, 0);
            m_RectTransform.pivot = new Vector2(1, 0);
            m_RectTransform.anchoredPosition = new Vector3(-20, 20, 0);
        }
        else if (id == 2)
        {
            m_RectTransform.anchorMin = new Vector2(0, 1);
            m_RectTransform.anchorMax = new Vector2(0, 1);
            m_RectTransform.pivot = new Vector2(0, 1);
            m_RectTransform.anchoredPosition = new Vector3(20, -20, 0);
        }
        else
        {
            m_RectTransform.anchorMin = new Vector2(1, 1);
            m_RectTransform.anchorMax = new Vector2(1, 1);
            m_RectTransform.pivot = new Vector2(1, 1);
            m_RectTransform.anchoredPosition = new Vector3(-20, -20, 0);
        }

        string curSceneName = SceneManager.GetActiveScene().name;

        if (curSceneName.StartsWith("LevelScene"))
        {
            heartPanel = GameObject.Find("HeartPanel").GetComponent<HeartPanel>();
            heartPanel.AddAllHearts();
            ToggleHealthBar(true);
        }
        else if (curSceneName.StartsWith("HubScene"))
        {
            ToggleHealthBar(false);
        }
    }

    // hook
    private void UpdateHealthBar(int oldHealth, int newHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
    public void ToggleHealthBar(bool state)
    {
        HB.SetActive(state);
    }

    #endregion
}
