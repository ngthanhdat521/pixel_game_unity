using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject healthBar;
    public GameObject expBar;
    public BoxCollider2D boxCollider2D;

    // Attack Hitbox Player
    public GameObject upHitbox;
    public GameObject downHitbox;
    public GameObject leftHitbox;
    public GameObject rightHitbox;
    Dictionary<string, GameObject> hitboxes;
    public GameObject hitPopup;
    public TMP_Text textPopup;

    public static readonly float SPEED = 1300;

    // Private props
    private float strength = 5;
    private float health = 100;

    // Audio
    public AudioSource playerAudio;
    public AudioClip attackClip;
    public AudioClip deadClip;

    // Player Stat
    public static readonly float NEXT_LEVEL_EXP = 100;
    private int level = 1;
    private float exp = 0;
    public string direction = "";
    public TMP_Text levelText;
    public static readonly float ATTACK_DELAY_TIME = 0.5f; // Thời gian delay mong muốn
    private float lastAttackTime = 0f; // Thời gian đánh đòn trước
    private float lastHurtTime = 0f; // Thời gian đánh đòn trước
    private float lastMoveTime = 0f; // Thời gian đánh đòn trước


    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;

        hitboxes = new Dictionary<string, GameObject>()
        {
            {"RightHitbox", rightHitbox},
            {"LeftHitbox", leftHitbox},
            {"UpHitbox", upHitbox},
            {"DownHitbox", downHitbox},
        };
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1; // Tỉ lệ từ 0 đến 1

        if (stateInfo.IsName("Player_Die"))
        {
            int currentFrame = Mathf.FloorToInt(normalizedTime * 3);
            if (currentFrame == 2)
            {
                animator.enabled = false;
            }
        }

        if (!IsDead())
        {
            rb.velocity = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ) * SPEED * Time.deltaTime;
        }

        animator.SetFloat("MoveX", rb.velocity.x);
        animator.SetFloat("MoveY", rb.velocity.y);

        if (
            Input.GetAxisRaw("Horizontal") == 1 ||
            Input.GetAxisRaw("Horizontal") == -1 ||
            Input.GetAxisRaw("Vertical") == 1 ||
            Input.GetAxisRaw("Vertical") == -1
        ) {
            animator.SetFloat("LastMoveX", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("LastMoveY", Input.GetAxisRaw("Vertical"));
        }

        OnAttack();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Xử lý va chạm
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (DelayTimeForHurt(ATTACK_DELAY_TIME))
        {
            OnHit(collision);
        }
    }

    private void OnAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Check for space key press
        {
            if (DelayTime(ATTACK_DELAY_TIME))
            {
                animator.SetTrigger("Attack");
                playerAudio.PlayOneShot(attackClip);

                OnActiveAttack();
            }
        }
    }

    public void OnActiveAttack()
    {
        GameObject hitbox = GetCurrentHitbox();

        if (hitbox != null)
        {
            hitbox.SetActive(true);
        }
    }

    public void OnInactiveAttack()
    {
        GameObject hitbox = GetCurrentHitbox();

        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }
    }

    public string OnAttackDirection()
    {
        float lastMoveX = animator.GetFloat("LastMoveX");
        float lastMoveY = animator.GetFloat("LastMoveY");

        // Down Hitbox
        if (lastMoveX == 0 && lastMoveY == -1)
        {
            return "DownHitbox";
        }
        // Up Hitbox
        if (lastMoveX == 0 && lastMoveY == 1)
        {
            return "UpHitbox";
        }
        // Left Hitbox
        if (lastMoveX == -1 && lastMoveY == 0)
        {
            return "LeftHitbox";
        }
        // Right Hitbox
        if (lastMoveX == 1 && lastMoveY == 0)
        {
            return "RightHitbox";
        }

        return "";
    }

    public GameObject GetCurrentHitbox()
    {
        direction = OnAttackDirection();

        if (ContainsHitbox(direction))
        {
            GameObject hitbox = hitboxes[direction];
            return hitbox;
        }

        return null;
    }

    public void OnHit(Collision2D collision)
    {
        if (collision.gameObject.name.IndexOf("Slime") >= 0)
        {
            GameObject popup = Instantiate(hitPopup, transform.position, Quaternion.identity);
            popup.transform.SetParent(transform);
            popup.transform.localPosition = Vector3.zero;
            popup.transform.position = transform.position;

            textPopup.text = "10";
            health -= 10;
            float healthPercent = health / 100;

            healthBar.transform.localScale = new Vector2(
                1 * healthPercent,
                healthBar.transform.localScale.y
            );
            
            if (IsDead())
            {
                animator.SetTrigger("Die");
                playerAudio.PlayOneShot(deadClip);
                boxCollider2D.enabled = false;
                rb.velocity = Vector2.zero;
            }
        }
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public float CalcExpTotal()
    {
        if (level > 1) return exp + level * NEXT_LEVEL_EXP;
        return exp;
    }

    public float CalcExpPercent()
    {
        return exp / NEXT_LEVEL_EXP;
    }

    public void LevelUp()
    {
        level = Mathf.CeilToInt(CalcExpTotal() / NEXT_LEVEL_EXP);
        exp = exp % NEXT_LEVEL_EXP;
        levelText.text = $"Lv.0{level}";
    }

    public void GainExp(float exp)
    {
        this.exp += exp;

        if (CalcExpPercent() >= 1)
        {
            LevelUp();
        }

        expBar.transform.localScale = new Vector2(
            1 * CalcExpPercent(),
            expBar.transform.localScale.y
        );
    }

    public float CalcStrength()
    {
        return strength + level * 5;
    }

    public bool ContainsHitbox(string collisionName)
    {
        return hitboxes.ContainsKey(collisionName);
    }

    public void ActiveAttackDirection(string collisionName)
    {
        GameObject hitbox = hitboxes[collisionName];
        hitbox.SetActive(false);
    }

    public bool DelayTime(float seconds)
    {
        float currentTime = Time.time;
        if (currentTime - lastAttackTime >= seconds)
        {
            lastAttackTime = currentTime;
            return true;
        }


        return false;
    }

    public bool DelayTimeForMovement(float seconds)
    {
        float currentTime = Time.time;
        if (currentTime - lastMoveTime >= seconds)
        {
            lastMoveTime = currentTime;
            return true;
        }


        return false;
    }

    public bool DelayTimeForHurt(float seconds)
    {
        float currentTime = Time.time;
        if (currentTime - lastHurtTime >= seconds)
        {
            lastHurtTime = currentTime;
            return true;
        }


        return false;
    }
}
