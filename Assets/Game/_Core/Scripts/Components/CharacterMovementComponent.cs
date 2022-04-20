using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the logic for character's movement and jumping
/// </summary>
public class CharacterMovementComponent : MonoBehaviour
{
    // Movement
    [Header("Movement")]
    [SerializeField, Range(10f, 20f)]
    private float speed;
    [SerializeField, Range(0.01f, 0.2f)]
    private float accelerationTime;

    // Jump
    [Header("Jump | Main")]
    [SerializeField, Range(10f, 30f)]
    private float jumpForce;
    [SerializeField, Range(0f, 1f)]
    private float variableJumpHeightFactor;
    [SerializeField, Range(1, 3)]
    private int maxNumberJumps;
    private int currentJumpCount;
    private bool isJumping;
    private bool isGrounded;
    [Header("Jump | Gravity")]
    [SerializeField, Range(1f, 10f)]
    private float jumpUpwardsGravityMultiplier;
    [SerializeField, Range(1f, 10f)]
    private float fallGravityMultiplier;
    [Header("Jump | Coyote Time")]
    [SerializeField]
    private bool useCoyoteTime;
    [SerializeField, Range(0f, 1f)]
    private float coyoteTime;
    private float coyoteTimeCounter;

    // Components
    private GroundCheckComponent groundCheckComponent;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        groundCheckComponent = GetComponent<GroundCheckComponent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        CheckGroundedState();
        AdjustGravityInAir();
    }

    /// <summary>
    /// Move character in the given direction
    /// <param name="directionValue">Applied direction value</param>
    /// </summary>
    public void Move(float directionValue)
    {
        // Move character smoothly based on input
        Vector2 targetVelocity = new Vector2(directionValue * speed, rigidbody.velocity.y);
        Vector3 dampVelocity = Vector3.zero;
        rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, targetVelocity, ref dampVelocity, accelerationTime);

        // Flip character's sprite to match horizontal input direction
        if ((spriteRenderer.flipX && directionValue > 0f) || (!spriteRenderer.flipX && directionValue < 0f))
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    /// <summary>
    /// Check if character is on ground or in air
    /// </summary>
    private void CheckGroundedState()
    {
        bool wasGrounded = isGrounded;
        isGrounded = groundCheckComponent.IsGrounded();

        // Set grounded behaviour (only when character was not grounded before)
        if (wasGrounded != isGrounded)
        {
            if (isGrounded)
            {
                isJumping = false;
                currentJumpCount = 0;
                coyoteTimeCounter = 0f;
            }
        }

        // Set in air behaviour
        if (!isGrounded)
        {
            if (!isJumping)
            {
                // Update coyote time's counter for allowing the character to jump when is not already on the ground for a short time
                if (useCoyoteTime && coyoteTimeCounter < coyoteTime)
                {
                    coyoteTimeCounter += Time.deltaTime;
                }
                else
                {
                    // Consider that the character made a jump if it's in air once coyote time ended without having jumped
                    if (currentJumpCount == 0)
                    {
                        currentJumpCount = 1;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Character's jump
    /// </summary>
    public void Jump()
    {
        // Jump (apply impulse upwards) if hasn't reached the maximum number of jumps allowed
        if (currentJumpCount < maxNumberJumps)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0f);
            rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            currentJumpCount++;
        }
    }

    /// <summary>
    /// Cut character's current jump
    /// </summary>
    public void CutJump()
    {
        // Variable height jump (apply impulse downwards to reduce current jump's impulse)
        if (isJumping && rigidbody.velocity.y > 0f)
        {
            rigidbody.AddForce(Vector2.down * rigidbody.velocity.y * (1 - variableJumpHeightFactor), ForceMode2D.Impulse);
            coyoteTimeCounter = 0f;
        }
    }

    /// <summary>
    /// Adjust character's gravity depending on its vertical velocity
    /// </summary>
    private void AdjustGravityInAir()
    {
        if (rigidbody.velocity.y < 0f)
        {
            rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.deltaTime;
        }
        else if (rigidbody.velocity.y > 0f)
        {
            rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (jumpUpwardsGravityMultiplier - 1f) * Time.deltaTime;
        }
    }
}