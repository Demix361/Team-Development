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
        int pos_x = 380;//380;
        int pos_y = 278;//278;

        int id = gameObject.GetComponent<PlayerProperties>().playerId;
        id += 0;

        if (id == 0)
        {
            pos_x *= -1;
            pos_y *= -1;
        }
        else if (id == 1)
        {
            pos_y *= -1;
        }
        else if (id == 2)
        {
            pos_x *= -1;
        }

        Vector2 pos = new Vector2(580 + pos_x, 310 + pos_y);
        HB.transform.position = pos;

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
        Debug.Log(gamePlayer.playerId);
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void ToggleHealthBar(bool state)
    {
        HB.SetActive(state);
    }
    #endregion
}
