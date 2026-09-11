using System.Collections;
using UnityEngine;
using UnityEngine.Apple;

public class FollowPlayer : MonoBehaviour
{
    // This is a script any script can use as a way to follow and attack the player. 

    // State machine states
    public enum enemyState { Idle, Wander, Flee, Chase }
    public enemyState state = enemyState.Idle;

    [HideInInspector] public GameObject bound1;
    [HideInInspector] public GameObject bound2;

    [Header("Player Following Settings")]
    [SerializeField] private float followDistance;                    // How far does the player get before I follow?
    public float stopFollowDistance;                // How close do I get before I stop following?
    [SerializeField] private float speed;                             // How fast do I go?
    private float usedSpeed;
    [SerializeField] private float detectionRange;                    // How close does the player get before I chase?
    [HideInInspector] public bool littleBitch;                        // Do I run away if the player is too close?
    [SerializeField] private float fleeDistance;    // How close can the player get before I run away like a LITTLE BITCH?
    [SerializeField] private float stopFleeDistance; // How far away will I run? (should be like, flee + 1)

    public bool followPlayer;     // Bool that controls if the enemy should be following the player or not.

    [HideInInspector] public bool wanderer;
    private bool onTheMove = false;

    [SerializeField] private float howCloseToTarget;

    [SerializeField] private float yTolerance;

    [HideInInspector] public GameObject player;
    private Rigidbody2D rb;
    [HideInInspector] public float realSpeed;
    private Vector2 lastPosition;


    private void Awake()            // Grabs components (Only 2, no need to make a whole new method for it)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        usedSpeed = Random.Range(speed - 0.75f, speed + 1.25f);
    }

    private void Start()
    {
        lastPosition = rb.position;
    }
    private void Update()           // If player is within chase distance and is told to chase, chase
    {
        StartCoroutine(GetRealSpeed());
        switch (state)
        {
            case enemyState.Idle:                           // When idle, movement is null. 
                if (playerDistance() > followDistance && followPlayer)
                    state = enemyState.Chase;
                if (playerDistance() < fleeDistance && littleBitch)
                    state = enemyState.Flee;
                if (playerDistance() > detectionRange && wanderer)
                    state = enemyState.Wander;
                if (!PlayerMatchingY() && wanderer)
                    state = enemyState.Wander;
                break;

            case enemyState.Flee:                           // When flee, enemy will run away until outside of stop fleeing distance. Stop flee must be > flee.
                RunAway();
                if (playerDistance() >= stopFleeDistance || !littleBitch)
                    state = enemyState.Idle;
                if (!PlayerMatchingY() && wanderer)
                    state = enemyState.Wander;
                break;

            case enemyState.Chase:
                GiveChase();
                if (playerDistance() <= stopFollowDistance || !followPlayer)
                    state = enemyState.Idle;
                if (playerDistance() <= fleeDistance)
                    state = enemyState.Flee;
                if (!PlayerMatchingY() && wanderer)
                    state = enemyState.Wander;
                break;

            case enemyState.Wander:
                Wander();
                if (PlayerMatchingY())
                    state = enemyState.Idle;
                break;

        }
    }

    private void RunAway()           // Will run away from player
    {
        if (player.transform.position.x > transform.position.x)
        {
            rb.linearVelocity = new Vector2(-usedSpeed, rb.linearVelocity.y);
        }
        if (player.transform.position.x < transform.position.x)
        {
            rb.linearVelocity = new Vector2(usedSpeed, rb.linearVelocity.y);
        }
    }

    private void GiveChase()        // If player isn't at the follow distance, follow the player. No jumping.
    {
        if (player.transform.position.x > transform.position.x)
        {
            rb.linearVelocity = new Vector2(usedSpeed, rb.linearVelocity.y);
        }
        if (player.transform.position.x < transform.position.x)
        {
            rb.linearVelocity = new Vector2(-usedSpeed, rb.linearVelocity.y);
        }
    }

    private void Wander()
    {
        if (!onTheMove)
        {
            StartCoroutine(Wandering());
        }
    }

    private float playerDistance()
    {
        return (transform.position - player.transform.position).magnitude;
    }

    private bool PlayerMatchingY()
    {
        return Mathf.Abs(player.transform.position.y - transform.position.y) < yTolerance;
    }

    private Vector2 RandomWaypoint()
    {
        float vecx = Random.Range(bound1.transform.position.x + 1f, bound2.transform.position.x - 1f);
        float vecy = Random.Range(bound1.transform.position.y, bound2.transform.position.y);
        return new Vector2(vecx, vecy);
    }

    private IEnumerator Wandering()
    {
        while (state == enemyState.Wander)
        {
            Vector2 target = RandomWaypoint();;

            while (Vector2.Distance(transform.position, target) > howCloseToTarget)
            {
                onTheMove = true;

                if (PlayerMatchingY())
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    onTheMove = false;
                    yield break;
                }

                if (target.x > transform.position.x)
                {
                    rb.linearVelocity = new Vector2(usedSpeed, rb.linearVelocity.y);
                }
                if (target.x < transform.position.x)
                {
                    rb.linearVelocity = new Vector2(-usedSpeed, rb.linearVelocity.y);
                }
                yield return null;
            }

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            float timer = 5f;
            while (timer > 0f)
            {
                if (PlayerMatchingY())
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    onTheMove = false;
                    yield break;
                }

                timer -= Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator GetRealSpeed()
    {
        Vector2 currentPosition = rb.position;
        realSpeed = (currentPosition - lastPosition).magnitude / Time.deltaTime;
        lastPosition = currentPosition;
        yield return null;
    }
}
