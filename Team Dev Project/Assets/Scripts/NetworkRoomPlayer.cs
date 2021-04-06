using System;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using Steamworks;


public class NetworkRoomPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private RawImage[] playerImages = new RawImage[4];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Texture2D _emptyPlayerImage;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    private ulong steamId;
    private RoomsCanvases Canvases;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    public Texture2D profilePicture;

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

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;


    public override void OnStartAuthority()
    {
        Canvases = GameObject.Find("Canvases").GetComponent<RoomsCanvases>();

        var cSteamId = new CSteamID(steamId);
        CmdSetDisplayName(SteamFriends.GetFriendPersonaName(cSteamId));

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);

        UpdateDisplay();
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != steamId)
        {
            return;
        }

        profilePicture = GetSteamImageAsTexture(callback.m_iImage);
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue)
    {
        UpdateDisplay();
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue)
    {
        UpdateDisplay();
    }

    private void HandleSteamIdUpdated(ulong oldValue, ulong newValue)
    {
        UpdateDisplay();
        var cSteamId = new CSteamID(newValue);

        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);

        if (imageId == -1)
            return;

        profilePicture = GetSteamImageAsTexture(imageId);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }

    public void SetSteamId(ulong steamId)
    {
        this.steamId = steamId;
    }

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
            playerImages[i].texture = _emptyPlayerImage;
            playerImages[i].transform.localScale = new Vector3(1, 1, 1);
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
            playerImages[i].texture = Room.RoomPlayers[i].profilePicture;
            playerImages[i].transform.localScale = new Vector3(1, -1, 1);
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader)
        {
            return;
        }

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName; 
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient)
        {
            return;
        }

        Room.StartGame();
    }

    public void Disconnect()
    {
        Room.steamLobby.LeaveLobby();

        if (isLeader)
        {
            Room.StopClient();
            Room.StopServer();
        }
        else
        {
            Room.StopClient();
        }

        Canvases.MainMenuCanvas.Show();
    }
}
