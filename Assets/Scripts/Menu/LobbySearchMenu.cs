using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using System;

public class LobbySearchMenu : Panel
{

    [SerializeField] private LobbyListItem lobbyListItemPrefab = null;
    [SerializeField] private RectTransform lobbyListContainer = null;
    [SerializeField] private Button closeButton = null;
    [SerializeField] private Button createButton = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        ClearLobbyList();
        createButton.onClick.AddListener(Create);
        closeButton.onClick.AddListener(ClosePanel);
        base.Initialize();
    }

    public override void Open()
    {
        base.Open();
        GetLobbyListAsync();
    }

    private void Create()
    {
        LobbySettingsMenu panel = (LobbySettingsMenu)PanelManager.GetSingleton("lobby_settings");
        panel.Open(null);
    }

    private async void GetLobbyListAsync()
    {
        ClearLobbyList();
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0")
            };

            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(asc: false, field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            if (lobbies != null && lobbies.Results != null)
            {
                for (int i = 0; i < lobbies.Results.Count; i++)
                {
                    LobbyListItem item = Instantiate(lobbyListItemPrefab, lobbyListContainer);
                    item.Initialize(lobbies.Results[i]);
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    private void ClosePanel()
    {
        Close();
    }

    private void ClearLobbyList()
    {
        LobbyListItem[] items = lobbyListContainer.GetComponentsInChildren<LobbyListItem>();
        if (items != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Destroy(items[i].gameObject);
            }
        }
    }

}