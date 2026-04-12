using UnityEngine;
using System.Collections.Generic;

public class MenuBackground : MonoBehaviour
{
    private List<Particle> particles = new List<Particle>();
    private List<SpriteRenderer> particleRenderers = new List<SpriteRenderer>();
    private float spawnTimer;

    private const int maxParticles = 40;
    private const float spawnInterval = 0.3f;

    struct Particle
    {
        public Transform transform;
        public float speed;
        public float life;
        public float maxLife;
        public float driftX;
        public float alpha;
    }

    void Start()
    {
        for (int i = 0; i < 15; i++)
            SpawnParticle(true);
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && particles.Count < maxParticles)
        {
            SpawnParticle(false);
            spawnTimer = spawnInterval;
        }

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            Particle p = particles[i];
            p.life -= Time.deltaTime;

            if (p.life <= 0f || p.transform == null)
            {
                if (p.transform != null) Destroy(p.transform.gameObject);
                particles.RemoveAt(i);
                particleRenderers.RemoveAt(i);
                continue;
            }

            float t = p.life / p.maxLife;
            float fadeAlpha = t < 0.3f ? t / 0.3f : (t > 0.7f ? (1f - t) / 0.3f : 1f);
            fadeAlpha *= p.alpha;

            p.transform.position += new Vector3(p.driftX, p.speed, 0) * Time.deltaTime;
            p.transform.localScale *= (1f + 0.01f * Time.deltaTime);

            SpriteRenderer sr = particleRenderers[i];
            Color c = sr.color;
            c.a = fadeAlpha;
            sr.color = c;

            particles[i] = p;
        }
    }

    void SpawnParticle(bool randomY)
    {
        GameObject obj = new GameObject("MenuParticle");
        obj.transform.SetParent(transform);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = -1;

        float type = Random.value;
        Color color;
        float size;

        if (type < 0.4f)
        {
            color = new Color(0.3f, 0.5f, 0.9f);
            size = Random.Range(0.05f, 0.15f);
            sr.sprite = SpriteGenerator.CreateCircleSprite(8, Color.white);
        }
        else if (type < 0.7f)
        {
            color = new Color(0.6f, 0.4f, 0.9f);
            size = Random.Range(0.03f, 0.1f);
            sr.sprite = SpriteGenerator.CreateCircleSprite(8, Color.white);
        }
        else
        {
            color = new Color(1f, 0.85f, 0.4f);
            size = Random.Range(0.02f, 0.08f);
            sr.sprite = SpriteGenerator.CreateDiamondSprite(8, Color.white);
        }

        float alpha = Random.Range(0.1f, 0.35f);
        color.a = 0f;
        sr.color = color;

        float x = Random.Range(-8f, 8f);
        float y = randomY ? Random.Range(-6f, 6f) : -6.5f;
        obj.transform.position = new Vector3(x, y, 0);
        obj.transform.localScale = Vector3.one * size;

        Particle p = new Particle
        {
            transform = obj.transform,
            speed = Random.Range(0.3f, 1.2f),
            life = Random.Range(6f, 14f),
            driftX = Random.Range(-0.3f, 0.3f),
            alpha = alpha
        };
        p.maxLife = p.life;

        if (randomY)
            p.life = Random.Range(0f, p.maxLife);

        particles.Add(p);
        particleRenderers.Add(sr);
    }

    void OnDestroy()
    {
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            if (particles[i].transform != null)
                Destroy(particles[i].transform.gameObject);
        }
        particles.Clear();
        particleRenderers.Clear();
    }
}
