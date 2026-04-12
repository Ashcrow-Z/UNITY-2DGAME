using UnityEngine;

public class PropFirepower : PropBase
{
    public float boostDuration = 10f;

    protected override void ApplyEffect(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.ActivateFirepowerBoost(boostDuration);
        }
    }
}
