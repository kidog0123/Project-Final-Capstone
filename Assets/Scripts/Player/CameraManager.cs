using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
public class CameraManager : MonoBehaviour
{

    [SerializeField] [Range(-5, 5)] private float _defaultSenitivity = 1f; public static float defaultSenitivity { get { return singleton._defaultSenitivity; } }

    [SerializeField] [Range(-5, 5)] private float _aimingSenitivity = 0.5f; public static float aimingSenitivity { get { return singleton._aimingSenitivity; } }

    [SerializeField] private Camera _camera = null;public static Camera mainCamera { get { return singleton._camera; } }
    [SerializeField] private CinemachineVirtualCamera _playerCamera = null; public static CinemachineVirtualCamera playerCamera { get { return singleton._playerCamera; } }
    [SerializeField] private CinemachineVirtualCamera _aimingCamera = null; public static CinemachineVirtualCamera aimingCamera { get { return singleton._aimingCamera; } }
    [SerializeField] private CinemachineBrain _cameraBrain = null;
    [SerializeField] private LayerMask _aimLayer;

    public static CameraManager _singleton = null;

    public static CameraManager singleton
    {
        get
        {
            if(_singleton == null)
            {
                _singleton = FindFirstObjectByType<CameraManager>();
            }
            return _singleton;
        }
    }

    private bool _aiming = false; public bool aiming { get { return _aiming; } set { _aiming = value; } }

    private Vector3 _aimTargetPoint = Vector3.zero; public Vector3 aimTargetPoint { get { return _aimTargetPoint; } }

    private Transform _aimTargetObject = null; public Transform aimTargetObject { get { return _aimTargetObject; } }
    public float sensitivity { get { return _aiming ? _aimingSenitivity : _defaultSenitivity; } }
    private void Awake()
    {
        _cameraBrain.m_DefaultBlend.m_Time = 0.1f;
    }

    private void Update()
    {
        _aimingCamera.gameObject.SetActive(_aiming);
        SetAimTarget();
    }

    private void SetAimTarget()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
        if(Physics.Raycast(ray,out RaycastHit hit, 1000f, _aimLayer))
        {
            _aimTargetPoint = hit.point;
            _aimTargetObject = hit.transform;
        }
        else
        {
            _aimTargetPoint = ray.GetPoint(1000);
            _aimTargetObject = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_aimTargetPoint, 0.1f);
    }
#endif
}
