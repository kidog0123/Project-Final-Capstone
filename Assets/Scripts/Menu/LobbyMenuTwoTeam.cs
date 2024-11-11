using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine.UI;
using System.Linq;

public class LobbyMenuTwoTeam : Panel
{

    [SerializeField] private LobbyPlayerItem lobbyPlayerItemPrefab = null;
    [SerializeField] private RectTransform lobbyPlayersTeam1Container = null;
    [SerializeField] private RectTransform lobbyPlayersTeam2Container = null;
    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button closeButton = null;
    [SerializeField] private Button leaveButton = null;
    [SerializeField] private Button readyButton = null;
    [SerializeField] private Button startButton = null;
    [SerializeField] private Button switchTeamButton = null;

    private Lobby lobby = null; public Lobby JoinedLobby { get { return lobby; } }
    private float updateTimer = 0;
    private float heartbeatPeriod = 15;
    private bool sendingHeartbeat = false;
    private ILobbyEvents events = null;
    private bool isReady = false;
    private bool isHost = false;
    private string eventsLobbyId = "";
    private bool isStarted = false;
    private bool isJoining = false;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        ClearPlayersList();
        closeButton.onClick.AddListener(ClosePanel);
        leaveButton.onClick.AddListener(LeaveLobby);
        readyButton.onClick.AddListener(SwitchReady);
        readyButton.onClick.AddListener(SwitchReady);
        startButton.onClick.AddListener(StartGame);
        switchTeamButton.onClick.AddListener(SwitchTeam);
        base.Initialize();
    }

    private async void StartGame()
    {
        PanelManager.Open("loading");
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var data = new RelayServerData(allocation, "dtls");
            transport.SetRelayServerData(data);
            string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);


            //set session data
            SessionManager.role = SessionManager.Role.Host;
            SessionManager.joinCode = code;
            SessionManager.lobbyID = lobby.Id;

            SetLobbyStarting();

            StartingSessionMenu panel = (StartingSessionMenu)PanelManager.GetSingleton("start");
            heartbeatPeriod = 5;
            await UnsubscribeToEventsAsync();
            panel.StartGameByLobby(lobby, false);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        PanelManager.Close("loading");
    }

    private async void SetLobbyStarting()
    {
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.Data = new Dictionary<string, DataObject>();
            options.Data.Add("started", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: "1"));
            lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    private async void CheckStartGameStatus()
    {
        StartingSessionMenu panel = (StartingSessionMenu)PanelManager.GetSingleton("start");
        isStarted = lobby.Data.ContainsKey("started");
        string joinCode = lobby.Data.ContainsKey("join_code") ? lobby.Data["join_code"].Value : null;
        if (panel.isLoading == false && isStarted)
        {
            panel.StartGameByLobby(lobby, true);
        }

        if (isJoining == false && panel.isLoading && string.IsNullOrEmpty(joinCode) == false && panel.isConfirmed == false)
        {
            panel.StartGameByLobby(lobby, true);
            JoinGame(joinCode);
        }
    }

    private async void JoinGame(string joinCode)
    {
        if (string.IsNullOrEmpty(joinCode) == false)
        {
            isJoining = true;
            PanelManager.Open("loading");
            try
            {
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                var data = new RelayServerData(allocation, "dtls");
                transport.SetRelayServerData(data);

                //set session data

                SessionManager.role = SessionManager.Role.Client;
                SessionManager.joinCode = joinCode;
                SessionManager.lobbyID = lobby.Id;

                StartingSessionMenu panel = (StartingSessionMenu)PanelManager.GetSingleton("start");
                await UnsubscribeToEventsAsync();
                panel.StartGameConfirm();
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
                await Leave();
                isJoining = false;
                PanelManager.Close("start");
            }
            PanelManager.Close("loading");
        }
    }

    private void Update()
    {
        if (lobby == null)
        {
            return;
        }

        if (isHost == false && isJoining == false)
        {
            CheckStartGameStatus();
        }

        if (lobby.HostId == AuthenticationService.Instance.PlayerId && sendingHeartbeat == false)
        {
            updateTimer += Time.deltaTime;
            if (updateTimer < heartbeatPeriod)
            {
                return;
            }
            updateTimer = 0;
            HeartbeatLobbyAsync();
        }
    }

    private async void HeartbeatLobbyAsync()
    {
        sendingHeartbeat = true;
        try
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        sendingHeartbeat = false;
    }

    public void Open(Lobby lobby)
    {
        if (eventsLobbyId != lobby.Id)
        {
            _ = SubscribeToEventsAsync(lobby.Id);
        }
        this.lobby = lobby;
        nameText.text = lobby.Name;
        CheckStartGameStatus();
        startButton.gameObject.SetActive(false);
        isHost = false;
        LoadPlayers();
        Open();
    }

    private void LoadPlayers()
    {
        ClearPlayersList();// Xóa các mục người chơi trước khi load lại

        bool isEveryoneReady = true;
        bool youAreMember = false;
        // Duyệt qua tất cả người chơi trong lobby
        for (int i = 0; i < lobby.Players.Count; i++)
        {
            bool ready = lobby.Players[i].Data["ready"].Value == "1";
            string team = lobby.Players[i].Data.ContainsKey("team") ? lobby.Players[i].Data["team"].Value : "No Team";
            // Chọn container dựa trên đội
            RectTransform container = team == "Team 1" ? lobbyPlayersTeam1Container : lobbyPlayersTeam2Container;

            LobbyPlayerItem item = Instantiate(lobbyPlayerItemPrefab, container);
            item.Initialize(lobby.Players[i], lobby.Id, lobby.HostId);

            // Kiểm tra người chơi hiện tại có phải là người đang tham gia không
            if (lobby.Players[i].Id == AuthenticationService.Instance.PlayerId)
            {
                youAreMember = true;
                isReady = ready;
                isHost = lobby.Players[i].Id == lobby.HostId;
            }
            if (ready == false)
            {
                isEveryoneReady = false;
            }
        }
        // Điều chỉnh các nút tùy thuộc vào trạng thái của lobby
        startButton.gameObject.SetActive(isHost);
        if (isHost)
        {
            startButton.interactable = isEveryoneReady;
        }
        if (youAreMember == false)
        {
            Close();// Nếu người chơi không phải là thành viên thì đóng panel
        }
    }

    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate,string map)
    {
        PanelManager.Open("loading");
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = isPrivate;
            options.Data = new Dictionary<string, DataObject>()
            {
                { "map", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: map) },
                
            };

            options.Player = new Player();
            options.Player.Data = new Dictionary<string, PlayerDataObject>();
            options.Player.Data.Add("name", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: AuthenticationService.Instance.PlayerName));
            options.Player.Data.Add("ready", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: "0"));

            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            PanelManager.Close("lobby_search");
            Open(lobby);
        }
        catch (Exception exception)
        {
            ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
            panel.Open(ErrorMenu.Action.None, "Failed to create the lobby.", "OK");
            Debug.Log(exception.Message);
        }
        PanelManager.Close("loading");
    }

    public async void JoinLobby(string id)
    {
        PanelManager.Open("loading");
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();

            options.Player = new Player();
            options.Player.Data = new Dictionary<string, PlayerDataObject>();
            options.Player.Data.Add("name", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: AuthenticationService.Instance.PlayerName));
            options.Player.Data.Add("ready", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: "0"));

            lobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, options);

            Open(lobby);
            PanelManager.Close("lobby_search");
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        PanelManager.Close("loading");
    }

    public async void UpdateLobby(string lobbyId, string lobbyName, int maxPlayers, bool isPrivate,string map)
    {
        PanelManager.Open("loading");
        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.IsPrivate = isPrivate;
            options.Name = lobbyName;
            options.MaxPlayers = maxPlayers;
            options.Data = new Dictionary<string, DataObject>()
            {
                { "map", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: map) },
                
            };
            lobby = await LobbyService.Instance.UpdateLobbyAsync(lobbyId, options);
            Open(lobby);
        }
        catch
        {
            ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
            panel.Open(ErrorMenu.Action.None, "Failed to change the lobby host.", "OK");
        }
        PanelManager.Close("loading");
    }

    private void LeaveLobby()
    {
        _ = Leave();
    }

    private async Task Leave()
    {
        PanelManager.Open("loading");
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);
            lobby = null;
            Close();
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        PanelManager.Close("loading");
    }

    private async void SwitchReady()
    {
        readyButton.interactable = false;
        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>();
            options.Data.Add("ready", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: isReady ? "0" : "1"));
            lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId, options);
            LoadPlayers();
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        readyButton.interactable = true;
    }
    private async void SwitchTeam()
    {
        switchTeamButton.interactable = false;
        try
        {
            // Kiểm tra đội hiện tại của người chơi
            var currentTeam = lobby.Players.FirstOrDefault(p => p.Id == AuthenticationService.Instance.PlayerId)?.Data["team"].Value;
            string newTeam = (currentTeam == "Team 1") ? "Team 2" : "Team 1";

            // Kiểm tra xem đội mới có đủ chỗ không
            int newTeamCount = lobby.Players.Count(p => p.Data["team"].Value == newTeam);
            if (newTeamCount < 5)  // Mỗi đội chỉ có tối đa 5 người
            {
                // Cập nhật thông tin đội cho người chơi
                UpdatePlayerOptions options = new UpdatePlayerOptions();
                options.Data = new Dictionary<string, PlayerDataObject>();
                options.Data.Add("team", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: newTeam));
                lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId, options);

                // Reload lại danh sách người chơi và cập nhật UI
                LoadPlayers();
            }
            else
            {
                // Nếu đội mới đã đầy, hiển thị thông báo lỗi
                Debug.Log("Đội mới đã đầy, không thể chuyển.");
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        switchTeamButton.interactable = true;
    }
    private void ClearPlayersList()
    {
        LobbyPlayerItem[] item1s = lobbyPlayersTeam1Container.GetComponentsInChildren<LobbyPlayerItem>();
        LobbyPlayerItem[] item2s = lobbyPlayersTeam2Container.GetComponentsInChildren<LobbyPlayerItem>();
        if (item1s != null)
        {
            for (int i = 0; i < item1s.Length; i++)
            {
                Destroy(item1s[i].gameObject);
            }
        }if (item2s != null)
        {
            for (int i = 0; i < item2s.Length; i++)
            {
                Destroy(item2s[i].gameObject);
            }
        }
    }

    private void ClosePanel()
    {
        Close();
    }

    private async Task<bool> SubscribeToEventsAsync(string id)
    {
        try
        {
            var callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnChanged;
            callbacks.LobbyEventConnectionStateChanged += OnConnectionChanged;
            callbacks.KickedFromLobby += OnKicked;
            events = await Lobbies.Instance.SubscribeToLobbyEventsAsync(id, callbacks);
            eventsLobbyId = lobby.Id;
            return true;
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
        return false;
    }

    private async Task UnsubscribeToEventsAsync()
    {
        try
        {
            if (events != null)
            {
                await events.UnsubscribeAsync();
                events = null;
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    private void OnKicked()
    {
        if (IsOpen)
        {
            Close();
        }
        lobby = null;
        events = null;
        isStarted = false;
        isJoining = false;
    }

    private void OnChanged(ILobbyChanges changes)
    {
        if (changes.LobbyDeleted)
        {
            if (IsOpen)
            {
                Close();
            }
            lobby = null;
            events = null;
            isStarted = false;
            isJoining = false;
        }
        else
        {
            changes.ApplyToLobby(lobby);
            CheckStartGameStatus();
            if (IsOpen)
            {
                LoadPlayers();
            }
        }
    }

    private async void OnConnectionChanged(LobbyEventConnectionState state)
    {
        switch (state)
        {
            case LobbyEventConnectionState.Unsubscribed:
            case LobbyEventConnectionState.Error:
                if (lobby != null)
                {

                }
                break;
            case LobbyEventConnectionState.Subscribing: break;
            case LobbyEventConnectionState.Subscribed: break;
            case LobbyEventConnectionState.Unsynced: break;
        }
    }

}