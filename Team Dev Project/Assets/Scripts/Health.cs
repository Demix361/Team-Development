using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;


/// <summary>
/// Класс здоровья игрока.
/// </summary>
public class Health : NetworkBehaviour
{
    /// <summary>
    /// Максимальное значение здоровья.
    /// </summary>
    [Header("Health")]
    [SerializeField] private float maxHealth = 100;

    /// <summary>
    /// Контейнер пользовательского интерфейса полоски здоровья.
    /// </summary>
    [Header("UI Border")]
    [SerializeField] private GameObject UIContainer;
    /// <summary>
    /// Изображение полоски здоровья.
    /// </summary>
    [SerializeField] private Image healthBarImage;
    /// <summary>
    /// Спрайт оболочки полоски здоровья.
    /// </summary>
    [SerializeField] private Sprite healthBarSprite;
    /// <summary>
    /// Спрайт оболочки полоски здоровья мертвого игрока.
    /// </summary>
    [SerializeField] private Sprite healthBarDeadSprite;
    /// <summary>
    /// Спрайт оболочки перевернутой вертикально полоски здоровья.
    /// </summary>
    [SerializeField] private Sprite healthBarSpriteUp;
    /// <summary>
    /// Спрайт оболочки перевернутой вертикально полоски здоровья мертвого игрока.
    /// </summary>
    [SerializeField] private Sprite healthBarDeadSpriteUp;
    /// <summary>
    /// RectTransform контейнера полоски здоровья.
    /// </summary>
    [SerializeField] private RectTransform borderRectTransform;

    /// <summary>
    /// Изображение полоски здоровья.
    /// </summary>
    [Header("UI Fill")]
    [SerializeField] private Image healthBarFillImage;
    /// <summary>
    /// Изображение догоняющей полоски здоровья.
    /// </summary>
    [SerializeField] private Image _healthBarFillFollowImage;
    /// <summary>
    /// RectTransform полоски здоровья.
    /// </summary>
    [SerializeField] private RectTransform fillRectTransform;
    /// <summary>
    /// RectTransform заднего фона полоски здоровья.
    /// </summary>
    [SerializeField] private RectTransform fillBackgroundRectTransform;
    /// <summary>
    /// RectTransform догоняющей полоски здоровья.
    /// </summary>
    [SerializeField] private RectTransform fillFollowRectTransform;
    /// <summary>
    /// Задержка следования догоняющей полоски здоровья.
    /// </summary>
    [SerializeField] private float _fillFollowDelay;
    /// <summary>
    /// Скорость следования догоняющей послоски здоровья.
    /// </summary>
    [SerializeField] private float _fillFollowSpeed;

    /// <summary>
    /// Жив ли игрок.
    /// </summary>
    /// <remarks>
    /// Синхронизируемая переменная.
    /// </remarks>
    [SyncVar] private bool alive = true;
    /// <summary>
    /// Состояние полоски здоровья игрока.
    /// </summary>
    /// <remarks>
    /// Синхронизируемая переменная.
    /// </remarks>
    [SyncVar(hook = nameof(SetHealthBarImage))] private int healthBarImageState;
    /// <summary>
    /// Текущее значение здоровья игрока.
    /// </summary>
    /// <remarks>
    /// Синхронизируемая переменная.
    /// </remarks>
    [SyncVar(hook =nameof(UpdateHealthBar))] private float currentHealth;
    /// <summary>
    /// Панель командных жизней.
    /// </summary>
    private HeartPanel heartPanel;
    /// <summary>
    /// Панель наблюдения за игроками.
    /// </summary>
    private SpectatorMode spectatorPanel;
    /// <summary>
    /// Экран проигрыша.
    /// </summary>
    private LoseScreen loseScreen;
    /// <summary>
    /// Таймер следования полоски здоровья.
    /// </summary>
    private float _fillFollowTimer = 0;

    /// <summary>
    /// Установка максимального здоровья при старте сервера.
    /// </summary>
    public override void OnStartServer()
    {
        SetHealth(maxHealth);
    }

    /// <summary>
    /// Установить максимальное здоровье всем игрокам.
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdHealAllMax()
    {
        foreach (var health in FindObjectsOfType<Health>())
        {
            health.SetHealth(maxHealth);
        }
    }

