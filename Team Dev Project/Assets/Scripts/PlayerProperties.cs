using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerProperties : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float spikesHitCD;
    [SerializeField] Animator animator;
    public bool allowInput = true;

    [Header("Player UI")]
    [SerializeField] GameObject playerUI;
    [SerializeField] RectTransform playerUIRectTransform;
    [SerializeField] TMP_Text playerNameText;
    
    private float spikesHitTimer = 0;
    private Health health;
    private Rigidbody2D m_Rigidbody2D;
    [SyncVar] public int playerId;

    private void Start()
    {
        health = GetComponent<Health>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        playerNameText.SetText(gameObject.GetComponent<PlayerNetworkTalker>().getPlayerName());

        if (playerId == 0)
        {
            playerUIRectTransform.anchorMin = new Vector2(0, 0);
            playerUIRectTransform.anchorMax = new Vector2(0, 0);
            playerUIRectTransform.pivot = new Vector2(0, 0);
            playerUIRectTransform.anchoredPosition = new Vector3(20, 20, 0);
        }
        else if (playerId == 1)
        {
            playerUIRectTransform.anchorMin = new Vector2(1, 0);
            playerUIRectTransform.anchorMax = new Vector2(1, 0);
            playerUIRectTransform.pivot = new Vector2(1, 0);
            playerUIRectTransform.anchoredPosition = new Vector3(-20, 20, 0);
        }
        else if (playerId == 2)
        {
            playerUIRectTransform.anchorMin = new Vector2(0, 1);
            playerUIRectTransform.anchorMax = new Vector2(0, 1);
            playerUIRectTransform.pivot = new Vector2(0, 1);
            playerUIRectTransform.anchoredPosition = new Vector3(20, -20, 0);
        }
        else if (playerId == 3)
        {
            playerUIRectTransform.anchorMin = new Vector2(1, 1);
            playerUIRectTransform.anchorMax = new Vector2(1, 1);
            playerUIRectTransform.pivot = new Vector2(1, 1);
            playerUIRectTransform.anchoredPosition = new Vector3(-20, -20, 0);
        }

        string curScene = SceneManager.GetActiveScene().name;
        if (curScene.StartsWith("LevelScene"))
        {
            playerUI.SetActive(true);
        }
        else if (curScene.StartsWith("HubScene"))
        {
            playerUI.SetActive(false);
        }


    }

    private void Update()
    {
        if (spikesHitTimer > 0)
        {
            spikesHitTimer -= Time.deltaTime;
        }
    }

    public void getHitFromSpikes(int damage)
    {
        if (spikesHitTimer <= 0)
        {
            health.CmdDealDamage(damage);
            animator.SetTrigger("Hit");
            spikesHitTimer = spikesHitCD;

            m_Rigidbody2D.AddForce(new Vector2(0f, - m_Rigidbody2D.velocity.y) * 100);
        }
    }
}
