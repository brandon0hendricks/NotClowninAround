using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public float Health, MaxHealth;

    [SerializeField]
    private RectTransform HealthBar;

    private float originalWidth;
    private float originalHeight;

    void Awake()
    {
        // Store the size set in inspector
        originalWidth = HealthBar.sizeDelta.x;
        originalHeight = HealthBar.sizeDelta.y;
    }

    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
        SetHealth(maxHealth);
    }

    public void SetHealth(float health)
    {
        Health = Mathf.Clamp(health, 0, MaxHealth);

        if (MaxHealth <= 0) return;

        float newWidth = (Health / MaxHealth) * originalWidth;

        HealthBar.sizeDelta = new Vector2(newWidth, originalHeight);
    }
}