﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : Item
{
    [Header("Settings")]
    [SerializeField] private Handle _type = Handle.TwoHanded; public Handle type { get { return _type; } }
    [SerializeField] private string _ammoID = ""; public string ammoID { get { return _ammoID; } }
    [SerializeField] private float _damage; // sát thương
    [SerializeField] private float _fireRate; // tốc độ bắn
    [SerializeField] private int _clipSize = 30; public int clipSize { get { return _clipSize; } }

    [SerializeField] private float _handKick = 5f; public float handKick { get { return _handKick; } }
    [SerializeField] private float _bodyKick = 5f; public float bodyKick { get { return _bodyKick; } }

   
    [SerializeField] private Vector3 _leftHandPosition = Vector3.zero; public Vector3 leftHandPosition {  get { return _leftHandPosition; } }
    [SerializeField] private Vector3 _leftHandRotation = Vector3.zero; public Vector3 leftHandRotaion {  get { return _leftHandRotation; } }
    [SerializeField] private Vector3 _rightHandPosition = Vector3.zero; public Vector3 rightHandPosition {  get { return _rightHandPosition; } }
    [SerializeField] private Vector3 _rightHandRotation = Vector3.zero; public Vector3 rightHandRotation {  get { return _rightHandRotation; } }

    [Header("Referances")]
    [SerializeField] private Transform _muzzle = null;
    [SerializeField] private ParticleSystem _flash = null;
    [SerializeField] private ParticleSystem _bulletShell = null;
   
    [Header("Prefabs")]
    [SerializeField] private Projectile _projectile = null;
    public enum Handle
    {
        OneHanded = 1, TwoHanded = 2
    }

    private float _fireTimer = 0f;
    private int _ammo = 0; public int ammo { get { return _ammo; } set { _ammo = value; } }

    public override void Awake()
    {
        base.Awake();
        _fireTimer += Time.realtimeSinceStartup;
    }
    public bool Shoot(Character character,Vector3 target)
    {
        float passedTime = Time.realtimeSinceStartup - _fireTimer;
        if(_ammo > 0 && passedTime > _fireRate )
        {
            _ammo -= 1;
            _fireTimer = Time.realtimeSinceStartup;
            Projectile projectile = Instantiate(_projectile, _muzzle.position,Quaternion.identity);
            projectile.Initialize(character, target,_damage);
           if(_flash != null)
            {
                _flash.Play();
            }
            if(_bulletShell != null)
            {
                _bulletShell.Play();
            }
            return true;
        }
        return false;
    }
   
}
