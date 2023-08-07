using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    private float moveDirectionX;
    private bool isGamePaused = false;

    private bool isFacingRight = true;

    [Tooltip("Position checks")]
    public LayerMask listGroundLayers;
    public Transform groundCheck;
    public float groundCheckRadius;

    private bool isGrounded;
    public Animator animator;

    [Tooltip("Running system")]
    public float moveSpeed;

    [Header("Jump system"), ReadOnlyInspector]
    public int jumpCount = 0;
    public int maxJumpCount;
    public float jumpForce;

    public float fallThreshold = -15f;

    private bool isLandingFast = false;

    [Header("Misc")]
    public CameraShakeEventChannelSO onLandingFastSO;
    public ShakeTypeVariable landingFastShakeInfo;
    public BoolEventChannelSO onTogglePauseEvent;
    private Vector2 currentVelocity;
    private float maxYVelocity;

    [Header("Debug")]
    public VectorEventChannel onDebugTeleportEvent;

    private void OnEnable() {
        onTogglePauseEvent.OnEventRaised += OnPauseEvent;
        onDebugTeleportEvent.OnEventRaised += OnDebugTeleport;
    }

    private void OnPauseEvent(bool value)
    {
        isGamePaused = value;
    }

    private void Awake()
    {
        // The jump high cannot be higher that +10% of normal jumpforce
        maxYVelocity = (jumpForce * 0.1f) + jumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGamePaused)
        {
            return;
        }
        moveDirectionX = Input.GetAxis("Horizontal");

        if (isGrounded && !Input.GetButton("Jump"))
        {
            jumpCount = 0;
        }

        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount < maxJumpCount))
        {
            Jump(false);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            Jump(true);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && !isGrounded)
        {
            isLandingFast = true;
            rb.velocity = new Vector2(rb.velocity.x, -jumpForce);
        }

        if (isLandingFast && isGrounded)
        {
            LandingImpact();
        }

        Flip();
    }

    private void FixedUpdate()
    {
        Animations();
        LimitSpeed();
        isGrounded = IsGrounded();

        Move();
    }

    private void LimitSpeed()
    {
        currentVelocity = rb.velocity;
        currentVelocity.y = Mathf.Clamp(currentVelocity.y, rb.velocity.y, maxYVelocity);
        currentVelocity.x = Mathf.Clamp(currentVelocity.x, -moveSpeed, moveSpeed);

        rb.velocity = currentVelocity;
    }

    private void Move()
    {
        rb.velocity = new Vector2((moveDirectionX * moveSpeed), rb.velocity.y);
    }

    private void Animations()
    {
        animator.SetFloat("MoveDirectionX", Mathf.Abs(moveDirectionX));
        animator.SetFloat("MoveDirectionY", rb.velocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void Flip()
    {
        if (moveDirectionX > 0 && !isFacingRight || moveDirectionX < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void Jump(bool shortJump = false)
    {
        float jumpPower = (shortJump ? rb.velocity.y * 0.5f : jumpForce);
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);

        if (!shortJump)
        {
            jumpCount = jumpCount + 1;
        }

        if (jumpCount > 1)
        {
            animator.SetTrigger("DoubleJump");
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, listGroundLayers);
    }

    public bool IsFalling()
    {
        return rb.velocity.y < fallThreshold;
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void ToggleState(bool state)
    {
        enabled = !state;
    }

    private void LandingImpact()
    {
        isLandingFast = false;
        onLandingFastSO.Raise(landingFastShakeInfo);
    }

    private void OnDebugTeleport(Vector3 newPos)
    {
        transform.position = newPos;
    }

    private void OnDisable() {
        onTogglePauseEvent.OnEventRaised -= OnPauseEvent;
        onDebugTeleportEvent.OnEventRaised -= OnDebugTeleport;
    }
}
