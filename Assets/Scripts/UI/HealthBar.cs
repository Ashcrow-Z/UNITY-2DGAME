using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float barWidth = 0.8f;
    public float barHeight = 0.08f;
    public float yOffset = 0.55f;
    public Color barColor = Color.green;
    public Color bgColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public int sortingOrder = 10;

    private Transform barFill;
    private SpriteRenderer fillRenderer;
    private int maxHP;
    private int currentHP;
    private Transform barRoot;

    public void Setup(int maxHealth, int currentHealth, Color color)
    {
        maxHP = maxHealth;
        currentHP = currentHealth;
        barColor = color;
        BuildBar();
        UpdateBar(currentHealth);
    }

    void BuildBar()
    {
        barRoot = new GameObject("HealthBarRoot").transform;
        barRoot.SetParent(transform);
        barRoot.localPosition = new Vector3(0, yOffset, 0);
        barRoot.localRotation = Quaternion.identity;

        Sprite pixel = SpriteGenerator.CreateSquareSprite(4, Color.white);

        GameObject bg = new GameObject("HealthBG");
        bg.transform.SetParent(barRoot);
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localRotation = Quaternion.identity;
        bg.transform.localScale = new Vector3(barWidth, barHeight, 1f);
        SpriteRenderer bgSR = bg.AddComponent<SpriteRenderer>();
        bgSR.sprite = pixel;
        bgSR.color = bgColor;
        bgSR.sortingOrder = sortingOrder;

        GameObject fill = new GameObject("HealthFill");
        fill.transform.SetParent(barRoot);
        fill.transform.localPosition = Vector3.zero;
        fill.transform.localRotation = Quaternion.identity;
        fill.transform.localScale = new Vector3(barWidth, barHeight, 1f);
        fillRenderer = fill.AddComponent<SpriteRenderer>();
        fillRenderer.sprite = pixel;
        fillRenderer.color = barColor;
        fillRenderer.sortingOrder = sortingOrder + 1;
        barFill = fill.transform;
    }

    public void UpdateBar(int hp)
    {
        currentHP = hp;
        if (barFill == null || maxHP <= 0) return;

        float ratio = Mathf.Clamp01((float)hp / maxHP);
        barFill.localScale = new Vector3(barWidth * ratio, barHeight, 1f);
        float offset = -(barWidth * (1f - ratio)) / 2f;
        barFill.localPosition = new Vector3(offset, 0, 0);

        if (ratio > 0.5f)
            fillRenderer.color = Color.Lerp(Color.yellow, Color.green, (ratio - 0.5f) * 2f);
        else
            fillRenderer.color = Color.Lerp(Color.red, Color.yellow, ratio * 2f);
    }

    void LateUpdate()
    {
        if (barRoot != null)
            barRoot.rotation = Quaternion.identity;
    }
}
