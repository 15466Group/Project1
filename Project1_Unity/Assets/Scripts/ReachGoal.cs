using UnityEngine;
using System.Collections;

public class ReachGoal : MonoBehaviour {

	public GameObject goal;
	public Animation anim;

	private Vector3 acceleration;
	private Vector3 velocity;
	private float accMag;
	private float maxSpeed;
	private float originalMaxSpeed;

	private Vector3 targetAccel;
	private Vector3 targetPosition;

	private float idleSpeed;
	private float walkingSpeed;

	private float maxRadsDelta;
	private float maxMagDelta;

	private float smooth;

	private float charWidth;
	private float rayDist;


	private Vector3 tempGoal;

	// Use this for initialization
	void Start () {

		//as accMag increases, accelerates faster
		accMag = 500.0f;
		velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		acceleration = new Vector3 (0.0f, 0.0f, 0.0f);
		targetAccel = new Vector3 (0.0f, 0.0f, 0.0f);
		targetPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		maxMagDelta = 100.0f;
		//as maxRadsDelta increases, guy turns towards goal faster
		maxRadsDelta = Mathf.Deg2Rad * 10.0f;
		idleSpeed = 0.0f;
		anim.CrossFade ("idle");
		smooth = 5.0f;
		walkingSpeed = 20.0f;
		maxSpeed = 20.0f;
		originalMaxSpeed = maxSpeed;
		anim.CrossFade ("idle");
		charWidth = 5.0f;
		rayDist = 15.0f;
	}
	
	// Update is called once per frame
	void Update () {
		//want him to accelerate towards the target.position
		transform.position += velocity * Time.deltaTime;
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
		//velocity = veloCloseToTarget (velocity);
		veloCloseToTarget ();
		
		acceleration = Vector3.RotateTowards (acceleration, targetAccel, maxRadsDelta, maxMagDelta);
		targetPosition = transform.position + velocity * Time.deltaTime;
		if (velocity != new Vector3())
			RotateTo (targetPosition);


//		targetAccel = new Vector3 (target.transform.position.x - transform.position.x, 0.0f,target.transform.position.z - transform.position.z);
//		targetAccel = new Vector3 (transform.position.x - target.transform.position.x, 0.0f, transform.position.z - target.transform.position.z);
//		targetAccel = collisions (targetAccel);
//		targetAccel = avoidObjects (targetAccel);

		targetAccel = calculateAcceleration ();
		targetAccel = new Vector3 (targetAccel.x, 0.0f, targetAccel.z);
		targetAccel = targetAccel.normalized;
		targetAccel = targetAccel * accMag;

		float mag = velocity.magnitude;
		if (mag <= walkingSpeed && mag > idleSpeed) {
			anim.CrossFade ("Walk");
		} else if (mag > walkingSpeed) {
			anim.CrossFade ("Run");
		} else {
			anim.CrossFade ("idle");
		}
	}

	void RotateTo(Vector3 targetPosition){
		//maxDistance is the maximum ray distance
		Quaternion destinationRotation;
		Vector3 relativePosition;
		relativePosition = targetPosition - transform.position;
		Debug.DrawRay(transform.position,relativePosition*10,Color.red);
		Debug.DrawRay(transform.position,targetAccel.normalized*30,Color.red);
		Debug.DrawRay(transform.position,velocity.normalized*20,Color.green);
		Debug.DrawRay(transform.position,acceleration.normalized*10,Color.blue);
		//Debug.DrawRay(transform.position - transform.right*charWidth, transform.forward.normalized * rayDist, Color.yellow);
		//Debug.DrawRay(transform.position + transform.right*charWidth, transform.forward.normalized * rayDist, Color.yellow);
		//Debug.DrawRay(transform.position - transform.right*charWidth, (transform.forward - transform.right).normalized * 1.5f * rayDist, Color.yellow);
		//Debug.DrawRay(transform.position + transform.right*charWidth, (transform.forward + transform.right).normalized * 1.5f * rayDist, Color.yellow);
		//Debug.DrawRay(transform.position, (target.transform.position - transform.position) * 50.0f, Color.yellow);
		destinationRotation = Quaternion.LookRotation (relativePosition);
		transform.rotation = Quaternion.Slerp (transform.rotation, destinationRotation, Time.deltaTime * smooth);
	}

