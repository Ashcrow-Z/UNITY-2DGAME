using UnityEngine;

public class EnemyNormal : EnemyBase
{
    protected override void Awake()
    {
        base.Awake();
        maxHealth = 2;
        moveSpeed = 2.5f;
        damage = 1;
        attackCooldown = 1.5f;
        scoreValue = 10;
        currentHealth = maxHealth;
    }
}
