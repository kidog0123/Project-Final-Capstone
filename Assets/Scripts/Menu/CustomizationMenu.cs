using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine.UI;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using System.Drawing;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Mathematics;

public class CustomizationMenu : Panel
{
    //[SerializeField]
    //GameObject[] characterPrefabs;
    //[SerializeField]
    //GameObject[] weapons1Prefabs;
    //[SerializeField]
    //GameObject[] weapons2Prefabs;

    
    
    [SerializeField] public TextMeshProUGUI weapon1Text = null;
    [SerializeField] public TextMeshProUGUI weapon2Text = null;
    [SerializeField] private Button characterButton = null;
    [SerializeField] private Button weapon1Button = null;
    [SerializeField] private Button weapon2Button = null;
    [SerializeField] private Button closeButton = null;
    [SerializeField] private Button saveButton = null;

    [SerializeField] private Transform _characterSpawnPoint = null;
    [SerializeField] private Transform _weapon1SpawnPoint = null;
    [SerializeField] private Transform _weapon2SpawnPoint = null;
    [SerializeField] private Camera _characterCamera = null;
    [SerializeField] private Camera _weapon1Camera = null;
    [SerializeField] private Camera _weapon2Camera = null;

    private int savedCharacter = 0;
    private int savedWeapon1 = 0;
    private int savedWeapon2 = 0;

    private int characterCurrent = 0;
    private int weapon1Current = 0;
    private int weapon2Current = 0;

    private Character _character;
    private Weapon _weapon1;
    private Weapon _weapon2;
    public override void Initialize()
    {

        if (IsInitialized)
        {
            return;
        }
        closeButton.onClick.AddListener(ClosePanel);
        characterButton.onClick.AddListener(ChangeCharacter);
        weapon1Button.onClick.AddListener(ChangeWeapon1);
        weapon2Button.onClick.AddListener(ChangeWeapon2);
        saveButton.onClick.AddListener(Save);
        base.Initialize();
    }

    public override void Open()
    {
        _characterCamera.enabled = true;
        _weapon1Camera.enabled = true;
        _weapon2Camera.enabled = true;
        base.Open();


        LoadData();
    }
    private void ClosePanel()
    {
        _characterCamera.enabled = false;
        _weapon1Camera.enabled = false;
        _weapon2Camera.enabled = false;
        Close();
        ClearCharacter();
        ClearWeapon(3);

    }


    private async void LoadData()
    {
        weapon1Text.text = "";
        weapon2Text.text = "";
        weapon2Button.interactable = false;
        weapon1Button.interactable = false;
        saveButton.interactable = false;
        weapon1Current = 0;
        weapon2Current = 0;
        savedWeapon1 = 0;
        savedWeapon2 = 0;
        try
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {"characterCurrent"}, new LoadOptions(new PublicReadAccessClassOptions()));
            if (playerData.TryGetValue("characterCurrent", out var characterData))
            {
                var data = characterData.Value.GetAs<Dictionary<string, object>>();
                savedWeapon1 = int.Parse(data["type"].ToString());
                savedWeapon1 = int.Parse(data["weapon1_index"].ToString());
                savedWeapon2 = int.Parse(data["weapon2_index"].ToString());
                weapon1Current = savedWeapon1;
                weapon2Current = savedWeapon2;
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }

