using UnityEngine;

public class EnemyReinforced : EnemyBase
{
    protected override void Awake()
    {
        base.Awake();
        maxHealth = 5;
        moveSpeed = 1.5f;
        damage = 2;
        attackCooldown = 2f;
        scoreValue = 20;
        bulletSpeed = 4.5f;
        currentHealth = maxHealth;
    }
}
