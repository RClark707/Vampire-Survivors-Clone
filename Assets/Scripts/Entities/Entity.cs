using System;
using System.Collections;
using TMPro;
using UnityEngine;

[Obsolete("Don't use!")]
[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour
{
    [HideInInspector]
    public float health { get; set; }
    protected PickupController pc;

    [Header("Damage Feedback")]
    public Color damagedColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    public Canvas damageCanvas;
    public TextMeshProUGUI damageDisplay;
    protected Color originalColor;
    protected SpriteRenderer sr;

    public virtual void Awake()
    {
        if (TryGetComponent(out PickupController component))
        {
            pc = component;
        }

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public virtual void TakeDamage(float amount, Vector3 source)
    {
        health = Mathf.Max(health - amount, 0f);
        // Debug.Log($"The {name} has {health} health left after taking {amount} damage!");
        if (damageDisplay != null)
        {
            StartCoroutine(ShowDamagePopup(amount));
        }
        StartCoroutine(DamageFlash());

        if (health <= 0f)
        {
            Kill();
        }
    }

    protected IEnumerator DamageFlash()
    {
        sr.color = damagedColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    // TODO: Improve this script!
    protected IEnumerator ShowDamagePopup(float amount, bool criticalHit = false)
    {
        // Debug.Log($"{amount} damage was recorded by {gameObject.name}");

        TextMeshProUGUI popup = Instantiate(damageDisplay, transform.position, Quaternion.identity);
        popup.text = Mathf.RoundToInt(amount).ToString();
        popup.transform.SetParent(damageCanvas.transform);
        popup.transform.position = transform.position + new Vector3(1.2f, 0.4f, 0f);
        // modify other properties of the text here

        if (criticalHit)
        {
            // add particle effects here
            popup.color = Color.red;
        }

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0f;

        Destroy(popup.gameObject, damageFlashDuration);

        while (t < damageFlashDuration)
        {
            if (popup.gameObject == null) break;

            popup.color = new Color(popup.color.r, popup.color.g, popup.color.b, 1 - t / damageFlashDuration); // linear interpolate the alpha

            popup.transform.position = Vector3.MoveTowards(
                popup.transform.position,
                new Vector3(
                    popup.transform.position.x,
                    popup.transform.position.y + 0.03f * (1f - t / damageFlashDuration),
                    popup.transform.position.z
                    ),
                1f);

            yield return w;
            t += Time.deltaTime;
        }
    }

    public virtual void Kill()
    {
        Debug.Log($"The {name} has been killed.");
        if (pc) pc.OnHostKilled();
        Destroy(gameObject);
    }
}
