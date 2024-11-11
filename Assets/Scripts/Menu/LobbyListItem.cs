using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyListItem : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] public TextMeshProUGUI playersText = null;
    [SerializeField] public TextMeshProUGUI hostText = null;
    [SerializeField] public TextMeshProUGUI modeText = null;
    [SerializeField] public TextMeshProUGUI mapText = null;
    [SerializeField] private Button joinButton = null;

    private Lobby lobby = null;

    private void Start()
    {
        joinButton.onClick.AddListener(Join);
    }

    public void Initialize(Lobby lobby)
    {
        this.lobby = lobby;
        nameText.text = lobby.Name;
        playersText.text = lobby.Players.Count.ToString() + "/" + lobby.MaxPlayers.ToString();
        for (int i = 0; i < lobby.Players.Count; i++)
        {
            if (lobby.Players[i].Id == lobby.HostId)
            {
                hostText.text = lobby.Players[i].Data["name"].Value;
                break;
            }
        }
        modeText.text = lobby.Data["mode"].Value;
        mapText.text = lobby.Data["map"].Value;

    }

    private void Join()
    {
        LobbyMenu panel = (LobbyMenu)PanelManager.GetSingleton("lobby");
        panel.JoinLobby(lobby.Id);
    }

}