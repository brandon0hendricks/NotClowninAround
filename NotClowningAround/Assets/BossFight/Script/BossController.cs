using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private enum bossState { Idle, Follow, Attack, SpawnGuys, Levitate, Dead }
    private bossState state = bossState.Idle;
    [HideInInspector] public Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool facingLeft = false;

    [SerializeField] private GameObject spawnPoint1;
    [SerializeField] private GameObject spawnPoint2;

    [SerializeField] private GameObject fader;


    [SerializeField] private GameObject peanutGuy;
    [SerializeField] private GameObject whipGuy;

    [SerializeField] private float floatUpSpeed;
    private CapsuleCollider2D col;

    [SerializeField] private Transform floatPoint;

    private bool attacking;
    private bool exitFloat;
    private float attackCooldown;
    private float attackCooldownBase = 10f;

    public float bossHealthBase = 100f;
    public float bossHealthCurrent;

    [HideInInspector] public int guysAlive = 0;

    [SerializeField] private BossFightManager manager;
    [SerializeField] private FollowPlayer followScript;

    [SerializeField] DialogueBox dialogueBox; //dialogue box reference
    [SerializeField] DialogueString dialogueString; //dialogue string reference
    [SerializeField] GameObject creditFades;


    private void Start()
    {
        GetComponents();
        followScript.enabled = false;
        bossHealthCurrent = bossHealthBase;
    }

    private void Update()
    {
        FlipController();
        if (anim.GetBool("Float") && !exitFloat)
        {
            rb.linearVelocityY = floatUpSpeed;
            col.isTrigger = true;
        }
        if (!anim.GetBool("Float"))
        {
            col.isTrigger = false;
        }
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;
        if (attacking)
        {
            followScript.enabled = false;
        }
        else if (!attacking)
            followScript.enabled = true;
        //Debug.Log(state + " < State : Guys left > " + guysAlive);
        anim.SetFloat("Walk", Mathf.Abs(rb.linearVelocityX));

        if (bossHealthCurrent <= 0f && state != bossState.Dead)
        {
            state = bossState.Dead;
            SoundManager.PlaySound(SoundType.BossDeath, 0.3f);
        }



        if (manager.fightBegin)
        {
            
            if (!followScript.enabled)
            {
                followScript.enabled = true;
                followScript.followPlayer = true;
            }
            switch (state)
            {
                case bossState.Idle:
                    if (!attacking && attackCooldown <= 0f && guysAlive < 2)
                    {
                        if (guysAlive == 1 && PlayerDistance() > 2f)
                        {
                            state = bossState.Follow;
                        }
                        if (PlayerDistance() <= 2f && guysAlive == 1)
                        {
                            state = bossState.Attack;
                        }
                        else if (guysAlive == 0)
                        {
                            state = bossState.SpawnGuys;
                        }
                    }
                    if (guysAlive == 2)
                    {
                        state = bossState.Levitate;
                    }
                    break;
                case bossState.Follow:
                    if (attackCooldown > 0f)
                    {
                        followScript.followPlayer = true;
                    }
                    else if (attackCooldown <= 0f)
                    {
                        state = bossState.Idle;
                        followScript.followPlayer = false;
                    }
                    break;
                case bossState.Attack:
                    AttackBegin();
                    break;
                case bossState.SpawnGuys:
                    if (!attacking && attackCooldown <= 0f && !anim.GetBool("SpawnGuys"))
                    {
                        attacking = true;
                        anim.SetBool("SpawnGuys", true);
                        attackCooldown = attackCooldownBase;
                    }
                    break;
                case bossState.Levitate:
                    if (guysAlive == 2)
                    {
                        anim.SetBool("Float", true);
                        followScript.followPlayer = false;
                        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                        if (transform.position.y != floatPoint.position.y)
                        {
                            rb.gravityScale = 0f;
                            Vector3 newPos = new Vector3(transform.position.x, floatPoint.position.y, transform.position.z);
                            transform.position = Vector3.MoveTowards(transform.position, newPos, floatUpSpeed * Time.deltaTime);
                        }
                    }
                    else if (guysAlive < 2)
                    {
                        anim.SetBool("Float", false);
                        rb.gravityScale = 1f;
                        followScript.followPlayer = true;
                        state = bossState.Idle;
                        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                    break;
                case bossState.Dead:
                    anim.SetBool("Dead", true);

                    break;
                default:
                    Debug.Log("Boss state error");
                    break;

            }
        }
    }

    private void GetComponents()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FlipController()
    {
        if ((rb.linearVelocity.x < 0.3f && !facingLeft && followScript.state != FollowPlayer.enemyState.Idle) || (attacking && followScript.player.transform.position.x < transform.position.x && !facingLeft))
        {
            facingLeft = true;
            Flip();
        }
        if ((rb.linearVelocity.x > 0.3f && facingLeft) || (attacking && followScript.player.transform.position.x > transform.position.x && facingLeft))
        {
            facingLeft = false;
            Flip();
        }
    }

    private void Flip()         // If player to the left, flip the sprite. Otherwise, don't flip.
    {
        Vector3 localScale = sprite.transform.localScale;
        localScale.x *= -1f;
        sprite.transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("ExitTrigger"))
        {
            exitFloat = true;
            anim.SetBool("Float", false);
        }
        if (collider.gameObject.CompareTag("playerDamage0") && !anim.GetBool("Float"))
        {
            bossHealthCurrent -= 8f;
            SoundManager.PlaySound(SoundType.BossHit, 0.3f);
        }
    }

    private void SpawnTheDudes()
    {
        StartCoroutine(SpawnGuys());
        SoundManager.PlaySound(SoundType.BossTaunt, 0.25f);
        anim.SetBool("SpawnGuys", false);
        state = bossState.Idle;
    }

    private IEnumerator SpawnGuys()
    {
        Debug.Log("Spawning dudes");
        int guy1 = Random.Range(0, 2);
        int guy2 = Random.Range(0, 2);

        GameObject enemy1 = Instantiate(guy1 == 0 ? peanutGuy : whipGuy, spawnPoint1.transform.position, Quaternion.identity);
        EnemyParent enemy1Script = enemy1.GetComponent<EnemyParent>();
        enemy1Script.boss = this;
        guysAlive++;

        GameObject enemy2 = Instantiate(guy2 == 0 ? peanutGuy : whipGuy, spawnPoint2.transform.position, Quaternion.identity);
        EnemyParent enemy2Script = enemy2.GetComponent<EnemyParent>();
        enemy2Script.boss = this;
        guysAlive++;
        attacking = false;
        yield return new WaitForEndOfFrame();
    }

    private void Death()
    {
        creditFades.SetActive(true);
        if (DialogueBox.inDialogue == false)
        {
            dialogueBox.activateDialogue(dialogueString.DialogueLines); //send dialogue
        }
        fader.SetActive(true);

    }

    private void AttackBegin()
    {
        attacking = true;
        anim.SetBool("Attack", true);
        attackCooldown = attackCooldownBase;
    }

    private void AttackNoise()
    {
        SoundManager.PlaySound(SoundType.BossAttack, 0.3f);
    }

    private void AttackEnd()
    {
        anim.SetBool("Attack", false);
        attacking = false;
        state = bossState.Idle;
    }
    private float PlayerDistance()
    {
        return Vector2.Distance(transform.position, followScript.player.transform.position);
    }

}