    /// <summary>
    /// Нанести урон игроку.
    /// </summary>
    /// <param name="damage">Значение урона.</param>
    [Command(requiresAuthority = false)]
    public void CmdDealDamage(float damage)
    {
        //GetComponent<Animator>().SetTrigger("Hit");
        RpcHitAnimation();
        SetHealth(Mathf.Max(currentHealth - damage, 0));
    }

    /// <summary>
    /// Проиграть анимацию получения урона.
    /// </summary>
    [ClientRpc]
    private void RpcHitAnimation()
    {
        GetComponent<Animator>().SetTrigger("Hit");
    }

    /// <summary>
    /// Нанести максимальный урон игроку.
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdDealMaxDamage()
    {
        SetHealth(0);
    }

    /// <summary>
    /// Установить здоровье игрока.
    /// </summary>
    /// <param name="value">Значение здоровья.</param>
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

    /// <summary>
    /// Изменить изображение полоски здоровья игрока.
    /// </summary>
    /// <param name="spriteID">ID полоски здоровья игрока.</param>
    [Command]
    private void CmdChangeHealthbarSprite(int spriteID)
    {
        healthBarImageState = spriteID;
    }

    /// <summary>
    /// Вызывает <see cref="ServerStopCam"/>
    /// </summary>
    [Command]
    public void CmdStopCam()
    {
        ServerStopCam();
    }

    /// <summary>
    /// Вызывает <see cref="RpcStopCam"/>
    /// </summary>
    [Server]
    private void ServerStopCam()
    {
        RpcStopCam();
    }

    /// <summary>
    /// Остановить камеру у игроков, наблюдавших за игроком, который умер.
    /// </summary>
    [ClientRpc]
    private void RpcStopCam()
    {
        gameObject.GetComponent<PlayerCameraFollow>().StopFollowOnDeath();
    }

    /// <summary>
    /// Вызывает <see cref="ServerFollowCam"/>.
    /// </summary>
    [Command]
    public void CmdFollowCam()
    {
        ServerFollowCam();
    }

    /// <summary>
    /// Вызывает <see cref="RpcFollowCam"/>.
    /// </summary>
    [Server]
    private void ServerFollowCam()
    {
        RpcFollowCam();
    }

    /// <summary>
    /// Следование камеры за игроком.
    /// </summary>
    [ClientRpc]
    private void RpcFollowCam()
    {
        if (hasAuthority)
        {
            gameObject.GetComponent<PlayerCameraFollow>().FollowPlayer();
        }
    }

    /// <summary>
    /// Отвечает за начальное расположение пользовательского интерфейса полосок здоровья игроков.
    /// </summary>
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

    /// <summary>
    /// Жив ли игрок.
    /// </summary>
    /// <returns>Состояние игрока.</returns>
    public bool IsAlive()
    {
        return alive;
    }

    /// <summary>
    /// Метод смерти игрока.
    /// </summary>
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

    /// <summary>
    /// Вызывает <see cref="ServerRevive"/>.
    /// </summary>
    [Command]
    private void CmdRevive()
    {
        ServerRevive();
    }

    /// <summary>
    /// Отвечает за обновление командных жизней, смену изображения полоски здоровья и вызывает <see cref="RpcRevive"/>.
    /// </summary>
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

    /// <summary>
    /// Отвечает за возрождение игрока.
    /// </summary>
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
                if (cp.GetComponent<CheckPoint>()._unlocked)
                {
                    if (checkpoint == null)
                    {
                        checkpoint = cp;
                    }
                    else if (cp.GetComponent<CheckPoint>()._checkpointID > checkpoint.GetComponent<CheckPoint>()._checkpointID)
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
                spawnPosition = checkpoint.GetComponent<CheckPoint>()._spawnPoint.position;
            }
            spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);

            gameObject.GetComponent<PlayerProperties>().allowInput = true;
            gameObject.transform.localPosition = spawnPosition;

            GameObject.Find("SpectatorPanel").GetComponent<SpectatorMode>().SetSpectatorMode(false);
            CmdFollowCam();
        }
    }

    /// <summary>
    /// Hook. Обновляет изображение полоски здоровья игрока.
    /// </summary>
    /// <param name="oldHealth"></param>
    /// <param name="newHealth"></param>
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

    /// <summary>
    /// Обновление таймеров.
    /// </summary>
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

    /// <summary>
    /// Hook. Обновление изображения полоски здоровья.
    /// </summary>
    /// <param name="oldValue">Старое значение.</param>
    /// <param name="newValue">Новое значение.</param>
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
