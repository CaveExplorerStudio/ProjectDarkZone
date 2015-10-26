using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool facingRight = true;

    public Component mapGenerator;
    public float moveSpeed = 6f;
    public float jumpForce = 750f;
    
    private bool grounded, jump, crouch, upSlope, downSlope, onWall;
    private int direction;
    private new Rigidbody2D rigidbody;
    private PolygonCollider2D body;
    private CircleCollider2D feet;
    private Vector2[] standing, crouched;
    private Health health;


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        body = GetComponent<PolygonCollider2D>();
        feet = GetComponent<CircleCollider2D>();
        health = GetComponent<Health>();
        standing = body.points;
        crouched = new Vector2[standing.Length];
        grounded = false;
        onWall = false;
        upSlope = false;
        downSlope = false;

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
        if (Input.GetKey(KeyCode.Space) && grounded)
            jump = true;

        if (Input.GetKey(KeyCode.LeftControl) && grounded)
            crouch = true;
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            crouch = false;

        direction = 0;
        if (Input.GetKey(KeyCode.RightArrow))
            direction = 1;
        else if (Input.GetKey(KeyCode.LeftArrow))
            direction = -1;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(feet.IsTouching(collision.collider))
        {
            grounded = true;
            float velocity = Mathf.Abs(collision.relativeVelocity.magnitude);
            if(velocity > 16)
            {
                health.AddHealth((int)((16 - velocity) * 0.5));
            }
        }
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    public void SetUpSlope(bool value)
    {
        upSlope = value;
    }

    public void SetDownSlope(bool value)
    {
        downSlope = value;
    }

    public void SetOnWall(bool value)
    {
        onWall = value;
    }

    void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(direction * (onWall ? 0 : 1) * moveSpeed, rigidbody.velocity.y);

        if (direction == 1 && !facingRight)
            Flip();
        else if (direction == -1 && facingRight)
            Flip();
        
        if (jump)
        {
            jump = false;
            grounded = false;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
            rigidbody.AddForce(new Vector2(0f, jumpForce));
        }
        if (crouch)
            body.SetPath(0, crouched);
        else
            body.SetPath(0, standing);

        if (grounded && (upSlope || downSlope))
            NormalizeSlope();
    }

    private void NormalizeSlope()
    {
        float friction = (upSlope ? 1 : -1) * (facingRight ? 1 : -1) * 0.4f;
        float yVel = rigidbody.velocity.x == 0 ? 0 : rigidbody.velocity.y;
        rigidbody.velocity = new Vector2(rigidbody.velocity.x + friction, yVel);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        bool temp = upSlope;
        upSlope = downSlope;
        downSlope = temp;
        
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