        ShowCharacter(characterCurrent);
        ShowWeapon1(weapon1Current);
        ShowWeapon2(weapon2Current);
        weapon1Button.interactable = true;
        weapon2Button.interactable = true;
        ApplyData();
    }

    private async void Save()
    {
        saveButton.interactable = false;

        characterButton.interactable = false;
        weapon1Button.interactable = false;
        weapon2Button.interactable = false;
        try
        {
            var playerData = new Dictionary<string, object>
            {

                {"type",characterCurrent }  ,  
                {"weapon1_index",weapon1Current }  ,  
                {"weapon2_index",weapon2Current }    
            };
            var data = new Dictionary<string, object> { { "characterCurrent", playerData } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data, new SaveOptions(new PublicWriteAccessClassOptions()));

            savedCharacter = characterCurrent;
            savedWeapon1 = weapon1Current;
            savedWeapon2 = weapon2Current;
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
            saveButton.interactable = true;
        }
        characterButton.interactable = true;
        weapon1Button.interactable = true;
        weapon2Button.interactable = true;
    }

    private void ChangeCharacter()
    {
        ClearCharacter();
        characterCurrent++;
        if(characterCurrent >= PrefabManager.singleton.CharacterCount())
        {
            characterCurrent = 0;
        }       
        Debug.Log("characterCurrent: " + characterCurrent);
        ShowCharacter(characterCurrent);
        ApplyData();
    }

    private void ChangeWeapon1()
    {
        ClearWeapon(1);
        weapon1Current++;
        if (weapon1Current >= PrefabManager.singleton.WeaponCount())
        {
            weapon1Current = 0;
        }
        Debug.Log("weapon 1: " + PrefabManager.singleton.GetWeaponPrefabByCount(weapon1Current).id);
        ShowWeapon1(weapon1Current);
        ApplyData();
    }
    private void ChangeWeapon2()
    {
        ClearWeapon(2);
        weapon2Current++;
        if (weapon2Current >= PrefabManager.singleton.WeaponCount())
        {
            weapon2Current = 0;
        }
        Debug.Log("weapon 2: " + PrefabManager.singleton.GetWeaponPrefabByCount(weapon2Current).id);
        ShowWeapon2(weapon2Current);
        ApplyData();
    }

   private void ShowCharacter(int character)
    {
        
        Character prefabs = PrefabManager.singleton.GetCharacterPrefabByCount(character);
        if(prefabs != null)
        {
            if(_character != null)
            {
                Destroy(_character.gameObject);
            }
             _character = Instantiate(prefabs, _characterSpawnPoint.position, _characterSpawnPoint.rotation);
            
            CharacterController _controller = _character.GetComponent<CharacterController>();
            StarterAssetsInputs _input = _character.GetComponent<StarterAssetsInputs>();
            PlayerInput _playerInput = _character.GetComponent<PlayerInput>();
            ThirdPersonController _tps = _character.GetComponent<ThirdPersonController>();
            if (_tps != null)
            {
                Destroy(_tps);
            }
            if (_playerInput != null)
            {
                Destroy(_playerInput);
            }
            if (_input != null)
            {
                Destroy(_input);
            }
            if (_controller != null)
            {
                Destroy(_controller);
            }
            
        }
    }

    private void ShowWeapon1(int weapon1)
    {
        Weapon prefabs = PrefabManager.singleton.GetWeaponPrefabByCount(weapon1);
        if (prefabs != null)
        {
            if (_weapon1 != null)
            {
                Destroy(_weapon1.gameObject);
            }
            _weapon1 = Instantiate(prefabs,_weapon1SpawnPoint.position,_weapon1SpawnPoint.rotation);

            _weapon1.gameObject.GetComponent<Rigidbody>().useGravity = false;

        }
    }
    private void ShowWeapon2(int weapon2)
    {
        Weapon prefabs = PrefabManager.singleton.GetWeaponPrefabByCount(weapon2);
        if (prefabs != null)
        {
            if (_weapon2 != null)
            {
                Destroy(_weapon2.gameObject);
            }
            _weapon2 = Instantiate(prefabs, _weapon2SpawnPoint.position, _weapon2SpawnPoint.rotation);

            _weapon2.gameObject.GetComponent<Rigidbody>().useGravity = false;



        }
    }
    private void ClearCharacter()
    {
        if(_character != null)
        {
            Destroy(_character.gameObject);
            _character = null;
        }
    }
    private void ClearWeapon(int i)
    {
        if(i == 1)
        {
            if(_weapon1 != null)
            {
                Destroy( _weapon1.gameObject);
                _weapon1 = null;
            }
        }
        else if(i == 2)
        {
            if( _weapon2 != null)
            {
                Destroy( _weapon2.gameObject);
                _weapon2 = null;
            }
        }
        else
        {
            Destroy(_weapon1.gameObject);
            Destroy(_weapon2.gameObject);
            _weapon1 = null;
            _weapon2 = null;
        }
    }
    private void ApplyData()
    {
        //weapon1Text.text = weapons1Prefabs.PrefabList[weapon1Current].Prefab.GetComponent<Weapon>().id;
        //weapon2Text.text = weapons1Prefabs.PrefabList[weapon2Current].Prefab.GetComponent<Weapon>().id;

        weapon1Text.text = PrefabManager.singleton.GetWeaponPrefabByCount(weapon1Current).id;
        weapon2Text.text = PrefabManager.singleton.GetWeaponPrefabByCount(weapon2Current).id;
        saveButton.interactable = characterCurrent != savedCharacter || weapon1Current != savedWeapon1 || weapon2Current != savedWeapon2;
    }

    
}