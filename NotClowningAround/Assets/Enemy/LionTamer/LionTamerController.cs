using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class LionTamerController : EnemyParent
{
    // Movement saving bools
    private bool _follower;
    private bool _coward;

    // Components
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private GameObject player;

    // Attack related variables
    private float _usedTimer;
    [SerializeField] private float attackTimer;
    [SerializeField] private float attackTimerOffset;

    // Other
    private bool facingLeft = false;
    private bool canAttack = true;
    [SerializeField] private float attackDistance;
    private bool touchingBound = false;
    private bool attacking = false;

    protected override void Start()
    {
        base.Start();
        GetComponents();
    }

     protected override void Update()
    {
        base.Update();
        FlipController();
        AttackCheck();
        if (touchingBound)
            base.anim.SetFloat("Walk", 0f);
        else
            base.anim.SetFloat("Walk", Mathf.Abs(rb.GetPointVelocity(rb.position).x));
    }

    private void GetComponents()            // Gathers all necessary components
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void AttackCheck()
    {
        if (canAttack && PlayerRelativeDistance() < attackDistance)
        {
            Attack();
        }
    }

    private void Attack()
    {
        StartCoroutine(Timer());
        base.anim.SetTrigger("Attack");
    }

    private void WhipSound()
    {
        SoundManager.PlaySound(SoundType.WhipCrack, 0.3f);
    }

    private void AttackBool()
    {
        attacking = true;
    }

    private void ResetAttackBool()
    {
        attacking = false;
    }

    private void PauseMovement()             // Grabs movement from parent and saves the values
    {
        StartCoroutine(SaveMovement());
    }
    private void ResumeMovement()            // Restores previous movement values
    {
        base.followPlayer = true;
        base.runFromPlayer = true;
    }

    private IEnumerator SaveMovement()
    {
        _follower = base.followPlayer;
        _coward = base.runFromPlayer;
        base.followPlayer = false;
        base.runFromPlayer = false;
        yield return null;
    }

    private void Flip()         // If player to the left, flip the sprite. Otherwise, don't flip.
    {
        Vector3 localScale = sprite.transform.localScale;
        localScale.x *= -1f;
        sprite.transform.localScale = localScale;
    }

    private void FlipController()
    {
        if ((rb.linearVelocity.x < 0.3f && !facingLeft && base.followScript.state != FollowPlayer.enemyState.Idle) || (attacking && player.transform.position.x < transform.position.x && !facingLeft))
        {
            facingLeft = true;
            Flip();
        }
        if ((rb.linearVelocity.x > 0.3f && facingLeft) || (attacking && player.transform.position.x > transform.position.x && facingLeft))
        {
            facingLeft = false;
            Flip();
        }
    }

    private IEnumerator Timer()
    {
        canAttack = false;
        _usedTimer = Random.Range(attackTimer - attackTimerOffset, attackTimer + attackTimerOffset);
        yield return new WaitForSeconds(_usedTimer);
        canAttack = true;
    }

    private float PlayerRelativeDistance()
    {
        return (player.transform.position - transform.position).magnitude;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        touchingBound = (col.gameObject.CompareTag("EnemyBounds"));
    }
}
