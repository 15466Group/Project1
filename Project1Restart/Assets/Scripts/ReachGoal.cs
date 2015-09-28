using UnityEngine;
using System.Collections;

public class ReachGoal: MonoBehaviour {

	public GameObject goal;
	private Vector3 velocity;
	private Vector3 acceleration;
	private float accMag;
	private float accMagDefault = 50.0f;
	private float speedMaxDefault = 20.0f;
	private float speedMax;
	private Vector3 target;
	private float epsilon = 5.0f;
	private float searchRadius = 100.0f;
	private Vector3 targetPosition;
	private float rayDist = 20.0f;

	// Use this for initialization
	void Start () {
		velocity = new Vector3 ();
		acceleration = new Vector3 ();
		accMag = accMagDefault;
		speedMax = speedMaxDefault;
		target = goal.transform.position;
		acceleration = calculateAcceleration ();
	}

	void Update () {
		//want him to accelerate towards the target.position
		Debug.Log (acceleration);
		transform.position += velocity * Time.deltaTime;
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, speedMax);
		Debug.Log (speedMax);
		//veloCloseToTarget ();
		targetPosition = transform.position + velocity * Time.deltaTime;
		target = goal.transform.position;
		if (velocity != new Vector3())
			RotateTo (targetPosition);
		acceleration = calculateAcceleration ();
		acceleration = new Vector3 (acceleration.x, 0.0f, acceleration.z).normalized * accMag;

	}
	
	void RotateTo(Vector3 targetPosition){
		//maxDistance is the maximum ray distance
		Quaternion destinationRotation;
		Vector3 relativePosition;
		relativePosition = targetPosition - transform.position;
		Debug.DrawRay(transform.position,relativePosition*10,Color.red);
		Debug.DrawRay(transform.position,velocity.normalized*20,Color.green);
		Debug.DrawRay(transform.position,acceleration.normalized*10,Color.blue);
		destinationRotation = Quaternion.LookRotation (relativePosition);
		transform.rotation = Quaternion.Slerp (transform.rotation, destinationRotation, Time.deltaTime * 5.0f);
	}

	/*Vector3 calculateAcceleration() {
		Collider[] hits = Physics.OverlapSphere (transform.position, rayDist);
		if (hits.Length > 2) {
			Vector3 accumulator = new Vector3 ();
			foreach (Collider obstacle in hits) {
				if (obstacle.gameObject != this && obstacle.gameObject.name != "Ground") {
					RaycastHit hit;
					RaycastHit hitN;
					Physics.Raycast (transform.position, obstacle.transform.position - transform.position, out hit, rayDist);
					Vector3 normal = hit.normal;
					bool closest = Physics.Raycast (transform.position, normal * (-1.0f), out hitN, rayDist);
					if (closest) {
						accumulator += hitN.normal.normalized * (hitN.distance / rayDist);
					} else {
						accumulator += (transform.position - obstacle.transform.position).normalized * (hit.distance / rayDist);
					}
				}
			}
			//accumulator += (goal.transform.position - transform.position).normalized;
			accumulator += transform.forward.normalized;
			return accumulator.normalized * accMag;
		} else {
			return (goal.transform.position - transform.position).normalized * accMag;
		}
	}*/

	Vector3 checkCloseCalls(Vector3 acceleration) {
		Collider[] hits = Physics.OverlapSphere (transform.position, 5.0f);
		if (hits.Length > 2) {
			Vector3 accumulator = new Vector3 ();
			foreach (Collider obstacle in hits) {
				if (obstacle.gameObject != this && obstacle.gameObject.name != "Ground") {
					RaycastHit hit;
					RaycastHit hitN;
					Physics.Raycast (transform.position, obstacle.transform.position - transform.position, out hit, rayDist);
					Vector3 normal = hit.normal;
					bool closest = Physics.Raycast (transform.position, normal * (-1.0f), out hitN, rayDist);
					if (closest) {
						accumulator += hitN.normal.normalized * (hitN.distance / 10.0f);
					} else {
						accumulator += (transform.position - obstacle.transform.position).normalized * (hit.distance / 10.0f);
					}
				}
			}
			return accumulator.normalized * accMag;
		} else {
			return acceleration;
		}
	}

	Vector3 calculateAcceleration() {
		RaycastHit hitL;
		RaycastHit hitR;
		RaycastHit hitC;

		bool hitLeft = Physics.Raycast (transform.position - transform.right.normalized * 5.0f, transform.forward, out hitL, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right.normalized * 5.0f, transform.forward, out hitR, rayDist);
		bool hitCenter = Physics.Raycast (transform.position, transform.forward, out hitC, rayDist);



		bool hitFront = hitLeft || hitRight;
		Debug.DrawRay(transform.position,transform.forward * 50.0f,Color.red);
		if (hitRight || hitLeft) {
			Collider[] hits = Physics.OverlapSphere (transform.position, rayDist);
			Vector3 accumulator = new Vector3 ();
			foreach (Collider obstacle in hits) {
				if (obstacle.gameObject != this && obstacle.gameObject.name != "Ground") {
					RaycastHit hit;
					RaycastHit hitN;
					Physics.Raycast (transform.position, obstacle.transform.position - transform.position, out hit, rayDist);
					Vector3 normal = hit.normal;
					bool closest = Physics.Raycast (transform.position, normal * (-1.0f), out hitN, rayDist);
					if (closest) {
						accumulator += hitN.normal.normalized * (hitN.distance / 10.0f);
					} else {
						accumulator += (transform.position - obstacle.transform.position).normalized * (hit.distance / 10.0f);
					}
				}
			}
			accumulator += transform.forward.normalized;
			//return accumulator.normalized * accMag;
			return checkCloseCalls(accumulator.normalized * accMag);
		}
		else {
			Vector3 potentialAccel = (goal.transform.position - transform.position).normalized * accMag;
			Vector3 potentialVeloc = velocity + potentialAccel * Time.deltaTime;
			//bool potentialHit = Physics.Raycast (transform.position, potentialVeloc, potentialVeloc.magnitude);
			//bool potentialHit = Physics.Raycast (transform.position, goal.transform.position - transform.position, Mathf.Min (potentialVeloc.magnitude, Vector3.Distance(goal.transform.position, transform.position)));
			bool potentialHit = Physics.Raycast (transform.position, goal.transform.position - transform.position, rayDist);
			if(potentialHit) {
				//return transform.forward.normalized * accMag;
				return checkCloseCalls(transform.forward.normalized * accMag);
			}
			else {
				//return (goal.transform.position - transform.position).normalized * accMag;
				return checkCloseCalls((goal.transform.position - transform.position).normalized * accMag);
			}
		}
		//return (goal.transform.position - transform.position).normalized * accMag;
		return checkCloseCalls((goal.transform.position - transform.position).normalized * accMag);
	}

	void veloCloseToTarget () {
		float epsilon = 0.7f;
		float xDistance = Mathf.Abs (transform.position.x - goal.transform.position.x);
		float zDistance = Mathf.Abs (transform.position.z - goal.transform.position.z);
		float distance = Mathf.Sqrt (xDistance * xDistance + zDistance * zDistance);
		
		if (distance <= epsilon) {
			speedMax = 0.0f; 
		}
		else { //exponential growth translated up by 10, capped at originalMaxSpeed
			//fixme
			speedMax = Mathf.Min(Mathf.Pow(1.1f,distance) + 10.0f, speedMax);
		}
	}
}