using UnityEngine;

public class PropHealthPack : PropBase
{
    public int healAmount = 2;

    protected override void ApplyEffect(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
        }
    }
}
