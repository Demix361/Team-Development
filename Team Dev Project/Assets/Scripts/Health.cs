using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class Health : NetworkBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100;
    //[SyncVar] [SerializeField] private bool alive;

    [Header("UI Border")]
    [SerializeField] private GameObject UIContainer;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite healthBarSprite;
    [SerializeField] private Sprite healthBarDeadSprite;
    [SerializeField] private Sprite healthBarSpriteUp;
    [SerializeField] private Sprite healthBarDeadSpriteUp;
    [SerializeField] private RectTransform borderRectTransform;

    [Header("UI Fill")]
    [SerializeField] private Image healthBarFillImage;
    [SerializeField] private Image _healthBarFillFollowImage;
    [SerializeField] private RectTransform fillRectTransform;
    [SerializeField] private RectTransform fillBackgroundRectTransform;
    [SerializeField] private RectTransform fillFollowRectTransform;
    [SerializeField] private float _fillFollowDelay;
    [SerializeField] private float _fillFollowSpeed;

    [SyncVar] private bool alive = true;
    [SyncVar(hook = nameof(SetHealthBarImage))] private int healthBarImageState;
    [SyncVar(hook =nameof(UpdateHealthBar))] private float currentHealth;
    private HeartPanel heartPanel;
    private SpectatorMode spectatorPanel;
    private LoseScreen loseScreen;
    private float _fillFollowTimer = 0;

    // Server start
    public override void OnStartServer()
    {
        SetHealth(maxHealth);
    }

    // Восстановить всем игрокам максимальное здоровье
    [Command(requiresAuthority = false)]
    public void CmdHealAllMax()
    {
        foreach (var health in FindObjectsOfType<Health>())
        {
            health.SetHealth(maxHealth);
        }
    }

    // Получение урона
    [Command(requiresAuthority = false)]
    public void CmdDealDamage(float damage)
    {
        //GetComponent<Animator>().SetTrigger("Hit");
        RpcHitAnimation();
        SetHealth(Mathf.Max(currentHealth - damage, 0));
    }

    [ClientRpc]
    private void RpcHitAnimation()
    {
        GetComponent<Animator>().SetTrigger("Hit");
    }

    // Получение максимального урона
    [Command(requiresAuthority = false)]
    public void CmdDealMaxDamage()
    {
        SetHealth(0);
    }

    [Server]
    private void SetHealth(float value)
    {
        if (value == 0)
        {
            alive = false;

            if (healthBarImageState == 0)
                healthBarImageState = 1;
            else if (healthBarImageState == 2)
                healthBarImageState = 3;

            Die();

            // Включение экрана проигрыша, если все игроки мертвы
            if (heartPanel.curHearts == 0)
            {
                var a = GameObject.FindGameObjectsWithTag("Player");
                int count = 0;
                foreach (GameObject player in a)
                    if (!player.GetComponent<Health>().IsAlive())
                        count += 1;

                if (count == a.Length)
                {
                    loseScreen.RpcEnableLoseScreen();
                    UIContainer.SetActive(false);
                }
            }
        }
        currentHealth = value;
    }

    [Command]
    private void CmdChangeHealthbarSprite(int spriteID)
    {
        healthBarImageState = spriteID;
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

        if (id == 1)
        {
            Vector3 temp = borderRectTransform.localScale;
            temp.x = -temp.x;
            borderRectTransform.localScale = temp;
            temp = fillRectTransform.localScale;
            temp.x = -temp.x;

            fillRectTransform.localScale = temp;
            fillBackgroundRectTransform.localScale = temp;
            fillFollowRectTransform.localScale = temp;

            fillRectTransform.anchoredPosition = new Vector2(-fillRectTransform.anchoredPosition.x, fillRectTransform.anchoredPosition.y);
            fillBackgroundRectTransform.anchoredPosition = new Vector2(-fillBackgroundRectTransform.anchoredPosition.x, fillBackgroundRectTransform.anchoredPosition.y);
            fillFollowRectTransform.anchoredPosition = new Vector2(-fillFollowRectTransform.anchoredPosition.x, fillFollowRectTransform.anchoredPosition.y);
        }
        else if (id == 2)
        {
            CmdChangeHealthbarSprite(2);

            fillRectTransform.anchoredPosition = new Vector2(fillRectTransform.anchoredPosition.x, -fillRectTransform.anchoredPosition.y);
            fillBackgroundRectTransform.anchoredPosition = new Vector2(fillBackgroundRectTransform.anchoredPosition.x, -fillBackgroundRectTransform.anchoredPosition.y);
            fillFollowRectTransform.anchoredPosition = new Vector2(fillFollowRectTransform.anchoredPosition.x, -fillFollowRectTransform.anchoredPosition.y);
        }
        else if (id == 3)
        {
            CmdChangeHealthbarSprite(2);
            Vector3 temp = borderRectTransform.localScale;
            temp.x = -temp.x;
            borderRectTransform.localScale = temp;
            temp = fillRectTransform.localScale;
            temp.x = -temp.x;

            fillRectTransform.localScale = temp;
            fillBackgroundRectTransform.localScale = temp;
            fillFollowRectTransform.localScale = temp;

            fillRectTransform.anchoredPosition = new Vector2(-fillRectTransform.anchoredPosition.x, -fillRectTransform.anchoredPosition.y);
            fillBackgroundRectTransform.anchoredPosition = new Vector2(-fillBackgroundRectTransform.anchoredPosition.x, -fillBackgroundRectTransform.anchoredPosition.y);
            fillFollowRectTransform.anchoredPosition = new Vector2(-fillFollowRectTransform.anchoredPosition.x, -fillFollowRectTransform.anchoredPosition.y);
        }
        
        string curSceneName = SceneManager.GetActiveScene().name;

        if (curSceneName.StartsWith("LevelScene"))
        {
            heartPanel = GameObject.Find("HeartPanel").GetComponent<HeartPanel>();
            heartPanel.AddAllHearts();

            spectatorPanel = GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>();
            spectatorPanel.reviveButton.onClick.AddListener(CmdRevive);

            loseScreen = GameObject.Find("LoseScreen").GetComponent<LoseScreen>();
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

        if (healthBarImageState == 1)
            healthBarImageState = 0;
        else if (healthBarImageState == 3)
            healthBarImageState = 2;

        RpcRevive();
    }

    [ClientRpc]
    private void RpcRevive()
    {
        if (hasAuthority)
        {
            Vector3 spawnPosition;

            GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
            GameObject checkpoint = null;
            foreach (GameObject cp in checkpoints)
            {
                if (cp.GetComponent<CheckPoint>().unlocked)
                {
                    if (checkpoint == null)
                    {
                        checkpoint = cp;
                    }
                    else if (cp.GetComponent<CheckPoint>().checkpointID > checkpoint.GetComponent<CheckPoint>().checkpointID)
                    {
                        checkpoint = cp;
                    }
                }
            }

            if (checkpoint == null)
            {
                spawnPosition = PlayerSpawnSystem.spawnPoints[0].position;
            }
            else
            {
                spawnPosition = checkpoint.GetComponent<CheckPoint>().spawnPoint.position;
            }
            spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);

            gameObject.GetComponent<PlayerProperties>().allowInput = true;
            gameObject.transform.localPosition = spawnPosition;

            GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>().SetSpectatorMode(false);
            CmdFollowCam();
        }
    }

    // hook для обновления полоски здоровья
    private void UpdateHealthBar(float oldHealth, float newHealth)
    {
        healthBarFillImage.fillAmount = (float)currentHealth / maxHealth;

        if (newHealth > oldHealth)
        {
            _healthBarFillFollowImage.fillAmount = healthBarFillImage.fillAmount;
        }
        else
        {
            _fillFollowTimer = _fillFollowDelay;
        }
    }

    private void Update()
    {
        if (_fillFollowTimer > 0)
        {
            _fillFollowTimer -= Time.deltaTime;
        }
        else
        {
            if (_healthBarFillFollowImage.fillAmount > healthBarFillImage.fillAmount)
            {
                _healthBarFillFollowImage.fillAmount -= _fillFollowSpeed;
            }
        }
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
        else if (healthBarImageState == 2)
        {
            healthBarImage.sprite = healthBarSpriteUp;
        }
        else if (healthBarImageState == 3)
        {
            healthBarImage.sprite = healthBarDeadSpriteUp;
        }
    }
}
