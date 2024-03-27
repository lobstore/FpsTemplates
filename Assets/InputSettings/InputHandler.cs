using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class InputHandler : MonoBehaviour, IMoveInput, IWeaponInput
{
    public UnityEvent OnJumped { get; } = new();
    public UnityEvent<bool> IsSprinting { get; } = new ();
    public UnityEvent<bool> IsShooting { get; } = new ();
    public UnityEvent<float> OnWeaponSwitch { get; } = new ();
    public UnityEvent OnWeaponDrop { get; } = new ();
    public UnityEvent OnWeaponPickUp { get; } = new ();
    public UnityEvent<bool> IsCrouching { get; } = new();
    public UnityEvent OnAmmoReload { get; }=new();

    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    
    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnReload(InputValue value)
    {
        if (value.isPressed)
        {
            OnAmmoReload.Invoke();
        }
    }
    public void OnSwitchWeapon(InputValue value)
    {
        OnWeaponSwitch.Invoke(value.Get<float>());
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            OnJumped.Invoke();
        }
    }

    public void OnDrop(InputValue value)
    {
        if (value.isPressed)
        {
            OnWeaponDrop.Invoke();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            OnWeaponPickUp.Invoke();
        }
    }
    public void OnCrouch(InputValue value)
    {
        IsCrouching.Invoke(value.isPressed);
    }

    public void OnShoot(InputValue value)
    {
        IsShooting.Invoke(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        IsSprinting.Invoke(value.isPressed);
    }


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
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