	void veloCloseToTarget () {
		float epsilon = 0.7f;
		float xDistance = Mathf.Abs (transform.position.x - goal.transform.position.x);
		float zDistance = Mathf.Abs (transform.position.z - goal.transform.position.z);
		float distance = Mathf.Sqrt (xDistance * xDistance + zDistance * zDistance);

		if (distance <= epsilon) {
			maxSpeed = 0.0f; 
		}
		else { //exponential growth translated up by 10, capped at originalMaxSpeed
			//fixme
			maxSpeed = Mathf.Min(Mathf.Pow(1.1f,distance) + 10.0f, originalMaxSpeed);
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (transform.position, rayDist);
	}



	/*
	Vector3 calculateAcceleration() {
		rayDist = 30.0f;
		rayDist = Mathf.Min (rayDist, (goal.transform.position - transform.position).magnitude);
		Collider[] hits = Physics.OverlapSphere (transform.position, rayDist);
		Vector3 accel = new Vector3 (goal.transform.position.x - transform.position.x, 0.0f,goal.transform.position.z - transform.position.z);

		foreach (Collider obstacle in hits) {
			if (obstacle.gameObject != this.gameObject && obstacle.gameObject.name != "Ground") {
				Debug.Log (obstacle.gameObject.name);
				RaycastHit hit;
				Vector3 closest = obstacle.ClosestPointOnBounds(transform.position);
				bool hitObstacle = Physics.Raycast (transform.position, closest - transform.position, out hit, rayDist);
				//bool hitObstacle = Physics.Raycast (transform.position, transform.forward, out hit, rayDist);
				Debug.DrawRay (transform.position, (closest - transform.position) * rayDist, Color.yellow);
				Vector3 normal = hit.normal.normalized * accMag;
				float obstacleDist = hit.distance;
				//float obstacleDist = Vector3.Distance (obstacleLoc, transform.position);
				//maxRadsDelta = Mathf.Deg2Rad * 20.0f * (1.0f - (obstacleDist/rayDist));
				if (obstacleDist < Vector3.Distance (goal.transform.position, transform.position)) {
					accel = accel * obstacleDist / rayDist + normal * (1.0f - (obstacleDist / rayDist));
					//accel = transform.forward * accMag * obstacleDist/rayDist - normal * (1.0f - (obstacleDist/rayDist));
					//accel = accel * obstacleDist/rayDist + transform.right * accMag * (1.0f - (obstacleDist/rayDist));
					Debug.DrawRay (hit.point, normal * (1.0f - (obstacleDist / rayDist)));
					accel = accel.normalized * accMag;
				}
			}
		}
		return accel;
	}
	*/

	Vector3 calculateAcceleration() {
		rayDist = 40.0f;
		rayDist = Mathf.Min (rayDist, (goal.transform.position - transform.position).magnitude);
		Collider[] hits = Physics.OverlapSphere (transform.position, rayDist);
		Vector3 accel = new Vector3 (goal.transform.position.x - transform.position.x, 0.0f,goal.transform.position.z - transform.position.z);
		accel = accel.normalized * accMag;
		RaycastHit hitD;
		bool hitLeft = Physics.Raycast (transform.position - transform.right*charWidth, transform.forward, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right*charWidth, transform.forward, rayDist);
		bool hitForward = hitLeft || hitRight;
		bool hitDirect = Physics.Raycast (transform.position, goal.transform.position - transform.position, out hitD, rayDist);

		if (!hitForward && !hitDirect) {
			Debug.Log (hitForward + " " + hitDirect);
			Debug.Log ("inhere1");
			return accel;
		} else if (!hitForward && hitDirect) {
			Debug.Log (hitD.collider.name);
			Debug.Log (hitForward + " " + hitDirect);
			Debug.Log ("inhere2");
			return transform.forward.normalized * accMag;
		} else {
			accel = accel.normalized;

			if (hits.Length == 2) {
				Debug.Log ("asdfasdfadsfadsfadsf");
				return accel.normalized * accMag;
			}
			Vector3 accumulate = new Vector3 (0.0f, 0.0f, 0.0f);
			foreach (Collider obstacle in hits) {
				if (obstacle.gameObject != this.gameObject && obstacle.gameObject.name != "Ground") {
					RaycastHit hit;
					RaycastHit hitClose;
					Vector3 groundPosition = new Vector3 (obstacle.transform.position.x, 0.0f, obstacle.transform.position.z);
					Physics.Raycast (transform.position, groundPosition - transform.position, out hit, rayDist);
					float dist;
					Vector3 normal;
					Physics.Raycast (transform.position, hit.normal * (-1.0f), out hitClose, rayDist);
					dist = hitClose.distance;
					normal = hitClose.normal.normalized;
					Debug.DrawRay (transform.position, (hitClose.point - transform.position).normalized * dist, Color.white);
					Debug.DrawRay (hitClose.point, normal * rayDist, Color.white);
					Debug.Log (hit.normal);
					//Debug.DrawRay (transform.position, hit.normal * (-1.0f), Color.white);
					//float dist = hitClose.distance;
					Vector3 temp;
					Debug.Log ("number of close objects: " + (hits.Length - 2));

					//Vector3 normal = hitClose.normal.normalized;
					Debug.Log (hitClose.normal);
					//Debug.DrawRay (transform.position, (hitClose.point - transform.position).normalized * dist, Color.white);
					//Debug.DrawRay (hitClose.point, normal * rayDist, Color.white);
					temp = (accel * dist / rayDist) + (normal * (1.0f - (dist / rayDist)));
					//Debug.Log ((accel * dist/rayDist));
					//Debug.Log (normal * (1.0f - (dist/rayDist)));
					accumulate = accumulate + temp;

				}


			}
		
			Debug.Log (originalMaxSpeed);
			if (accumulate == new Vector3 ()) {
				accumulate = accel;
			}
			return accumulate = accumulate.normalized * accMag;
		}
		//}


		//bool hitDirect = Physics.Raycast (transform.position, goal.transform.position - transform.position, rayDist);
		/*if (!hitForward && !hitDirect) {
			return accel;
		}
		else if (!hitForward && hitDirect){
			return transform.forward.normalized * accMag;
		}
		else {
			foreach (Collider obstacle in hits) {
				if(obstacle.gameObject != this.gameObject && obstacle.gameObject.name != "Ground") {
					Debug.Log (obstacle.gameObject.name);
					//Vector3 obstacleLoc = new Vector3 (obstacle.transform.position.x, 0.0f, obstacle.transform.position.z);

					//Vector3 closest = obstacle.ClosestPointOnBounds(transform.position);
					RaycastHit hit;
					bool hitObstacle = Physics.Raycast (transform.position, closest - transform.position, out hit, rayDist);
					//bool hitObstacle = Physics.Raycast (transform.position, transform.forward, out hit, rayDist);
					Debug.DrawRay(transform.position, (closest - transform.position) * rayDist, Color.yellow);
					Vector3 normal = hit.normal.normalized * accMag;
					float obstacleDist = hit.distance;
					//float obstacleDist = Vector3.Distance (obstacleLoc, transform.position);
					//maxRadsDelta = Mathf.Deg2Rad * 20.0f * (1.0f - (obstacleDist/rayDist));
					if(obstacleDist < Vector3.Distance (goal.transform.position, transform.position)) {
						accel = accel * obstacleDist/100.0f + normal * (1.0f - (obstacleDist/100.0f));
						//accel = transform.forward * accMag * obstacleDist/rayDist - normal * (1.0f - (obstacleDist/rayDist));
						//accel = accel * obstacleDist/rayDist + transform.right * accMag * (1.0f - (obstacleDist/rayDist));
						Debug.DrawRay (hit.point, normal * (1.0f - (obstacleDist/rayDist)));
						accel = accel.normalized * accMag;
					}
				}
			}
			return accel;
		}*/
	}


	/*
	Vector3 avoidObjects(Vector3 acceleration){
		RaycastHit hitL;
		RaycastHit hitR;

		bool hitLeft = Physics.Raycast (transform.position - transform.right*charWidth, transform.forward, out hitL, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right*charWidth, transform.forward, out hitR, rayDist);

		float targetDist = Vector3.Distance (transform.position, target.transform.position);

		if (hitLeft && (targetDist >= hitL.distance)) {
			return Vector3.RotateTowards(acceleration, transform.forward + transform.right, 2.0f, 0.0f);
		}

		if (hitRight && (targetDist >= hitR.distance)) {
			return Vector3.RotateTowards(acceleration, transform.forward - transform.right, 2.0f, 0.0f);
		}

		return acceleration;
	}
	*/

	/*Vector3 calculateAcceleration() {

		RaycastHit hitL;
		RaycastHit hitR;

		bool hitLeft = Physics.Raycast (transform.position - transform.right*charWidth, transform.forward, out hitL, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right*charWidth, transform.forward, out hitR, rayDist);
		if (Vector3.Distance (transform.position, tempGoal) < 1.0f) {
			
		

			if (hitLeft && hitRight) {
				if (hitL.distance < hitR.distance) {
					Vector3 somewheretotheright = hitR.point + hitR.normal.normalized * 15.0f + ;
					tempGoal = somewheretotheright;
				} else {
					Vector3 somewheretotheleft = hitL.point + hitL.normal.normalized * 50.0f;
					tempGoal = somewheretotheleft;
				}
			} else if (!hitLeft && hitRight) {
				Vector3 somewheretotheleft = hitR.point + hitR.normal.normalized * 50.0f;
				tempGoal = somewheretotheleft;
			} else if (hitLeft && !hitRight) {
				Vector3 somewheretotheright = hitL.point + hitL.normal.normalized * 50.0f;
				tempGoal = somewheretotheright;
			} else {
				tempGoal = goal.transform.position;
			}

		}
		targetAccel = new Vector3 (tempGoal.x - transform.position.x, 0.0f,tempGoal.z - transform.position.z);
		return targetAccel;
	}*/

	/*
	Vector3 calculateAcceleration () {
		RaycastHit hitL;
		RaycastHit hitR;
		RaycastHit hitLS;
		RaycastHit hitRS;
		RaycastHit hitT;

		targetAccel = new Vector3 (goal.transform.position.x - transform.position.x, 0.0f,goal.transform.position.z - transform.position.z);

		bool hitLeft = Physics.Raycast (transform.position - transform.right*charWidth, transform.forward.normalized, out hitL, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right*charWidth, transform.forward.normalized, out hitR, rayDist);
		bool hitLeftS = Physics.Raycast (transform.position - transform.right*charWidth, (transform.forward - transform.right).normalized, out hitLS, 1.5f * rayDist);
		bool hitRightS = Physics.Raycast (transform.position + transform.right*charWidth, (transform.forward + transform.right).normalized, out hitRS, 1.5f * rayDist);

		Debug.DrawRay(transform.position - transform.right*charWidth, transform.forward.normalized * hitL.distance, Color.magenta);
		Debug.DrawRay(transform.position + transform.right*charWidth, transform.forward.normalized * hitR.distance, Color.magenta);
		Debug.DrawRay(transform.position - transform.right*charWidth, (transform.forward - transform.right).normalized * hitLS.distance, Color.magenta);
		Debug.DrawRay(transform.position + transform.right*charWidth, (transform.forward + transform.right).normalized * hitRS.distance, Color.magenta);

		float lDist = (transform.position - transform.right * charWidth - goal.transform.position).magnitude;
		float rDist = (transform.position + transform.right * charWidth - goal.transform.position).magnitude;
		bool rightClose = rDist < lDist;
		float targetDist = Vector3.Distance (transform.position, goal.transform.position);
		
		if (!hitLeft && !hitRight && !hitLeftS && !hitRightS) {
			Debug.Log ("nothing is hitting");
			return targetAccel;
		} else if (!hitLeft && !hitRight) {
			Debug.Log ("gostraight" + hitRight + hitLeft);
			if (rightClose) {
				if (!hitRightS || (targetDist <= hitRS.distance)) {
					//acceleration already set towards target
					return targetAccel;
				} else {
					return transform.forward * (accMag);
				}
			} else {
				if (!hitLeftS || (targetDist <= hitLS.distance)) {
					return targetAccel;
				} else {
					return transform.forward * (accMag);
				}
			}
		} else if (hitLeft || hitRight) {
			Debug.Log ("asdf-1");
			if (rightClose) {
				if (!hitRightS || (targetDist <= hitRS.distance)) {
					//return Vector3.RotateTowards (acceleration, transform.forward + transform.right, 1.0f, 0.0f);
					//return targetAccel;
					return (transform.forward + transform.right).normalized * accMag;
				} else {
					if (!hitLeftS || (targetDist <= hitLS.distance)) {
						return (transform.forward - transform.right).normalized * accMag;
						//return Vector3.RotateTowards (acceleration, transform.forward - transform.right, 1.0f, 0.0f);
					} else {
						return (targetAccel * (-1.0f));
					}
				}
			} else {
				if (!hitLeftS || (targetDist <= hitLS.distance)) {
					//return Vector3.RotateTowards (acceleration, transform.forward - transform.right, 1.0f, 0.0f);
					Debug.Log ("asdf0");
					return (transform.forward - transform.right).normalized * accMag;
				} else {
					if (!hitRightS || (targetDist <= hitRS.distance)) {
						Debug.Log ("asdf1");
						return (transform.forward + transform.right).normalized * accMag;
						//return Vector3.RotateTowards (acceleration, transform.forward + transform.right, 1.0f, 0.0f);
					} else {
						return (targetAccel * (-1.0f));
					}
				}
			}
		} else {
			return targetAccel;
		}
	}*/

	/*Vector3 collisions(Vector3 acceleration) {
		RaycastHit hitL;
		RaycastHit hitR;
		RaycastHit hitLS;
		RaycastHit hitRS;
		RaycastHit hitT;

		bool hitLeft = Physics.Raycast (transform.position - transform.right*charWidth, transform.forward, out hitL, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right*charWidth, transform.forward, out hitR, rayDist);
		bool hitLeftS = Physics.Raycast (transform.position - transform.right*charWidth, transform.forward - transform.right, out hitLS, 1.5f * rayDist);
		bool hitRightS = Physics.Raycast (transform.position + transform.right*charWidth, transform.forward + transform.right, out hitRS, 1.5f * rayDist);
		bool hitTarget = Physics.Raycast (transform.position, target.transform.position - transform.position, out hitT, rayDist);

		float lDist = (transform.position - transform.right * charWidth - target.transform.position).magnitude;
		float rDist = (transform.position + transform.right * charWidth - target.transform.position).magnitude;
		Debug.Log ("rDist: " + rDist + " lDist: " + lDist);
		bool rightClose = rDist < lDist;
		float targetDist = Vector3.Distance (transform.position, target.transform.position);

		if (!hitLeft && !hitRight && !hitLeftS && !hitRightS && !hitTarget) {
			return acceleration;
		} else if (!hitLeft && !hitRight) {
			if (rightClose) {
				if (!hitRightS || (targetDist <= hitRS.distance)) {
					//acceleration already set towards target
					return acceleration;
				} else {
					return transform.forward * (acceleration.magnitude);
				}
			} else {
				if (!hitLeftS || (targetDist <= hitLS.distance)) {
					return acceleration;
				} else {
					return transform.forward * (acceleration.magnitude);
				}
			}
		} else if (hitLeft || hitRight) {
			Debug.Log("hitRight: " + hitRight);
			if (rightClose) {
				Debug.Log("\trightClose: " + rightClose);
				if (!hitRightS && (targetDist > hitRS.distance)) {
					Debug.Log ("\t\tNot HitRightS");
					return Vector3.RotateTowards (acceleration, transform.forward + transform.right, 1.0f, 0.0f);
				} else {
					Debug.Log("\t\t\thitRightS is True");
					if (!hitLeftS && (targetDist > hitLS.distance)) {
						Debug.Log("\t\t\t\tNot hitLeftS");
						return (transform.forward - transform.right) * acceleration.magnitude;
						//return Vector3.RotateTowards (acceleration, transform.forward - transform.right, 1.0f, 0.0f);
					} else {
						return (acceleration * -1.0f);
					}
				}
			} else {
				Debug.Log("LeftClose");
				if (!hitLeftS && (targetDist > hitLS.distance)) {
					Debug.Log("\tNot HitleftS");
					return Vector3.RotateTowards (acceleration, transform.forward - transform.right, 1.0f, 0.0f);
				} else {
					Debug.Log ("\t\tHitLeftS");
					if (!hitRightS && (targetDist > hitRS.distance)) {
						Debug.Log ("\t\t\tNot HitRightS");
						return Vector3.RotateTowards (acceleration, transform.forward + transform.right, 1.0f, 0.0f);
					} else {
						Debug.Log ("\t\t\tHitRightS");
						return (acceleration * -1.0f);
					}
				}
			}
		} else {
			return acceleration;
		}

	}*/

}
