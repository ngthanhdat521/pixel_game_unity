using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject healthBar;
    public GameObject expBar;

    // Attack Hitbox Player
    public GameObject upHitbox;
    public GameObject downHitbox;
    public GameObject leftHitbox;
    public GameObject rightHitbox;
    Dictionary<string, GameObject> hitboxes;
    public GameObject hitPopup;
    public TMP_Text textPopup;

    public static readonly float SPEED = 3000;

    // Private props
    private float strength = 5;

    // Audio
    public AudioSource playerAudio;
    public AudioClip attackClip;

    // Player Stat
    public static readonly float NEXT_LEVEL_EXP = 100;
    private int level = 1;
    private float exp = 0;
    public string direction = "";
    public TMP_Text levelText;

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
        rb.velocity = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ) * SPEED * Time.deltaTime;

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

    private void OnAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Check for space key press
        {
            animator.SetTrigger("Attack");
            playerAudio.PlayOneShot(attackClip);

            OnActiveAttack();
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

    private void CreateHitPopup()
    {
        GameObject popup = Instantiate(hitPopup, transform.position, Quaternion.identity);
        popup.transform.SetParent(transform);
        popup.transform.localPosition = Vector3.zero;
        popup.transform.position = transform.position;

        textPopup.text = strength.ToString();
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
        Debug.Log("Hitbox unactive " + collisionName);
        hitbox.SetActive(false);
    }
}
