using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Animation
    private Animator anim;

    private Vector2 movementInput;

    [Header("Input")]
    public InputActionReference movement;
    public InputActionReference jump;
    public InputActionReference attack;
    public InputActionReference dash;

    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Attack")]
    [SerializeField] private AttackHitbox attackHitbox;

    [Header("Better Jump Settings")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpGravity = 2f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool canAttack = true;
    private bool isAttacking = false;

    private bool canDash = true;
    private bool isDashing = false;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeedMultiplier = 1.5f;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashResetTimer;

    private bool isGrounded;
    private bool touchingWall;

    [SerializeField] private BossFightManager fightManager;

    private bool touchingGrass;

    private AudioObject dashSFX;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!anim.GetBool("PlayerDeath"))
        {
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetFloat("Y_velocity", rb.linearVelocity.y);
            anim.SetBool("Walking", Mathf.Abs(rb.linearVelocity.x) > 0.3f);

            // Attack cooldown variables
            if (attack.action.IsPressed() && canAttack && isGrounded)
            {
                Attack();
                canAttack = false; // disable attacking immediately
                StartCoroutine(AttackCooldown()); // start 2 second cooldown timer
            }

            // Dash input
            if (Keyboard.current.eKey.isPressed && canDash && !isAttacking)
            {
                StartCoroutine(Dash());
            }

            // Prevent movement during attack or dash
            if (!isAttacking && !isDashing && DialogueBox.inDialogue == false)
            {
                 movementInput = movement.action.ReadValue<Vector2>();
            }
            else
            {
                movementInput = Vector2.zero;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

            // Flip player based on direction
            if (movementInput.x > 0f)
                transform.localScale = new Vector3(1f, 1f, 1f);
            else if (movementInput.x < 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f);

            // Ground check
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );

            if (isGrounded)
                coyoteTimeCounter = coyoteTime;
            else
                coyoteTimeCounter -= Time.deltaTime;

            // Jump
            if (!isAttacking && !isDashing)
            {
                if (jump.action.IsPressed() && coyoteTimeCounter > 0f && rb.linearVelocity.y <= 0.3f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    SoundManager.PlaySound(SoundType.PlayerJump, 0.5f);
                    coyoteTimeCounter = 0f;
                }
            }
        }
    }

    private void Attack()
    {
        anim.SetTrigger("attack");
        isAttacking = true;
        SoundManager.PlaySound(SoundType.HammerAttack, 0.25f);

        // Stop movement instantly when attacking
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // Assign damage to hitbox
        if (attackHitbox != null)
        {
            attackHitbox.SetDamage2();
        }
    }

  
    public void AttackReset()
    {
        anim.ResetTrigger("attack");
        isAttacking = false; // allow movement again
      
    }

    
    private IEnumerator AttackCooldown() // attack cooldown timing
    {
        yield return new WaitForSeconds(.5f); // wait 2 seconds
        canAttack = true; // re-enable attacking after cooldown
    }

    void FixedUpdate()
    {
        if (!anim.GetBool("PlayerDeath"))
        {
            if (!isAttacking && !isDashing)
            {
                rb.linearVelocity = new Vector2(
                    movementInput.x * baseSpeed,
                    rb.linearVelocity.y
                );
            }

            // Better jump gravity
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector2.up *
                                     Physics2D.gravity.y *
                                     (fallMultiplier - 1) *
                                     Time.fixedDeltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !jump.action.IsPressed())
            {
                rb.linearVelocity += Vector2.up *
                                     Physics2D.gravity.y *
                                     (lowJumpMultiplier - 1) *
                                     Time.fixedDeltaTime;
            }

            if (rb.linearVelocity.y > 0f && jump.action.IsPressed())
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * jumpGravity * Time.deltaTime;
            }
        }
        else if (anim.GetBool("PlayerDeath"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void DashBool()
    {
        anim.SetBool("dashPause", true);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float timer = dashTime;

        anim.SetBool("dash", true);
        dashSFX = SoundManager.PlayAudioObject(SoundType.Dash, transform.position, 0.5f);

        while (timer > 0f && !touchingWall)
        {
            rb.linearVelocity = new Vector2(dashSpeedMultiplier * transform.localScale.x, 0f);
            timer -= Time.deltaTime;
            yield return null;
        }

        SoundManager.KillSoundEarly(dashSFX);

        isDashing = false;
        anim.SetBool("dashPause", false);
        anim.SetBool("dash", false);
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashResetTimer);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
            touchingWall = true;
        else if (col.gameObject.CompareTag("GroundGrass"))
            SoundManager.PlaySound(SoundType.PlayerLandGrass, 0.25f);
        else if (col.gameObject.CompareTag("GroundTarp"))
            SoundManager.PlaySound(SoundType.PlayerLandTarp, 0.25f);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
            touchingWall = false;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("GroundGrass"))
            touchingGrass = true;
        else if (col.gameObject.CompareTag("GroundTarp"))
            touchingGrass = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ExitTrigger"))
        {
            fightManager.fightBegin = true;
        }
    }

    private void StepSound()
    {
        if (touchingGrass)
            SoundManager.PlaySound(SoundType.StepsGrass, 0.25f);
        else if (!touchingGrass)
            SoundManager.PlaySound(SoundType.StepsTarp, 0.25f);
    }
}