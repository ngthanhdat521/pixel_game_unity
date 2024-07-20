using UnityEngine;

public class HurtRange : MonoBehaviour
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
        //slime.OnHit(collision);

        //if (slime.DelayTime(0.5f))
        //{
        //    if (collision.name == "Player")
        //    {
        //        Debug.Log("okk");
        //    }
        //}
    }
}
