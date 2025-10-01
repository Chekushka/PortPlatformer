using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Public properties to be read by other scripts, like PlayerController
    public Vector2 MoveInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool JumpPressed { get; private set; }

    // Private reference to the Player Input component
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found.");
            return;
        }

        playerInput.actions["Move"].performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => MoveInput = Vector2.zero;
        playerInput.actions["Jump"].performed += ctx => JumpPressed = true;
        playerInput.actions["Sprint"].performed += ctx => IsSprinting = true;
        playerInput.actions["Sprint"].canceled += ctx => IsSprinting = false;
    }
    
    // This is the new method to call from PlayerController after a jump
    public void ResetJumpInput()
    {
        JumpPressed = false;
    }
    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks when the object is destroyed
        if (playerInput != null)
        {
            playerInput.actions["Move"].performed -= OnMovePerformed;
            playerInput.actions["Move"].canceled -= OnMoveCanceled;
            playerInput.actions["Jump"].performed -= OnJumpPerformed;
            playerInput.actions["Sprint"].performed -= OnSprintPerformed;
            playerInput.actions["Sprint"].canceled -= OnSprintCanceled;
        }
    }

    // Event handler methods
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // The jump action is a one-time event, so we only need 'performed'
        JumpPressed = true;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        IsSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        IsSprinting = false;
    }
}