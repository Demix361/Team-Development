using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class Health : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Image healthBarFillImage = null;
    [SerializeField] private GameObject HB = null;
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite healthBarSprite;
    [SerializeField] private Sprite healthBarDeadSprite;
    [SyncVar(hook = nameof(SetHealthBarImage))] private int healthBarImageState;
    [SyncVar(hook =nameof(UpdateHealthBar))] private int currentHealth;
    [SyncVar] [SerializeField] private bool alive;
    private HeartPanel heartPanel;
    private SpectatorMode spectatorPanel;
    private LoseScreen loseScreen;

    // Server start
    public override void OnStartServer()
    {
        SetHealth(maxHealth);
    }

    // Получение урона
    [Command]
    public void CmdDealDamage(int damage)
    {
        SetHealth(Mathf.Max(currentHealth - damage, 0));
    }

    [Server]
    private void SetHealth(int value)
    {
        if (value == 0)
        {
            if (heartPanel.curHearts == 0)
            {
                alive = false;

                healthBarImageState = 1;

                Die();

                // Включение экрана проигрыша, если все игроки мертвы
                var a = GameObject.FindGameObjectsWithTag("Player");
                int count = 0;
                foreach (GameObject player in a)
                    if (!player.GetComponent<Health>().IsAlive())
                        count += 1;

                if (count == a.Length)
                    loseScreen.RpcEnableLoseScreen();
            }
            else
            {
                heartPanel.RemoveHeart();
                value = maxHealth;
            }
        }
        currentHealth = value;
    }

    // Остановить камеру у игроков, наблюдавших за игроком, который умер Cmd -> Server -> Rpc
    [Command]
    public void CmdStopCam()
    {
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
        gameObject.GetComponent<PlayerCameraFollow>().StopFollowOnDeath();
    }

    // Заставить камеру следовать за собой Cmd -> Server -> Rpc
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

    // Client start
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

        healthBarImageState = 0;

        if (curSceneName.StartsWith("LevelScene"))
        {
            heartPanel = GameObject.Find("HeartPanel").GetComponent<HeartPanel>();
            //heartPanel.AddAllHearts();

            heartPanel.RemoveAllHearts();
            heartPanel.AddHeart();

            ToggleHealthBar(true);

            spectatorPanel = GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>();
            spectatorPanel.reviveButton.onClick.AddListener(CmdRevive);

            loseScreen = GameObject.Find("LoseScreen").GetComponent<LoseScreen>();
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

    // Умереть 
    [ClientRpc]
    private void Die()
    {
        if (hasAuthority)
        {
            gameObject.GetComponent<PlayerProperties>().allowInput = false;
            gameObject.transform.position = PlayerSpawnSystem.deathPoint.position;
            GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>().SetSpectatorMode(true);

            // Останавливаем камеры игроков, которые наблюдали за нами
            CmdStopCam();

            // Останавливаем свою камеру
            gameObject.GetComponent<PlayerCameraFollow>().StopFollow();

            //loseScreen.CmdEnableLoseScreen();

            Debug.Log("пользователь умер");
        }
    }

    // Возродиться Cmd -> Server -> Rpc
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
        healthBarImageState = 0;

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

            Debug.Log("пользователь воскрес");
        }
    }

    // hook для обновления полоски здоровья
    private void UpdateHealthBar(int oldHealth, int newHealth)
    {
        healthBarFillImage.fillAmount = (float)currentHealth / maxHealth;
    }

    // hook для смены спрайта healthbar'а
    private void SetHealthBarImage(int oldValue, int newValue)
    {
        if (healthBarImageState == 0)
        {
            healthBarImage.sprite = healthBarSprite;
        }
        else if (healthBarImageState == 1)
        {
            healthBarImage.sprite = healthBarDeadSprite;
        }
    }

    // вкл/выкл healthbar
    public void ToggleHealthBar(bool state)
    {
        HB.SetActive(state);
    }
}
