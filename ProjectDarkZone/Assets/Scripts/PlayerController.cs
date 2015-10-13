using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool facingRight = true;
    [HideInInspector]
    public bool jump = false;
    [HideInInspector]
    public bool crouch = false;

    public float moveForce = 350f;
    public float maxSpeed = 4f;
    public float jumpForce = 800f;
    
    private bool grounded = false;
    private Animator anim;
    private new Rigidbody2D rigidbody;
    private PolygonCollider2D body;
    private Vector2[] standing;
    private Vector2[] crouched;


    void Awake()
    {
        anim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        body = GetComponent<PolygonCollider2D>();
        standing = body.points;
        crouched = new Vector2[standing.Length];

        int min = 0;
        for (int i = 0; i < standing.Length; i++)
        {
            if (standing[i].y < standing[min].y)
                min = i;
        }
        for (int i = 0; i < standing.Length; i++)
        {
            float newY = (standing[i].y - standing[min].y) * 0.5f + standing[min].y;
            crouched[i] = new Vector2(standing[i].x, newY);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            jump = true;
        if (Input.GetKeyDown(KeyCode.LeftControl) && grounded) {
            crouch = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouch = false;
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        grounded = true;
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }


    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        if (h * rigidbody.velocity.x < maxSpeed)
            rigidbody.AddForce(Vector2.right * h * moveForce);
        
        if (Mathf.Abs(rigidbody.velocity.x) > maxSpeed)
            rigidbody.velocity = new Vector2(Mathf.Sign(rigidbody.velocity.x) * maxSpeed, rigidbody.velocity.y);
        
        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();
        
        if (jump)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
            rigidbody.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
        if (crouch)
            body.SetPath(0, crouched);
        else
            body.SetPath(0, standing);
    }


    void Flip()
    {
        facingRight = !facingRight;
        
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Crouch(bool crouch)
    {
        Vector2[] points = body.points;
        int min = 0;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].y < points[min].y)
                min = i;
        }
        float factor = 1f;
        if (crouch)
            factor = 0.5f;
        else
            factor = 2f;
        for (int i = 0; i < points.Length; i++)
        {
            float newY = (points[i].y - points[min].y) * factor + points[min].y;
            points[i] = new Vector2(points[i].x, newY);
        }
        body.SetPath(0, points);
    }
}
