using UnityEngine;

public class LevelObjectController : MonoBehaviour
{
    private Rigidbody2D rb;
    public int speed = -1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.velocity = new Vector2(speed, 0);
        if(transform.childCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
