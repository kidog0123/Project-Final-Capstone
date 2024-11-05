using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine.UI;
//using Unity.Services.Friends;
//using Unity.Services.Lobbies;
//using Unity.Services.Lobbies.Models;

public class MainMenu : Panel
{

    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button logoutButton = null;
    //[SerializeField] private Button leaderboardsButton = null;
    //[SerializeField] private Button friendsButton = null;
    //[SerializeField] private Button renameButton = null;
    //[SerializeField] private Button customizationButton = null;
    //[SerializeField] private Button lobbyButton = null;

    private bool isFriendsServiceInitialized = false;
    private List<string> joinedLobbyIds = new List<string>();

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        logoutButton.onClick.AddListener(SignOut);
        //leaderboardsButton.onClick.AddListener(Leaderboards);
        //friendsButton.onClick.AddListener(Friends);
        //renameButton.onClick.AddListener(RenamePlayer);
        //customizationButton.onClick.AddListener(Customization);
       // lobbyButton.onClick.AddListener(Lobby);
        base.Initialize();
    }

    public override void Open()
    {
        //friendsButton.interactable = isFriendsServiceInitialized;
        UpdatePlayerNameUI();
        //if (isFriendsServiceInitialized == false)
        //{
        //    InitializeFriendsServiceAsync();
        //}
        base.Open();
    }

    //private async void Lobby()
    //{
    //    PanelManager.Open("loading");
    //    try
    //    {
    //        var lobbyIds = await LobbyService.Instance.GetJoinedLobbiesAsync();
    //        joinedLobbyIds = lobbyIds;
    //    }
    //    catch (Exception exception)
    //    {
    //        Debug.Log(exception.Message);
    //    }

    //    Lobby lobby = null;
    //    if (joinedLobbyIds.Count > 0)
    //    {
    //        try
    //        {
    //            lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobbyIds.Last());
    //        }
    //        catch (Exception exception)
    //        {
    //            Debug.Log(exception.Message);
    //        }
    //    }

    //    if (lobby == null)
    //    {
    //        LobbyMenu panel = (LobbyMenu)PanelManager.GetSingleton("lobby");
    //        if (panel.JoinedLobby != null && joinedLobbyIds.Count > 0 && panel.JoinedLobby.Id == joinedLobbyIds.Last())
    //        {
    //            lobby = panel.JoinedLobby;
    //        }
    //    }

    //    if (lobby != null)
    //    {
    //        LobbyMenu panel = (LobbyMenu)PanelManager.GetSingleton("lobby");
    //        panel.Open(lobby);
    //    }
    //    else
    //    {
    //        PanelManager.Open("lobby_search");
    //    }

    //    PanelManager.Close("loading");
    //}

    private void Customization()
    {
        PanelManager.Open("customization");
    }

    //private async void InitializeFriendsServiceAsync()
    //{
    //    try
    //    {
    //        await FriendsService.Instance.InitializeAsync();
    //        isFriendsServiceInitialized = true;
    //        friendsButton.interactable = true;
    //    }
    //    catch (Exception exception)
    //    {
    //        Debug.Log(exception.Message);
    //    }
    //}

    private void SignOut()
    {
        //ActionConfirmMenu panel = (ActionConfirmMenu)PanelManager.GetSingleton("action_confirm");
        //panel.Open(SignOutResult, "Are you sure that you want to sign out?", "Yes", "No");
        MenuManager.Singleton.SignOut();
    }

    //private void SignOutResult(ActionConfirmMenu.Result result)
    //{
    //    if (result == ActionConfirmMenu.Result.Positive)
    //    {
    //        MenuManager.Singleton.SignOut();
    //        isFriendsServiceInitialized = false;
    //    }
    //}

    private void UpdatePlayerNameUI()
    {
        nameText.text = AuthenticationService.Instance.PlayerName;
    }

    private void Leaderboards()
    {
        PanelManager.Open("leaderboards");
    }

    private void Friends()
    {
        PanelManager.Open("friends");
    }

    //private void RenamePlayer()
    //{
    //    GetInputMenu panel = (GetInputMenu)PanelManager.GetSingleton("input");
    //    panel.Open(RenamePlayerConfirm, GetInputMenu.Type.String, 20, "Enter a name for your account.", "Send", "Cancel");
    //}

    //private async void RenamePlayerConfirm(string input)
    //{
    //    renameButton.interactable = false;
    //    try
    //    {
    //        await AuthenticationService.Instance.UpdatePlayerNameAsync(input);
    //        UpdatePlayerNameUI();
    //    }
    //    catch
    //    {
    //        ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
    //        panel.Open(ErrorMenu.Action.None, "Failed to change the account name.", "OK");
    //    }
    //    renameButton.interactable = true;
    //}

}