using System.Collections;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static int count; // this is used by the SpawnController to track spawn counts
    PickupController pc;
    EnemyMovement enemyMovement;

    [System.Serializable]
    public class Resistances
    {
        [Range(0f, 1f)] public float freeze = 0f, kill = 0f, debuff = 0f;

        public static Resistances operator *(Resistances r, float factor)
        {
            r.freeze = Mathf.Clamp(r.freeze * factor, 0f, 1f);
            r.kill = Mathf.Clamp(r.kill * factor, 0f, 1f);
            r.debuff = Mathf.Clamp(r.debuff * factor, 0f, 1f);
            return r;
        }
    }

    [System.Serializable]
    public struct Stats
    {
        [Min(0)] public float maxHealth, moveSpeed, damage;
        public float knockbackMultiplier;
        public Resistances resistances;
        [System.Flags]
        public enum Boostable { health = 1, moveSpeed = 2, damage = 4, knockbackMultiplier = 8, resistances = 16 }
        public Boostable curseBoosts, levelBoosts;

        private static Stats Boost(Stats s1, float factor, Boostable boostable)
        {
            if ((boostable & Boostable.health) != 0) s1.maxHealth *= factor;
            if ((boostable & Boostable.moveSpeed) != 0) s1.moveSpeed *= factor;
            if ((boostable & Boostable.damage) != 0) s1.damage *= factor;
            if ((boostable & Boostable.knockbackMultiplier) != 0) s1.knockbackMultiplier *= factor;
            if ((boostable & Boostable.resistances) != 0) s1.resistances *= factor;
            return s1;
        }

        // Use the multiply operator for curses
        public static Stats operator *(Stats s1, float factor) { return Boost(s1, factor, s1.curseBoosts); }
        // Use the XOR operator for level boosted stats
        public static Stats operator ^(Stats s1, float factor) { return Boost(s1, factor, s1.levelBoosts); }
    }

    public Stats baseStats = new Stats
    {
        maxHealth = 10f,
        moveSpeed = 1f,
        damage = 3f,
        knockbackMultiplier = 1f,
        curseBoosts = (Stats.Boostable)(1 | 2),
        levelBoosts = 0
    };

    [Header("Enemy Stats")]
    Stats _actualStats;
    public Stats ActualStats
    {
        get { return _actualStats; }
    }

    float _currentHealth;

    [Header("Damage Feedback")]
    public Color damagedColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.3f;
    public Canvas damageCanvas;
    public TextMeshProUGUI damageDisplay;
    Color originalColor;
    SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        count++;
    }

    private void Start()
    {
        if (TryGetComponent(out PickupController component))
        {
            pc = component;
        }

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        enemyMovement = GetComponent<EnemyMovement>();

        RecalculateStats();
        _currentHealth = _actualStats.maxHealth;
    }

    public void RecalculateStats()
    {
        float curse = GameController.GetCumulativeCurse(), level = GameController.GetCumulativeLevels();
        _actualStats = (baseStats * curse) ^ level; // LMAO
    }

    public void TakeDamage(float amount, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        // this is where we can implement immunity to insta-kills or damage?

        _currentHealth = Mathf.Max(_currentHealth - amount, 0f);
        // Debug.Log($"The {name} has {health} health left after taking {amount} damage!");

        StartCoroutine(DamageFlash());
        if (damageDisplay && amount > 0)
        {
            StartCoroutine(ShowDamagePopup(amount));
        }

        if (knockbackForce > 0f)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            enemyMovement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if (_currentHealth <= 0f)
        {
            Kill();
        }
    }

    public void Kill()
    {
        // Debug.Log($"The {name} has been killed.");
        StartCoroutine(KillFade());

        if (pc) pc.OnHostKilled(); // update this with reference to if we are an elite enemy!
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && collision.collider.TryGetComponent(out Player player))
        {
            player.TakeDamage(_actualStats.damage);
        }
    }

    private void OnDestroy()
    {
        count--;
    }

    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha); // linear interpolate the alpha
        }

        Destroy(gameObject);
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
            if (popup == null) break;

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
}
