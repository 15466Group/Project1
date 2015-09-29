using UnityEngine;
using System.Collections;

[System.Serializable]
public class Behavior : MonoBehaviour {

	//weight obstacle forces more than character forces so they don't go through objects
	//make characters go towards the goal even if there's a raycasthit if the raycasthit.distance <= distance to goal
	//goal close to wall, he stops moving once he reaches it, then goal moves, and character still stays still -- change with case where goal is close to object
	//goal outside the boundaries, character moves right through walls -- change with accmag

	//not go through walls, balance btwn accMag and speedMax

	public GameObject goal;
	private Animation anim;
	public Vector3 velocity { get; set; }
	public Vector3 acceleration { get; set; }
	public float accMag { get; set; }
	public float accMagDefault { get; set; }
	public float speedMaxDefault { get; set; }
	public float speedMax { get; set; }
	public Vector3 target { get; set; }
	public float epsilon { get; set; }
	public float searchRadius { get; set; }
	public Vector3 targetPosition { get; set; }
	public float rayDist { get; set; }
	public float rayDistDefault { get; set; }
	public float closeRayDist { get; set; }
	public float closeRayDistDefault { get; set; }

	private float walkingSpeed;
	private float charWidth;
	private float smooth;
	private float obstacleWeight;
	private float charWeight;

	// Use this for initialization
	public virtual void Start () {
		velocity = new Vector3 ();
		acceleration = new Vector3 ();
		accMagDefault = 50.0f;
		speedMaxDefault = 20.0f;
		walkingSpeed = 15.0f;
		epsilon = 2.0f;
		searchRadius = 100.0f;
		rayDistDefault = 20.0f;
		rayDist = rayDistDefault;
		closeRayDistDefault = 5.0f;
		closeRayDist = closeRayDistDefault;
		charWidth = 5.0f;
		smooth = 5.0f;
		obstacleWeight = 3.0f;
		charWeight = 1.0f;
		accMag = accMagDefault;
		speedMax = speedMaxDefault;
		anim = GetComponent<Animation> ();
		anim.CrossFade ("idle");
	}
	
	// Update is called once per frame
	public virtual void Update () {
		//want him to accelerate towards the target.position
//		Debug.Log (acceleration);
		transform.position += velocity * Time.deltaTime;
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, speedMax);
//		Debug.Log (speedMax);
		//update speedMax
		veloCloseToTarget ();
		targetPosition = transform.position + velocity * Time.deltaTime;
//		target = goal.transform.position;
		if (velocity != new Vector3())
			RotateTo (targetPosition);
		float targetDist = Vector3.Distance (transform.position, target);
		rayDist = Mathf.Min (rayDistDefault, targetDist);
		closeRayDist = Mathf.Min (closeRayDistDefault, targetDist);
		acceleration = calculateAcceleration (target);
		acceleration = new Vector3 (acceleration.x, 0.0f, acceleration.z).normalized * accMag;

