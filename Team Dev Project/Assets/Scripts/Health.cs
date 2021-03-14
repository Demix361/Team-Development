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

    #region Server
    [Server]
    private void SetHealth(int value)
    {
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
    
    [Command]
    public void CmdMaxHealth()
    {
        SetHealth(maxHealth);
    }

    [Command]
    public void CmdHealHealth(int heal)
    {
        SetHealth(Mathf.Min(currentHealth + heal, maxHealth));
    }
    
    #endregion

    #region Client
    void Update()
    {
        if (hasAuthority && Input.GetButtonDown("Interact"))
        {
            CmdDealDamage(10);
        }
    }

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
            ToggleHealthBar(true);
        }
        else if (curSceneName.StartsWith("HubScene"))
        {
            ToggleHealthBar(false);
        }
    }

    private void UpdateHealthBar(int oldHealth, int newHealth)
    {
        //NetworkGamePlayer gamePlayer = GetComponent<NetworkGamePlayer>();
        //var gamePlayer = GameObject.FindObjectOfType<NetworkGamePlayer>();
        var gamePlayer = gameObject.GetComponent<PlayerProperties>();
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void ToggleHealthBar(bool state)
    {
        HB.SetActive(state);
    }
    #endregion
}
