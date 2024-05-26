using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public Rigidbody2D rb;
    public Slime slime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"Trigger Attack Range {collision.gameObject.name}");

        if (collision.name == "Player")
        {
            slime.TriggerFollow();
        }
    }
}
