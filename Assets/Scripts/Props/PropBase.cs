using UnityEngine;

public abstract class PropBase : MonoBehaviour
{
    public float bobSpeed = 2f;
    public float bobAmount = 0.1f;

    private Vector3 startPos;

    protected virtual void Start()
    {
        startPos = transform.position;
    }

    protected virtual void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ApplyEffect(other.gameObject);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.pickupSFX);

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (VFXManager.Instance != null && sr != null)
                VFXManager.Instance.SpawnPickupEffect(transform.position, sr.color);

            Destroy(gameObject);
        }
    }

    protected abstract void ApplyEffect(GameObject player);
}
