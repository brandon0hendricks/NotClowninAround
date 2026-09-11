using UnityEngine;

public class EnemyParent : MonoBehaviour
{
    // Enemy parent script

    public int health;
    protected int currentHealth;
    protected Animator anim;

    public bool followPlayer;
    public bool runFromPlayer;
    public bool wanderer;
    public FollowPlayer followScript;
    [HideInInspector] public EnemySpawner parentSpawner;

    [HideInInspector] public BossController boss;

    protected virtual void Start()
    {
        currentHealth = health;
        GetComponents();
    }

    protected virtual void Update()
    {
        if (currentHealth <= 0f)
            HealthCheck();

        if (followScript != null)
        {
            followScript.followPlayer = followPlayer;
            followScript.littleBitch = runFromPlayer;
            followScript.wanderer = wanderer;
        }
    }

    private void TakeDamage(int type) //0 = light, 1 = med, 2 = heavy
    {
        switch (type)
        {
            case 0: currentHealth -= 5;
                break;
            case 1: currentHealth -= 7;
                break;
            case 2: currentHealth -= 10;
                break;
            default: currentHealth -= 7;
                break;
        }
        anim.SetTrigger("Hit");
    }

    void OnTriggerEnter2D(Collider2D collider)  // When enemy is hit by a trigger, checks the collider's tag. Then runs TakeDamage based on which damage type it is.
    {
        switch (collider.tag)
        {
            case "playerDamage0": TakeDamage(0);
                SoundManager.PlaySound(SoundType.EnemyHit, 0.3f);
                break;
            case "playerDamage1": TakeDamage(1);
                SoundManager.PlaySound(SoundType.EnemyHit, 0.3f);
                break;
            case "playerDamage2": TakeDamage(2);
                SoundManager.PlaySound(SoundType.EnemyHit, 0.3f);
                break;
            default: //Debug.Log("Trigger hitting enemy is not player damage"); ;
                break;
        }
    }

    void HealthCheck()  // Triggers the death animation of the enemy
    {
        if (anim != null)
        {
            anim.SetTrigger("Death");
            if(parentSpawner != null)
                parentSpawner.isDead = true;
        }
        else
        {
            DeleteObject();
        }
    }

    void DeleteObject() // Deletes enemy (Call as event at end of death animation)
    {
        if (boss != null)
            boss.guysAlive -= 1;

        Destroy(gameObject);
    }

    private void Balloons()
    {
        SoundManager.PlaySound(SoundType.BalloonPop, 0.5f);
    }

    private void GetComponents()            // Grabs all required components
    {
        anim = GetComponent<Animator>();
    }
}
