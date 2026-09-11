using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PeanutThrowerController : EnemyParent
{
    // This script will be to control the peanut thrower.
    [SerializeField] private GameObject peanutPrefab;       // Prefab of the 'nut
    private Transform _throwPoint;                          // The location of the object where the spawn point is
    private Vector2 throwPoint;                             // Where the object is spawned

    [Header("Settings")]
    [SerializeField] private float arcHeight;               // How high will it go?
    [SerializeField] private float playerDistance;          // How far is the player?
    [SerializeField] private float speedMultiplier;         // How fast should it move?
    [SerializeField] private float distanceOffset;          // How far should it correct for player movement? (Seemingly, if you set this to about the player's speed variable, it hits)
    [SerializeField] private float timer;                   // Middle of the timer range
    [SerializeField] private float timerOffset;             // How far front and back the timer goes from the base timer

    private GameObject player;                              // Player object
    [HideInInspector] public GameObject peanut;             // Peanut object (After instantiation)
    private Vector2 targetPos;                              // Target (duh)
    private SpriteRenderer sprite;                          // Sprite renderer of the enemy
    private float useableTimer;
    private bool canSpawn = true;
    private bool throwingNut = false;

    private bool touchingBound = false;

    private bool followSetting;
    private bool runSetting;

    private bool facingLeft;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        GetComponents();        //Grabs components
    }

    protected override void Update()
    {
        base.Update();
        UpdateVariables();
        FlipController();
        if (playerRelativePos().magnitude < playerDistance && peanut == null && canSpawn)   //If player is within distance and there's no peanut
        {
            canSpawn = false;
            base.anim.SetTrigger("ArcShot");
        }
        if (touchingBound)
            base.anim.SetFloat("X_Velocity", 0f);
        else
            base.anim.SetFloat("X_Velocity", Mathf.Abs(rb.GetPointVelocity(rb.position).x));
        //Debug.Log(currentHealth);
    }

    private void GetComponents()        // Grabs the components
    {
        player = GameObject.FindGameObjectWithTag("Player");        // Finds player
        _throwPoint = transform.GetChild(0);                        // Finds the throw point for the peanut
        sprite = GetComponent<SpriteRenderer>();                    // Grabs the sprite renderer component
        rb = GetComponent<Rigidbody2D>();                           // Grabs the Rigidbody2D component
    }

    private void UpdateVariables()
    {
        targetPos = player.transform.position;                                  // Sets the target position to the player
        throwPoint = _throwPoint.position;                                      // Sets throwPoint to the pos of throw point
        distanceOffset = player.GetComponent<Rigidbody2D>().linearVelocity.x;   // Sets the distance offset to the velocity of the player. Roughly works out
    }

    private void Flip()         // If player to the left, flip the sprite. Otherwise, don't flip.
    {
        Vector3 localScale = sprite.transform.localScale;
        localScale.x *= -1f;
        sprite.transform.localScale = localScale;
    }

    public void SpawnTheNut()
    {
        StartCoroutine(SpawnPeanut());
        SoundManager.PlayAudioObject(SoundType.PeanutToss, transform.position, 0.5f);
    }

    public void PauseMovement()         // Notes what the movement variables were, saves them, then sets them to false
    {
        StartCoroutine(SaveMovement());
        throwingNut = true;
    }

    public void ResumeMovement()        // Restores movement variables to previous state
    {
        base.followPlayer = true;
        base.runFromPlayer = true;
        throwingNut = false;
    }

    public void OnPeanutDead()
    {
        peanut = null;
        StartCoroutine(ThrowCooldown());
    }

    private IEnumerator SpawnPeanut()
    {
        // This will instantiate a peanut and arc it. 
        // The math is beyond me, so this is it:
        // g = gravity. h = desired arc height. VerticalV = required velocity to hit height, calculated with kinetic energy formula. 
        // timeUp = time to reach arc height, verticalV / g. timeDown = time to fall to target, calculated with kinematic formula. totalTime = both times added.
        // Horizontal velocity = distance / time. Then peanut velocity = both velocities. Or some shit like that. It's above my head TBH. I need to go learn some math.
        float g = Mathf.Abs(Physics2D.gravity.y * speedMultiplier);
        float h = Mathf.Max(arcHeight, targetPos.y - throwPoint.y + 0.5f);
        float verticalV = Mathf.Sqrt(2 * g * h);
        float timeUp = verticalV / g;
        float timeDown = Mathf.Sqrt(2 * (h + (throwPoint.y - targetPos.y)) / g);
        float totalTime = (timeUp + timeDown);
        float horizontalV = ((targetPos.x - throwPoint.x) / totalTime) + distanceOffset;
        peanut = Instantiate(peanutPrefab, throwPoint, Quaternion.identity);
        Peanut peanutScript = peanut.GetComponent<Peanut>();
        peanutScript.parent = this;
        peanut.GetComponent<Rigidbody2D>().gravityScale = speedMultiplier;
        peanut.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(horizontalV, verticalV);
        yield return new WaitForSeconds(0.01f);
    }

    private IEnumerator SaveMovement()
    {
        runSetting = base.runFromPlayer;
        followSetting = base.followPlayer;
        base.followPlayer = false;
        base.runFromPlayer = false;
        Debug.Log("Saving Enemy Movement!");
        yield return new WaitForSeconds(0.0001f);
    }

    private IEnumerator ThrowCooldown()
    {
        canSpawn = false;
        useableTimer = Random.Range(timer - timerOffset, timer + timerOffset);
        yield return new WaitForSeconds(useableTimer);
        canSpawn = true;
    }

    private Vector2 playerRelativePos()         // Returns the location of the player relative to the thrower
    {
        return targetPos - throwPoint;
    }

    private void FlipController()
    {
        if ((!throwingNut && rb.linearVelocity.x < 0.1f && !facingLeft) || (throwingNut && targetPos.x < transform.position.x && !facingLeft))
        {
            facingLeft = true;
            Flip();
        }
        if ((!throwingNut && rb.linearVelocity.x > 0.1f && facingLeft) || (throwingNut && targetPos.x > transform.position.x && facingLeft))
        {
            facingLeft = false;
            Flip();
        }
    }
    private void OnCollisionStay2D(Collision2D col)
    {
        touchingBound = (col.gameObject.CompareTag("EnemyBounds"));
    }
}
