using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [SerializeField] private Item[] _items = null;
    [SerializeField] private Weapon[] _weapons = null;

    [SerializeField] private Character[] _characters = null;


    private  static PrefabManager _singleton = null;

    public static PrefabManager singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = FindFirstObjectByType<PrefabManager>();
            }
            return _singleton;
        }
    }

    public Item GetItemPrefab(string id) 
    {
        if(_items != null)
        {
            for(int i = 0; i <= _items.Length; i++) 
            {
                if (_items[i] != null && _items[i].id == id)
                {
                    return _items[i];
                }
            }
        }

        return null;
    }
    public Character GetCharacterPrefab(string id) 
    {
        if(_characters != null)
        {
            for(int i = 0; i <= _characters.Length; i++) 
            {
                if (_characters[i] != null && _characters[i].id == id)
                {
                    return _characters[i];
                }
            }
        }

        return null;
    }

    public Character GetCharacterPrefabByCount(int id)
    {
        if(_characters != null)
        {
           for(int i = 0; i <= _characters.Length; i++)
            {
                if (_characters[i] != null && id == i)
                {
                    return _characters[i];
                }
            }
        }
        return null;
    }
    public Weapon GetWeaponPrefabByCount(int id)
    {
        if (_weapons != null)
        {
            for (int i = 0; i <= _weapons.Length; i++)
            {
                if (_weapons[i] != null && id == i)
                {
                    return _weapons[i];
                }
            }
        }

        return null;
    }


    public int CharacterCount()
    {
        return _characters.Length;
    }
    public int WeaponCount()
    {
        return _weapons.Length;
    }
}
