using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool facingRight = true, grounded = false;

    public EventController events;
    public float moveSpeed = 6f;
    public float jumpForce = 400f;

    private static string heldGem = null;
    private static int numCollected = 0;
    private static string[] collectedGems = new string[8];

    private bool jump, crouch, upSlope, downSlope, onWall, movementEnabled;
    private int direction;
    private new Rigidbody2D rigidbody;
    private PolygonCollider2D body;
    private CircleCollider2D feet;
    private Vector2[] standing, crouched;
    private Health health;
	private Animator anim;
    private PlayerHUDController hud;
	private GameObject headLight;


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        body = GetComponent<PolygonCollider2D>();
        feet = GetComponent<CircleCollider2D>();
        health = GetComponent<Health>();
		movementEnabled = true;
        standing = body.points;
        crouched = new Vector2[standing.Length];
        grounded = false;
        onWall = false;
        upSlope = false;
        downSlope = false;
		anim = GetComponent<Animator>();
        hud = GetComponent<PlayerHUDController>();
		headLight = GameObject.Find ("HeadLight");

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

    void Start()
    {
        hud.UpdateGemIcon(heldGem);
    }

    void Update()
    {
        if(health.GetHealth() == 0)
        {
            StartCoroutine(Death());
        }

		if(movementEnabled)
		{
	        if (Input.GetKey(KeyCode.Space) && grounded)
	            jump = true;

	        if (Input.GetKey(KeyCode.LeftControl) && grounded)
	            crouch = true;
	        else if (Input.GetKeyUp(KeyCode.LeftControl))
	            crouch = false;

            direction = 0;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                direction = 1;
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                direction = -1;
        }
		else
		{
			jump = false;
			crouch = false;
			direction = 0;
		}
    }

	public void disableMovement()
	{
		movementEnabled = false;
	}

	public void enableMovement()
	{
		movementEnabled = true;
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(feet.IsTouching(collision.collider))
        {
            grounded = true;
            float velocity = Mathf.Abs(collision.relativeVelocity.magnitude);
			float verticalVelocity = collision.relativeVelocity.y;
//			Debug.Log ("Velocity: " + verticalVelocity.ToString());
            if(velocity > 16)
            {
                health.AddHealth((int)((16 - velocity) * 0.5));
            }
			else if (verticalVelocity < -10.0f) {
				anim.SetTrigger("Land");
			}
            if (collision.collider.tag.Equals("Overworld"))
                Sanity.SetDepleteSanity(false);
            else if (collision.collider.tag.Equals("Cave"))
                Sanity.SetDepleteSanity(true);
        }

        if (collision.collider.tag.Equals("Gem") && heldGem == null)
        {
            Destroy(collision.collider.gameObject);
            heldGem = collision.collider.gameObject.name;
            hud.UpdateGemIcon(heldGem);
        }

        GameObject.Find("ActionBar").GetComponent<ActionBarHandler>().checkCollision(collision.collider);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Heart Container"))
        {
            Destroy(collider.gameObject);
            health.AddHeart();
        }
        else if (collider.name.Equals("ChestTrigger"))
        {
            hud.OpenCollection(collectedGems);
            if (heldGem != null)
            {
                StartCoroutine(NewGemAnim());
            }
        }
    }

    private IEnumerator NewGemAnim()
    {
        disableMovement();
        string gem = DropGem();
        hud.UpdateGemIcon(heldGem);
        StartCoroutine(hud.AddGemToCollection(gem));
        yield return new WaitForSeconds(1.5f);
        collectedGems[numCollected] = gem;
        numCollected++;
        enableMovement();
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.name.Equals("ChestTrigger"))
        {
            hud.CloseCollection();
        }
    }

    public string DropGem()
    {
        string gem = heldGem;
        heldGem = null;
        return gem;
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

		//Send Parameters to Animator
		anim.SetFloat("HorizontalSpeed",Mathf.Abs(rigidbody.velocity.x)/10);

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
		this.headLight.transform.Rotate(0,180,0);
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

    public static void ResetAllValues()
    {
        heldGem = null;
        numCollected = 0;
        collectedGems = new string[8];
    }

    private IEnumerator Death()
    {
		anim.SetBool("isDead", true);
        disableMovement();
        events.ShakeCamera(0.5f, 3f);
        yield return new WaitForSeconds(5f);
        events.TorchSweepOut(0.5f);
        yield return new WaitForSeconds(7f);
        ResetAllValues();
        Health.ResetAllValues();
        Sanity.ResetAllValues();
        Application.LoadLevel("Title Screen");
    }
}
