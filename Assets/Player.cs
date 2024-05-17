using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject heart;
    public GameObject rootHealthBar;
    public GameObject hearthBar;

    // Attack Hitbox Player
    public GameObject upHitbox;
    public GameObject downHitbox;
    public GameObject leftHitbox;
    public GameObject rightHitbox;
    public GameObject hitPopup;
    public TMP_Text textPopup;

    public static readonly float SPEED = 3000;

    // Private props
    public float strength = 15;

    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ) * Player.SPEED * Time.deltaTime;

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
        float lastMoveX = animator.GetFloat("LastMoveX");
        float lastMoveY = animator.GetFloat("LastMoveY");

        if (Input.GetKeyDown(KeyCode.Space)) // Check for space key press
        {
            animator.SetTrigger("Attack");
            
            // Down Hitbox
            if (lastMoveX == 0 && lastMoveY == -1)
            {
                downHitbox.SetActive(true);
            }
            // Up Hitbox
            if (lastMoveX == 0 && lastMoveY == 1)
            {
                upHitbox.SetActive(true);
            }
            // Left Hitbox
            if (lastMoveX == -1 && lastMoveY == 0)
            {
                leftHitbox.SetActive(true);
            }
            // Right Hitbox
            if (lastMoveX == 1 && lastMoveY == 0)
            {
                rightHitbox.SetActive(true);
            }
        }
    }

    private void CreateHitPopup()
    {
        GameObject popup = Instantiate(hitPopup, transform.position, Quaternion.identity);
        popup.transform.SetParent(transform);
        popup.transform.localPosition = Vector3.zero;
        popup.transform.position = transform.position;

        textPopup.text = strength.ToString();
    }
}
