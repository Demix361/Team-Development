using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    [SerializeField] private Popup popup;

    private PlayerNetworkTalker playerNetworkTalker;
    private PlayerProperties playerProperties;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButton("Interact"))
        {
            playerNetworkTalker = collision.GetComponent<PlayerNetworkTalker>();
            playerProperties = collision.GetComponent<PlayerProperties>();

            playerProperties.allowInput = false;

            endScreen.SetActive(true);

            // Сохранение
            playerNetworkTalker.CmdSaveLevel();
            playerNetworkTalker.CmdSaveGems();
        }
    }

    public void ButtonOK()
    {   
        if (playerProperties)
        {
            playerProperties.allowInput = true;
        }
        playerNetworkTalker.CmdChangeScene("HubScene");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            popup.SetPopup(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            popup.SetPopup(false);
    }
}
