using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BatController : MonoBehaviour {

	//Bool Flags
	bool shouldFly = false;
	bool isLanded = false;
	bool isInWall = false;

	//Radi
	float neighborRadius = 4.0f; // Bats that affect this bat's behvior are within this radius
	float desiredSeparationRadius = 2.0f; // The separation the bat will try to achieve when in a flock
	float playerAvoidanceRadius = 2.0f; // How far away the bats will try to stay away from the player
	float shouldLandRadius = 20.0f; // Bats that are further than this radius away from the player will land on the nearest
									// ceiling tile. This prevents offscreen bats from taking processing power.
	
	//Scaling for how much each aspect affects the overall course of the bat
	float totalForceScale = 25.0f;
	float cohesionScale = 1.5f;
	float separationScale = 2.0f;
	float alignmentScale = 1.0f;
	float wallAvoidanceScale = 6.0f;
	float playerAvoidanceScale = 2.0f;
	float individualismScale = 5.0f;

	//Sounds
	private AudioSource audioSource;
	float squeakProbability = 0.01f; //Probability (0.0-1.0) of a bat squeaking when near the player (higher = more frequent squeaking)
	

	//Reference Variables
	public GameObject mapGenerator;
	public GameObject player;
	public List<Transform> bats; //A list of all the bats in the scene
	Rigidbody2D rigidBody2D;
	
	Vector2 defaultForce; //Used in calculating the bat's individualism force. 
	Vector2 landingSpot; //Where the bat should try to land when its far from the player
	

	

	
	void Start () {
		player = GameObject.Find ("Player");
		audioSource = GetComponent<AudioSource>();
		
		rigidBody2D = GetComponent<Rigidbody2D>();
		//rigidBody2D.velocity = GetInitialVelocity();
		
		mapGenerator = GameObject.Find("Map Generator");
		
		SetInitialDefaultForce();
		RandomizeInitialPosition();
		
		SetBats ();
	}
	
	void SetBats() {
		Transform batParent = GameObject.Find("Bats").transform;
		foreach (Transform bat in batParent) {
			if (bat != this.transform) {
				this.bats.Add (bat);
			}
		}
	}
	
	Vector2 SetInitialDefaultForce() {
		return new Vector2(UnityEngine.Random.Range(-100.0f,101.0f)/(float)10,UnityEngine.Random.Range(-100.0f,101.0f)/(float)10);
	}
	
	void RandomizeInitialPosition() {
		Vector2 randOffset = new Vector2((UnityEngine.Random.value-0.5f),(UnityEngine.Random.value-0.5f));;
		Vector2 newPosition = (Vector2)this.transform.position + randOffset;
		this.transform.position = newPosition;
	}
	
	Vector2 AverageVectors(List<Vector2> vectors) {
		float sumX = 0;
		float sumY = 0;
		int count = vectors.Count;
		foreach (Vector2 vector in vectors) {
			sumX += vector.x;
			sumY += vector.y;
		}
		return new Vector2(sumX/count,sumY/count);
	}
	
	List<Vector2> GetPositionVectors(List<Transform> transforms) {
		List<Vector2> v = new List<Vector2>();
		foreach (Transform t in transforms) {
			v.Add(t.position);
		}
		return v;
	}
	List<Vector2> GetVelocityVectors(List<Transform> transforms) {
		
		List<Vector2> v = new List<Vector2>();
		foreach (Transform t in transforms) {
			Rigidbody2D rb = t.gameObject.GetComponent<Rigidbody2D>();
			v.Add(rb.velocity);
		}
		return v;
	}
	
	List<Transform> GetNeighbors(float radius) {
		List<Transform> neighbors = new List<Transform>();
		foreach (Transform bat in this.bats) {
			float distance = Vector2.Distance(bat.position,this.transform.position);
			if (distance <= radius) {
				neighbors.Add (bat);
			}
		}
		return neighbors;
	}
	
	Vector2 GetCohesion(List<Transform> neighbors) {
		if (neighbors.Count > 0) {
			Vector2 averageVector = AverageVectors(GetPositionVectors(neighbors));
			Vector2 newVector = averageVector - (Vector2)this.transform.position;
			newVector.Normalize();
			return newVector*cohesionScale;
		}
		else {
			return Vector2.zero;
		}
	}
	
	Vector2 GetAlignment(List<Transform> neighbors) {
		if (neighbors.Count > 0) {
			Vector2 averageVector = AverageVectors(GetVelocityVectors(neighbors));
			averageVector.Normalize();
			return averageVector*alignmentScale;
		}
		else {
			return Vector2.zero;
		}
	}
	
	Vector2 GetSeparation(List<Transform> neighbors) {
		if (neighbors.Count > 0) {
			Vector2 averageVector = AverageVectors(GetPositionVectors(neighbors));
			Vector2 newVector =  (Vector2)this.transform.position - averageVector;
			newVector.Normalize();
			return newVector*separationScale;
		}
		else {
			return Vector2.zero;
		}
	}
	
	public void DrawForceVector(Vector2 force, Color color) {
		Vector2 start = (Vector2)this.transform.position;
		Vector2 end = start + force;
		Debug.DrawLine(start,end,color);
	}
	
	Vector2 GetWallAvoidance() {
		MapGenerator mapGenScript = this.mapGenerator.GetComponent<MapGenerator>();
		if (mapGenScript.IsInWall(this.transform.position)) {
			
			if (isInWall == false) {
				this.defaultForce = this.defaultForce*-1;
				this.rigidBody2D.velocity = this.rigidBody2D.velocity/2.0f;
			}
			
			isInWall = true;
			
			Vector2 newDestination = mapGenScript.GetEscapeRoute(this.transform.position);
			Vector2 forceVector = newDestination-(Vector2)this.transform.position;
			return forceVector*wallAvoidanceScale;
		}
		else {
			this.isInWall = false;
			return Vector2.zero;
		}
	}
	
	Vector2 GetPlayerAvoidance() {
		
		float distance = Vector2.Distance(this.player.transform.position,this.transform.position);
		
		if (distance <= this.playerAvoidanceRadius) {
			Vector2 force =  (Vector2)this.transform.position - (Vector2)this.player.transform.position;
			force.Normalize();
			return force*playerAvoidanceScale;
		}
		else {
			return Vector2.zero;
		}
	}
	
	Vector2 GetIndividualismForce() {
		Vector2 randForce = new Vector2((UnityEngine.Random.value-0.5f)/10,(UnityEngine.Random.value-0.5f)/10);
		defaultForce = this.defaultForce + randForce;
		defaultForce.Normalize();
		defaultForce = this.defaultForce/10;
		return defaultForce*individualismScale;
	}
	
	Vector2 GetNewForce() {
		List<Transform> neighbors = GetNeighbors(neighborRadius);
		List<Transform> crowdingNeigbors = GetNeighbors(desiredSeparationRadius);
		
		Vector2 cohesion = GetCohesion(neighbors); 
		Vector2 alignment = GetAlignment(neighbors);
		Vector2 separation = GetSeparation(crowdingNeigbors); //Adding more weight to the separation
		
		Vector2 wallAvoidance = GetWallAvoidance();
		Vector2 playerAvoidance = GetPlayerAvoidance();
		Vector2 individualismForce = GetIndividualismForce();
		
		DrawForceVector(cohesion,Color.green);
		DrawForceVector(alignment,Color.yellow);
		DrawForceVector(separation,Color.red);
		DrawForceVector(wallAvoidance,Color.magenta);
		DrawForceVector(playerAvoidance,Color.blue);
		DrawForceVector(individualismForce,Color.white);
		
		List<Vector2> vectors = new List<Vector2>();
		vectors.Add (cohesion);
		vectors.Add (alignment);
		vectors.Add (separation);
		vectors.Add(wallAvoidance);
		vectors.Add (playerAvoidance);
		vectors.Add (individualismForce);
		
		Vector2 finalVector = AverageVectors(vectors);
		DrawForceVector(finalVector,Color.cyan);
		return finalVector*this.totalForceScale;
	}
	
	bool IsNearPlayer() {
		float distance = Vector2.Distance(this.player.transform.position,this.transform.position);
		
		if (distance <= this.shouldLandRadius) {
			return true;
		}
		else {
			return false;
		}
	}
	
	void FindClosestLandingSpot() {
		MapGenerator mapGenScript = this.mapGenerator.GetComponent<MapGenerator>();
		this.landingSpot = mapGenScript.GetLandingSpot(this.transform.position);
	}
	
	bool IsInLandingSpot() {
		float distance = Vector2.Distance(this.landingSpot,this.transform.position);
		if (distance < 0.1) {
			return true;
		}
		else {
			return false;
		}
	}
	
	void MoveToLandingSpot() {
		Vector2 force = (Vector2)this.landingSpot-(Vector2)this.transform.position;
		DrawForceVector(force,Color.white);
		this.rigidBody2D.AddForce(force);
	}

	void TrySqueak() {
		if (this.audioSource.isPlaying == false) {
			float random = UnityEngine.Random.value; // 0.0 - 1.0
			if (random <= this.squeakProbability) {
				this.audioSource.Play();
			}
		}
	}

	void Update() {
		TrySqueak();
	}

	void FixedUpdate() {
		
		if (IsNearPlayer()) {
			shouldFly = true;
		}
		else {
			shouldFly = false;
			FindClosestLandingSpot();
		}
		
		if (shouldFly) {
			Vector2 force = GetNewForce();
			rigidBody2D.AddForce(force);
		}
		else {
			if (IsInLandingSpot()) {
				isLanded = true;
				this.rigidBody2D.velocity = Vector2.zero;
			}
			else {
				isLanded = false;
				MoveToLandingSpot();
			}
		}
		
		
		
	}

}
