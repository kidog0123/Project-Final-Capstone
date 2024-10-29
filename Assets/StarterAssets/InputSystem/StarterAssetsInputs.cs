using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool walk;
		public bool aim;
		public bool shoot;
		public bool reload;
		public float switchWeaponn;
		public bool holsterWeapon;
		public bool pickUpItem;
		public bool inventory;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnWalk(InputValue value)
		{
			WalkInput(value.isPressed);
		}
		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}

		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}
#endif
        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }

		public void OnSwitchWeapon(InputValue value)
		{
			SwitchWeaponInput(value.Get<float>());
		}

        public void OnHolsterWeapon(InputValue value)
        {
            HolsterWeaponInput(value.isPressed);
        }
		public void OnPickUpItem(InputValue value)
        {
            PickUpItemInput(value.isPressed);
        }
		public void OnInventory(InputValue value)
        {
            InventoryInput(value.isPressed);
        }
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void WalkInput(bool newWalkState)
		{
			walk = newWalkState;
		}
		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}
		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }
		public void SwitchWeaponInput(float newSwitchWeaponState)
        {
            switchWeaponn = newSwitchWeaponState;
        }
        public void HolsterWeaponInput(bool newHolsterWeaponState)
        {
            holsterWeapon = newHolsterWeaponState;
        }
		public void PickUpItemInput(bool newPickUpItemState)
        {
            pickUpItem = newPickUpItemState;
        }
		public void InventoryInput(bool newInventoryState)
        {
            inventory = newInventoryState;
        }
        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}