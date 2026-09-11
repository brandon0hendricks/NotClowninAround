using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Player's Health

    [SerializeField] private float maxHealth;
    [HideInInspector] public float currentHealth;
    public GameObject deathScreen;

    private float savedHealth;

    private Animator anim;

    
    [SerializeField] private Healthbar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();

        // ADDED: Initialize healthbar
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);   
            healthBar.SetHealth(currentHealth);  
        }
    }

    void Update()
    {
        HealthCheck();
        //HealthSave();

        
        if (healthBar != null) 
        {
            healthBar.SetHealth(currentHealth); 
        }
    }

    private void HealthCheck()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0f)
        {
            anim.SetBool("PlayerDeath", true);
        }
    }

    void DeathNoise()
    {
        SoundManager.PlaySound(SoundType.PlayerDeath, 0.5f);
    }

    /*private void HealthSave()
    {
        if (saveScript.saving)
        {
            savedHealth = currentHealth;
        }
        else if (saveScript.loading)
        {
            currentHealth = savedHealth;
        }
    }*/

    public void TurnOnDeath()
    {
        deathScreen.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("EnemyDamage"))
        {
            currentHealth -= 5f;
            HitSFX();
        }
        if (col.gameObject.CompareTag("EnemyDamage1"))
        {
            currentHealth -= 10f;
            SoundManager.PlaySound(SoundType.WhipHit, 0.5f);
            HitSFX();
        }
        if (col.gameObject.CompareTag("HealthPickup"))
        {
            currentHealth = maxHealth;
            Destroy(col.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("DamageWall"))
        {
            currentHealth -= 0.1f;
        }
    }

    void HitSFX()
    {
        if (currentHealth > 0f)
            SoundManager.PlaySound(SoundType.PlayerHit, 0.5f);
    }
}