		float mag = velocity.magnitude;
		if (mag > 0.0f && mag <= walkingSpeed) {
			anim.CrossFade ("Walk");
		} else if (mag > walkingSpeed) {
			anim.CrossFade ("Run");
		} else {
			anim.CrossFade("idle");
		}
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
		transform.rotation = Quaternion.Slerp (transform.rotation, destinationRotation, Time.deltaTime * smooth);
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
			//accumulator += (target - transform.position).normalized;
			accumulator += transform.forward.normalized;
			return accumulator.normalized * accMag;
		} else {
			return (target - transform.position).normalized * accMag;
		}
	}*/
	
	Vector3 checkCloseCalls(Vector3 acceleration) {
		accMag = accMagDefault;
		float weight = charWeight;
		Collider[] hits = Physics.OverlapSphere (transform.position, closeRayDist);
		//don't include hitting ground and self
		if (hits.Length > 2) {
			accMag = 500.0f;
			Vector3 accumulator = new Vector3 ();
			foreach (Collider obstacle in hits) {
				if (obstacle.gameObject != this && obstacle.gameObject.name != "Ground") {
					if (!obstacle.gameObject.name.Contains("samuz")){
						weight = obstacleWeight;
					} else {
						weight = charWeight;
					}
					RaycastHit hit;
					RaycastHit hitN;
					Physics.Raycast (transform.position, obstacle.transform.position - transform.position, out hit, rayDist);
					Vector3 normal = hit.normal;
					bool closest = Physics.Raycast (transform.position, normal * (-1.0f), out hitN, rayDist);
					if (closest) {
						accumulator += hitN.normal.normalized * (hitN.distance * weight / 10.0f);
					} else {
						accumulator += (transform.position - obstacle.transform.position).normalized * (hit.distance * weight / 10.0f);
					}
				}
			}
			Vector3 accel = accumulator.normalized * accMag;
			return accel;
		} else {
			return acceleration;
		}
	}
	
	public virtual Vector3 calculateAcceleration(Vector3 target) {
		RaycastHit hitL;
		RaycastHit hitR;
		RaycastHit hitC;
		float weight = charWeight;
		
		bool hitLeft = Physics.Raycast (transform.position - transform.right.normalized * charWidth, transform.forward, out hitL, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right.normalized * charWidth, transform.forward, out hitR, rayDist);
		bool hitCenter = Physics.Raycast (transform.position, transform.forward, out hitC, rayDist);

		float distToTarget = Vector3.Distance (transform.position, target);
		if (distToTarget < hitL.distance && distToTarget < hitR.distance && distToTarget < hitC.distance) {
			//fixme
//			return checkCloseCalls((target - transform.position).normalized * accMag);
			return (target - transform.position).normalized * accMag;
		}
		
		bool hitFront = hitLeft || hitRight;
		Debug.DrawRay(transform.position,transform.forward * 50.0f,Color.red);
		if (hitRight || hitLeft) {
//			Debug.Log ("hitright || hitLeft");
			Collider[] hits = Physics.OverlapSphere (transform.position, rayDist);
			Vector3 accumulator = new Vector3 ();
			foreach (Collider obstacle in hits) {
				if (obstacle.gameObject != this && obstacle.gameObject.name != "Ground") {
					if (!obstacle.gameObject.name.Contains("samuz")){
						weight = obstacleWeight;
					} else {
						weight = charWeight;
					}
					RaycastHit hit;
					RaycastHit hitN;
					Physics.Raycast (transform.position, obstacle.transform.position - transform.position, out hit, rayDist);
					Vector3 normal = hit.normal;
					bool closest = Physics.Raycast (transform.position, normal * (-1.0f), out hitN, rayDist);
					if (closest) {
						accumulator += hitN.normal.normalized * (hitN.distance * weight / 10.0f);
					} else {
						accumulator += (transform.position - obstacle.transform.position).normalized * (hit.distance * weight / 10.0f);
					}
				}
			}
			accumulator += transform.forward.normalized;
			//return accumulator.normalized * accMag;
			return checkCloseCalls(accumulator.normalized * accMag);
		}
		else {
//			Debug.Log ("else");
			Vector3 potentialAccel = (target - transform.position).normalized * accMag;
			Vector3 potentialVeloc = velocity + potentialAccel * Time.deltaTime;
			//bool potentialHit = Physics.Raycast (transform.position, potentialVeloc, potentialVeloc.magnitude);
			//bool potentialHit = Physics.Raycast (transform.position, target - transform.position, Mathf.Min (potentialVeloc.magnitude, Vector3.Distance(goal.transform.position, transform.position)));
			bool potentialHit = Physics.Raycast (transform.position, target - transform.position, rayDist);
			if(potentialHit) {
				//return transform.forward.normalized * accMag;
				return checkCloseCalls(transform.forward.normalized * accMag);
			}
			else {
				//return (target - transform.position).normalized * accMag;
				return checkCloseCalls((target - transform.position).normalized * accMag);
			}
		}
		//return (target - transform.position).normalized * accMag;
//		return checkCloseCalls((target - transform.position).normalized * accMag);
	}
	
	void veloCloseToTarget () {
		float epsilon = 0.7f;
		float xDistance = Mathf.Abs (transform.position.x - target.x);
		float zDistance = Mathf.Abs (transform.position.z - target.z);
		float distance = Mathf.Sqrt (xDistance * xDistance + zDistance * zDistance);
		float d = Vector3.Distance (transform.position, target);
//		Debug.Log ("DISTANCE: " + distance + " d: " + d);
		if (distance <= epsilon) {
			speedMax = 0.0f; 
		}
		else { //exponential growth translated up by 10, capped at originalMaxSpeed
			//fixme
			speedMax = Mathf.Min(Mathf.Pow(1.1f,distance) + 10.0f, speedMaxDefault);
		}
	}
}
