using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float jump = 10.0f;
	public float speed = 10.0f;
	public bool moonWalk = false;

	private bool facingRight = true;

	Animator animator;
	Rigidbody2D rigidbody;
	float velocity;
	
	void Start () {
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D> ();

	}
	
	void Update () {
		velocity = Input.GetAxis("Horizontal");

	}

	void Flip() {
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	

	void FixedUpdate() {

		if (Input.GetKeyDown(KeyCode.Space)) {
			Vector3 currentVelocity = rigidbody.velocity;
			rigidbody.velocity = new Vector3(currentVelocity.x,jump, currentVelocity.z);
		}

		if (Input.GetKeyDown(KeyCode.M)) {
			moonWalk = !moonWalk;
		}

		Vector3 cVelocity = rigidbody.velocity;
		rigidbody.velocity = new Vector3(velocity * speed,cVelocity.y, cVelocity.z);
		//rigidbody.MovePosition (rigidbody.position + velocity * Time.fixedDeltaTime);

		if (moonWalk) {
			if (!facingRight && velocity < 0) {
				Flip ();
			}
			else if (facingRight && velocity > 0) {
				Flip ();
				
			}
		}
		else {

			if (facingRight && velocity < 0) {
				Flip ();
			}
			else if (!facingRight && velocity > 0) {
				Flip ();
				
			}
		}

		animator.SetFloat("HorizontalSpeed", Mathf.Abs(rigidbody.velocity.x));
	}

}