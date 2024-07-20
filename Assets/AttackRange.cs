using UnityEngine;

public class AttackRange : MonoBehaviour
{
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
        if (collision.name == "Player" && !slime.IsAttacking())
        {
            slime.TriggerAttack();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            slime.UntriggerAttack();
        }
    }
}
