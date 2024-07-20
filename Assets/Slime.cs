using TMPro;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public static readonly float MOVE_SPEED = 1f;
    public static readonly float SPEED = 100;
    public float currentMoveSpeed = MOVE_SPEED;
    public Vector3 originalX;

    // Hitbox
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
    public AudioClip attackClip;

    // Slime Stat
    public bool IsTargeting;

    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;
        IsTargeting = false;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Slime_Attack") || IsTargeting) {
            float normalizedTime = stateInfo.normalizedTime % 1; // Tỉ lệ từ 0 đến 1
            int currentFrame = Mathf.FloorToInt(normalizedTime * 7);
            if (currentFrame >= 2 && currentFrame <= 4)
            {
                rb.velocity = new Vector3(currentMoveSpeed * 3, 0);
            }
        }
        else
        {
            OnMovement();
            animator.SetFloat("MoveX", rb.velocity.x);
            animator.SetFloat("MoveY", rb.velocity.y);
        }
    }

    public bool IsAttacking()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.IsName("Slime_Attack");
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("collision1" + collision.gameObject.name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("collision2" + collision.gameObject.name);

        OnHit(collision);
        player.OnHit(collision);
    }

    private void OnHit(Collider2D collision)
    {
        if (player.ContainsHitbox(collision.name))
        {
            CreateHitPopup();
            player.OnInactiveAttack();
        }
    }

    private void OnMovement()
    {
        if (Vector3.Distance(originalX, transform.position) > 5)
        {
            currentMoveSpeed = -currentMoveSpeed;
            originalX = transform.position;
        }

        rb.velocity = new Vector3(currentMoveSpeed, 0);
    }

    //private void OnFollow()
    //{
    //    Vector2 direction = player.transform.position - transform.position;
    //    float distance = Vector3.Distance(player.transform.position, transform.position);

    //    if (distance > 5)
    //    {
    //        direction.x = 0;
    //        direction.y = 0;
    //    }

    //    rb.velocity = direction * SPEED * Time.deltaTime;
    //}

    private void OnAttack()
    {
        //animator.SetTrigger("Attack");
        //Vector3 targetPosition = player.transform.position;
        //transform.position = Vector3.MoveTowards(
        //    transform.position,
        //    targetPosition,
        //    1 * Time.deltaTime
        //);
        //Vector2 next = Vector2.MoveTowards(
        //    transform.position,
        //    player.transform.position,
        //    1 * Time.deltaTime
        //);
        //Vector2 direction = player.transform.position - transform.position;
        //direction.y += 5;
        //rb.velocity = direction * 50 * Time.deltaTime;
        //Debug.Log("attack slime " + direction + " " + rb.velocity);
        //rb.velocity = next * 500 * Time.deltaTime;
        rb.AddForce(new Vector2(5, 5), ForceMode2D.Impulse);
        rb.gravityScale = 1;
    }

    public void TriggerAttack()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3 slimePosition = transform.position;

        // So sánh vị trí để xác định Player ở bên trái hay bên phải Slime
        if (playerPosition.x < slimePosition.x)
        {
            Debug.Log("Player is on the left side of Slime.");
            // Thực hiện các hành động khi Player ở bên trái Slime
            currentMoveSpeed = -MOVE_SPEED;
            animator.SetFloat("MoveX", -1);

        }
        else if (playerPosition.x > slimePosition.x)
        {
            Debug.Log("Player is on the right side of Slime.");
            // Thực hiện các hành động khi Player ở bên phải Slime
            currentMoveSpeed = MOVE_SPEED;
            animator.SetFloat("MoveX", 1);

        }
        else
        {
            Debug.Log("Player is directly on top of Slime or at the same position.");
            // Xử lý khi Player ở trên hoặc cùng vị trí với Slime (nếu cần)
        }

        animator.SetTrigger("Attack");
        rb.velocity = Vector2.zero;
        IsTargeting = true;
        audioSource.PlayOneShot(attackClip);
    }

    public void UntriggerAttack()
    {
        IsTargeting = false;
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
