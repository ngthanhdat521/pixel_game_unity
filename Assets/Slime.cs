using TMPro;
using System.Collections;
using System.Collections.Generic;
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

    // Slime Stat
    public string status = "Attack";

    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;
        originalX = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        OnAttack();
        //if (status == "Attack")
        //{
        //    Debug.Log("attack trigger123");
        //    OnAttack();
        //}
        //else if (status == "Chase")
        //{
        //    OnFollow();
        //}
        //else
        //{
        //    IsFacingRight();
        //    OnMovement();
        //}

        //animator.SetFloat("MoveX", rb.velocity.x);
        //animator.SetFloat("MoveY", rb.velocity.y);
    }

    public void IsFacingRight()
    {
        if (Vector3.Distance(originalX, transform.position) > 5) {
            currentMoveSpeed = -currentMoveSpeed;
            originalX = transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
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

    private void OnMovement()
    {
        rb.velocity = new Vector3(currentMoveSpeed, 0);
    }

    private void OnFollow()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance > 5)
        {
            direction.x = 0;
            direction.y = 0;
            TriggerIddle();
        }

        rb.velocity = direction * SPEED * Time.deltaTime;
    }

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
        rb.AddForce(Vector2.up * 700f);
    }

    public void TriggerAttack()
    {
        status = "Attack";
    }

    public void TriggerFollow()
    {
        status = "Chase";
    }

    public void TriggerIddle()
    {
        status = "";
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
