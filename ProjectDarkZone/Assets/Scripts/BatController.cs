using UnityEngine;
using System.Collections;

public class BatController : MonoBehaviour {

	public float speed = 0.06f;
	public float scareDistance = 15.0f;
	public GameObject player;

	private AudioSource audioSource;



	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		float distance = Vector2.Distance(this.transform.position, player.transform.position);

		if (distance <= scareDistance) {
			float dx1 = this.transform.position.x - player.transform.position.x;
			float dy1 = this.transform.position.y - player.transform.position.y;
			float angle = Mathf.Atan(dx1/dy1);
			float dx2 = speed*Mathf.Sin(angle);
			float dy2 = speed*Mathf.Cos (angle);

			dx2 = dx1 > 0 ? -dx2:dx2;
			dy2 = dy1 < 0 ? -dy2:dy2;

			transform.position = new Vector2(transform.position.x + dx2, transform.position.y + dy2);

		}
		else {
			//Random walk
			float randX = Random.Range(-speed,speed);
			float randY = Random.Range(-speed,speed);
			transform.position = new Vector2(transform.position.x + randX, transform.position.y + randY);
		}

		if (distance <= 20 && this.audioSource != null) {
			if (this.audioSource.isPlaying == false) {
				this.audioSource.PlayOneShot(this.audioSource.clip,this.audioSource.volume);
			}
		}

	}
}
