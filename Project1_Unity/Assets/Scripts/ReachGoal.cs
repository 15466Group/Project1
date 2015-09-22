using UnityEngine;
using System.Collections;

public class ReachGoal : MonoBehaviour {

	public GameObject target;
	public Animation anim;

	private Vector3 acceleration;
	private Vector3 velocity;
	private float accMag;
	private float maxSpeed;

	private Vector3 targetAccel;
	private Vector3 targetPosition;

	private float idleSpeed;
	private float walkingSpeed;

	private float maxRadsDelta;
	private float maxMagDelta;

	private float smooth;

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
		walkingSpeed = 10.0f;
		maxSpeed = 25.0f;
		anim.CrossFade ("idle");
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


		targetAccel = new Vector3 (target.transform.position.x - transform.position.x, 0.0f,target.transform.position.z - transform.position.z);
		//targetAccel = new Vector3 (transform.position.x - target.transform.position.x, 0.0f, transform.position.z - target.transform.position.z);
		targetAccel = collisions (targetAccel);
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
		float maxDistance = 20.0f;
		Quaternion destinationRotation;
		Vector3 relativePosition;
		relativePosition = targetPosition - transform.position;
		Debug.DrawRay(transform.position,relativePosition*10,Color.red);
		Debug.DrawRay(transform.position,targetAccel.normalized*30,Color.red);
		Debug.DrawRay(transform.position,velocity.normalized*20,Color.green);
		Debug.DrawRay(transform.position,acceleration.normalized*10,Color.blue);
		Debug.DrawRay(transform.position - transform.right*5.0f, transform.forward * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position + transform.right*5.0f, transform.forward * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position - transform.right*5.0f, (transform.forward - transform.right) * 2 * maxDistance, Color.yellow);
		Debug.DrawRay(transform.position + transform.right*5.0f, (transform.forward + transform.right) * 2 * maxDistance, Color.yellow);
		//Debug.DrawRay(transform.position, (target.transform.position - transform.position) * 50.0f, Color.yellow);
		destinationRotation = Quaternion.LookRotation (relativePosition);
		transform.rotation = Quaternion.Slerp (transform.rotation, destinationRotation, Time.deltaTime * smooth);
	}

	void veloCloseToTarget () {
		float epsilon = 0.7f;
		float xDistance = Mathf.Abs (transform.position.x - target.transform.position.x);
		float zDistance = Mathf.Abs (transform.position.z - target.transform.position.z);
		float distance = Mathf.Sqrt (xDistance * xDistance + zDistance * zDistance);

		if (distance <= epsilon) {
			maxSpeed = 0.0f; 
		}
		else { //exponential growth, capped at previous maxSpeed
			//fixme
			maxSpeed = Mathf.Min(Mathf.Pow(1.1f,distance) + 10.0f, 50.0f);
		}
	}

	Vector3 collisions(Vector3 acceleration) {
		float maxDistance = 100.0f;
		RaycastHit hitL;
		RaycastHit hitR;
		RaycastHit hitLS;
		RaycastHit hitRS;
		RaycastHit hitT;

		float charWidth;

		bool hitLeft = Physics.Raycast (transform.position - transform.right*5.0f, transform.forward, out hitL, maxDistance);
		bool hitRight = Physics.Raycast (transform.position + transform.right*5.0f, transform.forward, out hitR, maxDistance);
		bool hitLeftS = Physics.Raycast (transform.position - transform.right*5.0f, transform.forward - transform.right, out hitR, 2 * maxDistance);
		bool hitRightS = Physics.Raycast (transform.position + transform.right*5.0f, transform.forward + transform.right, out hitR, 2 * maxDistance);
		bool hitTarget = Physics.Raycast (transform.position, target.transform.position - transform.position, out hitT, maxDistance);

		float lDist = (transform.position - transform.right * 5.0f - target.transform.position).magnitude;
		float rDist = (transform.position + transform.right * 5.0f - target.transform.position).magnitude;
		Debug.Log ("rDist: " + rDist + " lDist: " + lDist);
		bool rightClose = rDist < lDist;

		if (!hitLeft && !hitRight && !hitLeftS && !hitRightS && !hitTarget) {
			return acceleration;
		} else if (!hitLeft && !hitRight) {
			if (rightClose) {
				if (!hitRightS) {
					//acceleration already set towards target
					return acceleration;
				} else {
					return transform.forward * (acceleration.magnitude);
				}
			} else {
				if (!hitLeftS) {
					return acceleration;
				} else {
					return transform.forward * (acceleration.magnitude);
				}
			}
		} else if (hitLeft || hitRight) {
			Debug.Log("hitRight: " + hitRight);
			if (rightClose) {
				Debug.Log("\trightClose: " + rightClose);
				if (!hitRightS) {
					Debug.Log ("\t\tNot HitRightS");
					return Vector3.RotateTowards (acceleration, transform.forward + transform.right, 1.0f, 0.0f);
				} else {
					Debug.Log("\t\t\thitRightS is True");
					if (!hitLeftS) {
						Debug.Log("\t\t\t\tNot hitLeftS");
						return (transform.forward - transform.right) * acceleration.magnitude;
						//return Vector3.RotateTowards (acceleration, transform.forward - transform.right, 1.0f, 0.0f);
					} else {
						return (acceleration * -1.0f);
					}
				}
			} else {
				Debug.Log("LeftClose");
				if (!hitLeftS) {
					Debug.Log("\tNot HitleftS");
					return Vector3.RotateTowards (acceleration, transform.forward - transform.right, 1.0f, 0.0f);
				} else {
					Debug.Log ("\t\tHitLeftS");
					if (!hitRightS) {
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

	}

}
