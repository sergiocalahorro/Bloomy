using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class that contains player's character logic
/// </summary>
public class Player : BaseCharacter
{
    // Components
    private PlayerInputActions playerInputActions;

    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Setup inputs
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += OnJumpButtonPressed;
        playerInputActions.Player.Jump.canceled += OnJumpButtonReleased;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    // FixedUpdate is called every fixed framerate frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (characterMovementComponent != null)
        {
            characterMovementComponent.Move(playerInputActions.Player.Movement.ReadValue<float>());
        }
    }

    /// <summary>
    /// Player pressed button bound to jump action (jump logic)
    /// </summary>
    /// <param name="context">Input action's context</param>
    private void OnJumpButtonPressed(InputAction.CallbackContext context)
    {
        if (characterMovementComponent != null)
        {
            characterMovementComponent.Jump();
        }
    }

    /// <summary>
    /// Player released button bound to jump action (variable height jump logic)
    /// </summary>
    /// <param name="context">Input action's context</param>
    private void OnJumpButtonReleased(InputAction.CallbackContext context)
    {
        if (characterMovementComponent != null)
        {
            characterMovementComponent.CutJump();
        }
    }

    /// <summary>
    /// Handle player's destruction
    /// </summary>
    public override void HandleDestruction()
    {
        // ToDo: things happening before player is destroyed

        // ...

        base.HandleDestruction();
    }
}