using UnityEngine;
using System.Collections;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void SpawnExplosion(Vector3 position, Color color, float scale = 1f)
    {
        StartCoroutine(ExplosionEffect(position, color, scale));
    }

    public void SpawnPickupEffect(Vector3 position, Color color)
    {
        StartCoroutine(PickupEffect(position, color));
    }

    IEnumerator ExplosionEffect(Vector3 position, Color color, float scale)
    {
        int particleCount = 8;
        GameObject[] particles = new GameObject[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            GameObject p = new GameObject("Particle");
            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteGenerator.CreateCircleSprite(8, color);
            sr.sortingOrder = 10;
            p.transform.position = position;
            p.transform.localScale = Vector3.one * 0.15f * scale;
            particles[i] = p;
        }

        float duration = 0.4f;
        float elapsed = 0f;

        Vector2[] directions = new Vector2[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            float angle = (360f / particleCount) * i * Mathf.Deg2Rad;
            directions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            for (int i = 0; i < particleCount; i++)
            {
                if (particles[i] != null)
                {
                    particles[i].transform.position = position + (Vector3)(directions[i] * t * 1.5f * scale);
                    particles[i].transform.localScale = Vector3.one * 0.15f * scale * (1f - t);
                    SpriteRenderer sr = particles[i].GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color c = sr.color;
                        c.a = 1f - t;
                        sr.color = c;
                    }
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < particleCount; i++)
        {
            if (particles[i] != null)
                Destroy(particles[i]);
        }
    }

    IEnumerator PickupEffect(Vector3 position, Color color)
    {
        GameObject ring = new GameObject("PickupRing");
        SpriteRenderer sr = ring.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteGenerator.CreateCircleSprite(16, color);
        sr.sortingOrder = 10;
        ring.transform.position = position;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            ring.transform.localScale = Vector3.one * (0.2f + t * 1.5f);
            Color c = color;
            c.a = 1f - t;
            sr.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(ring);
    }
}
