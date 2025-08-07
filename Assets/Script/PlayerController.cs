using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    // ŠO•”‚©‚ç“®‚«‚ğ§Œä‚·‚é‚½‚ß‚Ì•Ï”
    public bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // canMove‚ªtrue‚Ì‚¾‚¯ˆÚ“®ˆ—‚ğs‚¤
        if (canMove)
        {
           
        }
        else
        {
            // “®‚«‚ğ~‚ß‚é
            rb.velocity = Vector2.zero;
        }
    }
}