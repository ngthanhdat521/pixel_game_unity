using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform player;
    public static readonly float FOLLOW_SPEED = 5f;
    public static readonly float CAMERA_LIMIT_X = 14.77f;
    public static readonly float CAMERA_LIMIT_Y = 13.72f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GetNextVector();
    }

    Vector3 GetNextVector()
    {
        Vector3 next = Vector3.Slerp(
            transform.position,
            player.position,
            FOLLOW_SPEED * Time.deltaTime
        );

        float x = transform.position.x;
        float y = transform.position.y;

        if (next.x >= 13 && next.x <= 25)
        {
            x = next.x;
        }

        if (next.y >= 2.6f && next.y <= 13.5f)
        {
            y = next.y;
        }

        return new Vector3(x, y, -10);
    }
}
