using UnityEngine;
using Mirror;

public class Chest : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject coin;
    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;
    [SerializeField] private float timeDelay;
    [SerializeField] private float coinSpawnOffset;
    [SerializeField] public int chestID;
    //[SyncVar(hook = nameof(OpenChest))]private bool opened = false;
    private bool opened = false;
    private double timePassed = 0;
    private int leftCoins;
    private System.Random rand;

    private void Start()
    {
        leftCoins = maxCoins;
        rand = new System.Random();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.GetComponent<PlayerProperties>().allowInput && Input.GetButton("Interact") && !opened)
            {

                if (collision.GetComponent<PlayerNetworkTalker>().IsLocal())
                {
                    Debug.Log("Here");
                    //collision.GetComponent<PlayerNetworkTalker>().CmdOpenChest(chestID);
                    CmdOpenChest();
                }
                
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdOpenChest()
    {
        for (int i = 0; i < 10; i++)
        {
            ServerSpawnCoin();
        }
        RpcOpenChest();
    }

    [Server]
    private void ServerSpawnCoin()
    {
        float coinForceX = (float)(rand.NextDouble() - 0.5) * 50;
        GameObject spawnedCoin = GameObject.Instantiate(coin, new Vector3(transform.position.x, transform.position.y, transform.position.z), new Quaternion());
        spawnedCoin.GetComponent<Rigidbody2D>().AddForce(new Vector2(coinForceX, 250));//, ForceMode2D.Impulse);
        NetworkServer.Spawn(spawnedCoin);
    }

    [ClientRpc]
    private void RpcOpenChest()
    {
        animator.SetBool("Opened", true);
        opened = true;
    }

    


        /*
    public void OpenChest(bool oldValue, bool newValue)
    {
        animator.SetBool("Opened", true);
    }*/

    

    /*
    private void Update()
    {
        if (opened && leftCoins > 0)
        {
            timePassed += Time.deltaTime;

            if (timePassed > timeDelay)
            {
                float coinForceX = (float)(rand.NextDouble() - 0.5) * 50;

                GameObject spawnedCoin = GameObject.Instantiate(coin, new Vector3(transform.position.x, transform.position.y, transform.position.z), new Quaternion());
                spawnedCoin.GetComponent<Rigidbody2D>().AddForce(new Vector2(coinForceX, 250));//, ForceMode2D.Impulse);
                
                timePassed = 0;
                leftCoins -= 1;
            }
        }
    }
    */
}
