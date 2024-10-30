using System;
//using UnityEditorInternal;
using UnityEngine;


#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;

#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
       

        [Header("Player")]
        [SerializeField] public float WalkSpeed = 2.0f;
        [SerializeField] public float RunSpeed = 4.0f;
        [SerializeField] public float SprintSpeed = 5.335f;
        public float AimRotationSpeed = 20f;


        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private Character _character;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private RigManager _rigManager;

        private const float _threshold = 0.01f;

        
        private float targetSpeed = 2f;

        private bool _initialized = false;

        
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            
          
            
        }

        private void Start()
        {
            _rigManager = GetComponent<RigManager>();
            _character = GetComponent<Character>();
            
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

            _playerInput = GetComponent<PlayerInput>();
            
        }

        private void DestroyControllers()
        {
            Destroy(this);
            Destroy(_playerInput);
            Destroy(_input);
            Destroy(_controller);
        }


        private void Update()
        {
            if(_initialized == false)
            {
                if(_character.IsOwner) 
                {
                    _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

                    AssignAnimationIDs();

                    _mainCamera = CameraManager.mainCamera.gameObject;
                    CameraManager.playerCamera.m_Follow = CinemachineCameraTarget.transform;
                    CameraManager.aimingCamera.m_Follow = CinemachineCameraTarget.transform;

                    // reset our timeouts on start
                    _jumpTimeoutDelta = JumpTimeout;

                    _initialized = true;
                }
                else
                {
                    if(_character.clientID > 0)
                    {
                        DestroyControllers();
                    }
                    return;
                }

            }

            bool armed = _character.weapon != null;
            if (armed)
            {
                _character.aiming = _input.aim;

            }
            _character.sprinting = _input.sprint && _character.aiming == false;

            JumpAndGravity();
            //GroundedCheck();

            if(_input.holsterWeapon)
            {
                if(!_character.reloading && !_character.switchingWeapon)
                {
                    _character.HolsterWeapon();
                }
                _input.holsterWeapon = false;
            }

            if(_input.walk)
            {
                _input.walk = false;
                _character.walking  = !_character.walking;
            }

            if(_character.sprinting)
            {
                targetSpeed = SprintSpeed;
               
            }
            else if( _character.walking)
            {
                targetSpeed = WalkSpeed;
               
            }
            else
            {
                targetSpeed = RunSpeed;

            }


            if(_input.shoot)
            {
                _character.Shoot();
            }

            //reloading
            if(_input.reload)
            {
                if (!_character.reloading && !_character.switchingWeapon)
                {
                    _character.Reload();
                }
                _input.reload = false;
                
            }
            
            //switch weapon
            if(_input.switchWeaponn != 0)
            {
                _character.ChangeWeapon(_input.switchWeaponn);
            }

           

            CameraManager.singleton.aiming = _character.aiming;
            _character.aimTarget = CameraManager.singleton.aimTargetPoint;

            if(_input.inventory)
            {
                if(CanvasManager.singleton.isInventoryOpen)
                {
                    CanvasManager.singleton.CloseInventory();
                }
                else
                {
                    CanvasManager.singleton.OpenInventory();
                }
                _input.inventory = false;
            }

            float maxPickUpItem = 3f;
            Item itemtoPick = null;
            Character characterToLoot = null;

            if(CanvasManager.singleton.isInventoryOpen == false && CameraManager.singleton.aimTargetObject != null )
            {
                if ( CameraManager.singleton.aimTargetObject.tag == ("Item") &&
                                            Vector3.Distance(CameraManager.singleton.aimTargetObject.position, transform.position) <= maxPickUpItem)
                {
                    itemtoPick = CameraManager.singleton.aimTargetObject.GetComponent<Item>();
                    if (itemtoPick != null && itemtoPick.canBePickedUp == false)
                    {
                        itemtoPick = null;
                    }
                }
                else if ( CameraManager.singleton.aimTargetObject.root.tag == ("Character") &&
                                            Vector3.Distance(CameraManager.singleton.aimTargetObject.position, transform.position) <= maxPickUpItem +5)
                {
                    characterToLoot = CameraManager.singleton.aimTargetObject.root.GetComponent<Character>();
                    if (characterToLoot != null && characterToLoot.health > 0)
                    {
                        characterToLoot = null;
                    }
                }
            }
            
            if(CanvasManager.singleton.characterToLoot == null && CanvasManager.singleton.itemToPick != itemtoPick )
            {
                CanvasManager.singleton.itemToPick = itemtoPick;
            }
            else if(CanvasManager.singleton.itemToPick == null && CanvasManager.singleton.characterToLoot != characterToLoot)
            {
                CanvasManager.singleton.characterToLoot = characterToLoot;
            }


            if(_input.pickUpItem)
            {
                if(CanvasManager.singleton.itemToPick != null)
                {
                    _character.PickupItem(CanvasManager.singleton.itemToPick.networkID);
                }
                else if(CanvasManager.singleton.characterToLoot != null)
                {
                    CanvasManager.singleton.OpenInventoryForLoot(CanvasManager.singleton.characterToLoot);
                }
                _input.pickUpItem = false;
            }

            Move();
            Rotate();

            
        }

        private void LateUpdate()
        {
            if(_initialized == false)
            {
                return;
            }
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        

        private void CameraRotation()
        {
            Vector2 _lookInput = _input.look;
            if(CanvasManager.singleton.isInventoryOpen)
            {
                _lookInput = Vector2.zero;
            }

            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _lookInput.x * deltaTimeMultiplier * CameraManager.singleton.sensitivity;
                _cinemachineTargetPitch += _lookInput.y * deltaTimeMultiplier * CameraManager.singleton.sensitivity;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Rotate()
        {
            if(_character.aiming)
            {
                Vector3 aimTarget = CameraManager.singleton.aimTargetPoint;
                aimTarget.y = transform.position.y;
                Vector3 aimDirection = (aimTarget - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, AimRotationSpeed * Time.deltaTime);
            
            }
        }
        private void Move()
        {
            

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            _character.moveSpeed = _input.move == Vector2.zero ? 0 : _character.speedAnimationMultiplier;



            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero )
            {

                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
              
                if(_character.aiming == false)
                {
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

           
        }

        private void JumpAndGravity()
        {
            if (_character.isGrounded)
            {
               

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _jumpTimeoutDelta = JumpTimeout;
                    _character.Jump();
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;


                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        //private void OnDrawGizmosSelected()
        //{
        //    Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        //    Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        //    if (Grounded) Gizmos.color = transparentGreen;
        //    else Gizmos.color = transparentRed;

        //    when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        //    Gizmos.DrawSphere(
        //        new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
        //        GroundedRadius);
        //}

       


       
    }
}