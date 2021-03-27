﻿using UnityEngine;
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
    [SerializeField] private Sprite healthBarSpriteUp;
    [SerializeField] private Sprite healthBarDeadSpriteUp;
    [SyncVar(hook = nameof(SetHealthBarImage))] private int healthBarImageState;
    [SyncVar(hook =nameof(UpdateHealthBar))] private int currentHealth;
    [SyncVar] [SerializeField] private bool alive;
    [Header("Player UI")]
    [SerializeField] private RectTransform borderRectTransform;
    [SerializeField] private RectTransform fillRectTransform;
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
        }
        else if (id == 2)
        {
            CmdChangeHealthbarSprite(2);
            fillRectTransform.anchoredPosition = new Vector2(fillRectTransform.anchoredPosition.x, fillRectTransform.anchoredPosition.y + 12);
        }
        else if (id == 3)
        {
            CmdChangeHealthbarSprite(3);
            Vector3 temp = borderRectTransform.localScale;
            temp.x = -temp.x;
            borderRectTransform.localScale = temp;
            temp = fillRectTransform.localScale;
            temp.x = -temp.x;
            fillRectTransform.localScale = temp;
            fillRectTransform.anchoredPosition = new Vector2(fillRectTransform.anchoredPosition.x, fillRectTransform.anchoredPosition.y + 12);
        }
        
        string curSceneName = SceneManager.GetActiveScene().name;

        //healthBarImageState = 0;

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
        else if (healthBarImageState == 2)
        {
            healthBarImage.sprite = healthBarSpriteUp;
        }
        else if (healthBarImageState == 3)
        {
            healthBarImage.sprite = healthBarDeadSpriteUp;
        }
    }

    // вкл/выкл healthbar
    public void ToggleHealthBar(bool state)
    {
        HB.SetActive(state);
    }
}
