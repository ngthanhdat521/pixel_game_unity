using TMPro;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public static readonly float MOVE_SPEED = 1f;
    public float currentMoveSpeed = MOVE_SPEED;
    public Vector3 originalX;

    // Hitbox
    public GameObject rightHitbox;
    public GameObject leftHitbox;
    public GameObject upHitbox;
    public GameObject downHitbox;
    public GameObject hitPopup;
    public TMP_Text textPopup;
    public Player player;


    // Hp Monnster
    public GameObject healthBar;
    public static readonly float SLIME_HEALTH_STAT = 200;
    public static readonly float SLIME_HEALTH_WIDTH = 0.3336f;
    private float healthStat = SLIME_HEALTH_STAT;

    // Audio
    public AudioSource audioSource;
    public AudioClip deathClip;

    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;
        originalX = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        IsFacingRight();
        rb.velocity = new Vector3(currentMoveSpeed, 0);
        animator.SetFloat("MoveX", rb.velocity.x);
        animator.SetFloat("MoveY", rb.velocity.y);
    }

    public void IsFacingRight()
    {
        if (Vector3.Distance(originalX, transform.position) > 5) {
            currentMoveSpeed = -currentMoveSpeed;
            originalX = transform.position;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log($"OnCollisionStay2D {collision.gameObject.name}");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"OnTriggerStay2D {collision.gameObject.name}");
        OnHit(collision);
    }

    private void OnHit(Collider2D collision)
    {
        if (player.ContainsHitbox(collision.name))
        {
            CreateHitPopup();
            player.OnInactiveAttack();
        }
    }

    private void CreateHitPopup()
    {
        GameObject popup = Instantiate(hitPopup, transform.position, Quaternion.identity);
        popup.transform.SetParent(transform);
        popup.transform.localPosition = Vector3.zero;
        popup.transform.position = transform.position;

        float strenth = player.CalcStrength();

        textPopup.text = strenth.ToString();

        healthStat -= strenth;
        Injured(healthStat / SLIME_HEALTH_STAT);
        animator.SetTrigger("Injured");

        if (healthStat <= 0)
        {
            audioSource.PlayOneShot(deathClip);
            player.GainExp(150);
            Invoke("Die", deathClip.length);
        }
    }

    private void Die()
    {
        animator.SetTrigger("Dead");
    }

    private void Injured(float percent)
    {
        healthBar.transform.localScale = new Vector2(SLIME_HEALTH_WIDTH * percent, healthBar.transform.localScale.y);
    }
}
