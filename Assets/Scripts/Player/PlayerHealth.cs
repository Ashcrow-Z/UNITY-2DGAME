using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;

    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;

    public int CurrentHealth { get; private set; }
    public bool IsInvincible { get; private set; }

    public event Action<int> OnHealthChanged;
    public event Action OnPlayerDied;

    private SpriteRenderer spriteRenderer;
    private Vector2 respawnPosition;
    private HealthBar healthBar;

    void Awake()
    {
        CurrentHealth = maxHealth;
        respawnPosition = transform.position;
    }

    SpriteRenderer GetVisualSpriteRenderer()
    {
        if (spriteRenderer != null) return spriteRenderer;

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null && pc.VisualSpriteRenderer != null)
            spriteRenderer = pc.VisualSpriteRenderer;
        else
            spriteRenderer = GetComponent<SpriteRenderer>();

        return spriteRenderer;
    }

    void Start()
    {
        healthBar = gameObject.AddComponent<HealthBar>();
        healthBar.barColor = new Color(0.2f, 0.8f, 0.2f);
        healthBar.yOffset = 0.6f;
        healthBar.Setup(maxHealth, CurrentHealth, healthBar.barColor);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);
        if (healthBar != null) healthBar.UpdateBar(CurrentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (IsInvincible) return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(0, CurrentHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
        if (healthBar != null) healthBar.UpdateBar(CurrentHealth);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.hurtSFX);

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
        if (healthBar != null) healthBar.UpdateBar(CurrentHealth);
    }

    void Die()
    {
        OnPlayerDied?.Invoke();
        GameManager.Instance.LoseLife();

        if (GameManager.Instance.Lives > 0)
        {
            Respawn();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Respawn()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth);
        if (healthBar != null) healthBar.UpdateBar(CurrentHealth);
        transform.position = respawnPosition;
        StartCoroutine(InvincibilityCoroutine());
    }

    IEnumerator InvincibilityCoroutine()
    {
        IsInvincible = true;
        SpriteRenderer sr = GetVisualSpriteRenderer();

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            if (sr != null)
                sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        if (sr != null)
            sr.enabled = true;
        IsInvincible = false;
    }

    public void SetRespawnPosition(Vector2 pos)
    {
        respawnPosition = pos;
    }
}
