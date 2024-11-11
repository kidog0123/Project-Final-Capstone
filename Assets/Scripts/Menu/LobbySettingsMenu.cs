using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LobbySettingsMenu : Panel
{

    [SerializeField] private Button confirmButton = null;
    [SerializeField] private Button cancelButton = null;
    [SerializeField] private TMP_InputField nameInput = null;
    [SerializeField] private TMP_Dropdown visibilityDropdown = null;
    [SerializeField] private TMP_Dropdown mapDropdown = null;

    private Lobby lobby = null;
    int maxPlayer = 10;
    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Cancel);
        nameInput.contentType = TMP_InputField.ContentType.Standard;
        
        nameInput.characterLimit = 20;
        base.Initialize();
    }

    public void Open(Lobby lobby)
    {
        this.lobby = lobby;
        if (lobby == null)
        {
            nameInput.name = "";
            visibilityDropdown.SetValueWithoutNotify(0);
            mapDropdown.SetValueWithoutNotify(0);

        }
        else
        {
            nameInput.name = lobby.Name;
            visibilityDropdown.SetValueWithoutNotify(lobby.IsPrivate ? 1 : 0);
            for (int i = 0; i < visibilityDropdown.options.Count; i++)
            {
                if ((lobby.IsPrivate && visibilityDropdown.options[i].text.ToLower() == "private") || (lobby.IsPrivate == false && visibilityDropdown.options[i].text.ToLower() == "public"))
                {
                    visibilityDropdown.SetValueWithoutNotify(i);
                    break;
                }
            }

            if (lobby.Data.ContainsKey("map"))
            {
                var gameMap = lobby.Data["map"].Value.ToLower();
                for (int i = 0; i < mapDropdown.options.Count; i++)
                {
                    if (mapDropdown.options[i].text.ToLower() == gameMap)
                    {
                        mapDropdown.SetValueWithoutNotify(i);
                        break;
                    }
                }
            }
 
        }
        Open();
    }

    private void Confirm()
    {
        string lobbyName = nameInput.text.Trim();
        
        bool isPrivate = visibilityDropdown.captionText.text.Trim().ToLower() == "private" ? true : false;
        string map = mapDropdown.captionText.text.Trim();
        if (maxPlayer > 0 && string.IsNullOrEmpty(lobbyName) == false)
        {
            LobbyMenu panel = (LobbyMenu)PanelManager.GetSingleton("lobby");
            if (lobby == null)
            {
                panel.CreateLobby(lobbyName, maxPlayer, isPrivate, map);
            }
            else
            {
                panel.UpdateLobby(lobby.Id, lobbyName, maxPlayer, isPrivate,map);
            }
            Close();
        }
    }

    private void Cancel()
    {
        Close();
    }

    public override void Close()
    {
        base.Close();
        lobby = null;
    }

}