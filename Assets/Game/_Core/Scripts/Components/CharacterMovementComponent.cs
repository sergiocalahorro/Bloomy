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
    private bool isFacingRight;

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

    // Jump
    [Header("Dash | Main")]
    [SerializeField, Range(5f, 30f)]
    private float dashSpeed;
    [SerializeField, Range(0.1f, 1f)]
    private float dashDuration;
    [SerializeField, Range(0.1f, 1f)]
    private float dashCooldown;
    private Vector2 dashDirection;
    private bool isDashing;
    private bool canDash;
    private bool startedDashMidAir;

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

        isFacingRight = spriteRenderer.flipX;
    }

    // Start is called before the first frame update
    private void Start()
    {
        canDash = true;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        CheckGroundedState();
        AdjustGravityInAir();
        Dash();
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
        if ((!isFacingRight && directionValue > 0f) || (isFacingRight && directionValue < 0f))
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            isFacingRight = spriteRenderer.flipX;
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

                canDash = true;
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

            // Allow character to perform a dash if didn't previously dash in mid-air
            if (!startedDashMidAir)
            {
                if (!isDashing && !canDash)
                {
                    canDash = true;
                }
            }
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

    /// <summary>
    /// Start character's dash
    /// </summary>
    /// <param name="directionX"> Horizontal direction value </param>
    public void StartDash(float directionX)
    {
        if (canDash && !isDashing)
        {
            isDashing = true;
            canDash = false;

            startedDashMidAir = !isGrounded;

            dashDirection = new Vector2(directionX, 0f);

            // Dash in the direction the character is facing if the given direction is zero
            if (dashDirection == Vector2.zero)
            {
                if (isFacingRight)
                {
                    dashDirection = new Vector2(1f, 0f);
                }
                else
                {
                    dashDirection = new Vector2(-1f, 0f);
                }
            }

            // Stop dashing after a certain amount of time
            StartCoroutine(StopDash());
        }
    }

    /// <summary>
    /// Character's dash forward
    /// </summary>
    private void Dash()
    {
        if (isDashing)
        {
            rigidbody.velocity = dashDirection.normalized * dashSpeed;
        }
    }

    /// <summary>
    /// Stop character's dash
    /// </summary>
    public void InterruptDash()
    {
        if (isDashing)
        {
            isDashing = false;
            StartCoroutine(EnableDash());
        }
    }

    /// <summary>
    /// Stop character's dash after a certain delay
    /// </summary>
    /// <returns> Time in seconds to stop dashing </returns>
    private IEnumerator StopDash()
    {
        yield return new WaitForSeconds(dashDuration);
        InterruptDash();
    }

    /// <summary>
    /// Enable character's ability to dash after the cooldown is consumed
    /// </summary>
    /// <returns> Time in seconds to enable dash </returns>
    private IEnumerator EnableDash()
    {
        yield return new WaitForSeconds(dashCooldown);

        if (isGrounded)
        {
            if (!isDashing && !canDash)
            {
                canDash = true;
            }
        }
    }
}