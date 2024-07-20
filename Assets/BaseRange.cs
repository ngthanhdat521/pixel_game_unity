using UnityEngine;

public class BaseRange : MonoBehaviour
{
    public Player player;
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
        slime.OnHit(collision);
        player.OnHit(collision);
    }
}
