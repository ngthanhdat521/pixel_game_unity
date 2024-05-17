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
        if (collision.name == "RightHitbox")
        {
            Debug.Log("RightHitbox animation finished!");
            rightHitbox.SetActive(false);
            CreateHitPopup();
        }

        if (collision.name == "LeftHitbox")
        {
            Debug.Log("LeftHitbox animation finished!");
            leftHitbox.SetActive(false);
            CreateHitPopup();
        }

        if (collision.name == "UpHitbox")
        {
            Debug.Log("UpHitbox animation finished!");
            upHitbox.SetActive(false);
            CreateHitPopup();
        }

        if (collision.name == "DownHitbox")
        {
            Debug.Log("DownHitbox animation finished!");
            downHitbox.SetActive(false);
            CreateHitPopup();
        }
    }

    private void CreateHitPopup()
    {
        GameObject popup = Instantiate(hitPopup, transform.position, Quaternion.identity);
        popup.transform.SetParent(transform);
        popup.transform.localPosition = Vector3.zero;
        popup.transform.position = transform.position;

        textPopup.text = player.strength.ToString();
    }
}
