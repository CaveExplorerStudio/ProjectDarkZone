using UnityEngine;
using System.Collections;

public class Climbing : MonoBehaviour {
    public bool isClimbing;
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

    void FixedUpdate()
    {
        if (isClimbing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (rigidbody.velocity.y < maxSpeed)
                    rigidbody.AddForce(Vector2.up * moveForce);

                if (Mathf.Abs(rigidbody.velocity.y) > maxSpeed)
                    rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Sign(rigidbody.velocity.y) * maxSpeed);
            }
            
        }

    }

}
