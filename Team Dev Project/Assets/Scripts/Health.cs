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

    [SyncVar][SerializeField]
    private bool alive;

    private SpectatorMode spectatorPanel;

    #region Server
    [Server]
    private void SetHealth(int value)
    {
        if (value == 0)
        {
            if (heartPanel.curHearts == 0)
            {
                alive = false;
                Die();
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

    // STOP CAM
    [Command]
    public void CmdStopCam()
    {
        //if (!IsAlive() && !gameObject.GetComponent<PlayerCameraFollow>().IsFollowedPlayerAlive())
        ServerStopCam();
    }

    [Server]
    private void ServerStopCam()
    {
        RpcStopCam();
    }

    [ClientRpc]
    private void RpcStopCam()
    {
        //if (!IsAlive() && !gameObject.GetComponent<PlayerCameraFollow>().IsFollowedPlayerAlive())
        //if (hasAuthority)
        
            gameObject.GetComponent<PlayerCameraFollow>().StopFollowOnDeath();
        
    }

    // FOLLOW CAM
    [Command]
    public void CmdFollowCam()
    {
        ServerFollowCam();
    }

    [Server]
    private void ServerFollowCam()
    {
        RpcFollowCam();
    }

    [ClientRpc]
    private void RpcFollowCam()
    {
        if (hasAuthority)
        {
            gameObject.GetComponent<PlayerCameraFollow>().FollowPlayer();
        }
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
            //heartPanel.AddAllHearts();

            heartPanel.RemoveAllHearts();
            heartPanel.AddHeart();

            ToggleHealthBar(true);

            spectatorPanel = GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>();

            spectatorPanel.reviveButton.onClick.AddListener(CmdRevive);
        }
        else if (curSceneName.StartsWith("HubScene"))
        {
            ToggleHealthBar(false);
        }
    }

    public bool IsAlive()
    {
        return alive;
    }

    [ClientRpc]
    private void Die()
    {
        if (hasAuthority)
        {
            gameObject.GetComponent<PlayerProperties>().allowInput = false;
            gameObject.transform.position = PlayerSpawnSystem.deathPoint.position;
            GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>().SetSpectatorMode(true);

            CmdStopCam();

            gameObject.GetComponent<PlayerCameraFollow>().StopFollow();

            Debug.Log("пользователь умер");
        }
    }

    [Command]
    private void CmdRevive()
    {
        ServerRevive();
    }

    [Server]
    private void ServerRevive()
    {
        alive = true;
        heartPanel.RemoveHeart();
        currentHealth = maxHealth;
        RpcRevive();
    }

    [ClientRpc]
    private void RpcRevive()
    {
        if (hasAuthority)
        {
            gameObject.GetComponent<PlayerProperties>().allowInput = true;
            gameObject.transform.position = PlayerSpawnSystem.spawnPoints[0].position;
            GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>().SetSpectatorMode(false);

            CmdFollowCam();

            //gameObject.GetComponent<PlayerCameraFollow>().StopFollow();

            Debug.Log("пользователь воскрес");
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
