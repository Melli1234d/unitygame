using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;

    private void Awake()
    {

        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        
        //Richtungswechsel links/rechts vom spieler
        if (horizontalInput > 0.05f)
            transform.localScale = new Vector2(5, 5);
        else if (horizontalInput < -0.05f)
            transform.localScale = new Vector2(-5, 5);

        //set animator Hinweis: name in klammern muss genau so geschrieben sein wie in animation window
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        //Wandspringen
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero; //wenn er dran springt fällt er nicht
            }
            else
                body.gravityScale = 7;
        }
        else
            wallJumpCooldown += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
            Jump();
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            wallJumpCooldown = 0;
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6); //- weil der spieler weg von der wand gedrückt werden soll
        }  
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x,0.1f), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
