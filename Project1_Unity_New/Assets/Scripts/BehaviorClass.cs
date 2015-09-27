using UnityEngine;
using System.Collections;

[System.Serializable]
public class BehaviorClass : MonoBehaviour {

	//can add multiple scripts to gameobjects,
	//enable and disable them if event triggers change

	//fields
	public Animation anim;

	//for all behaviors
	private Vector3 velocity;
	private Vector3 nextPosition;
	public Vector3 acceleration{ get; set; }
	public Vector3 targetAccel { get; set; }
	public float accMag { get; set; }
	public float maxRadsDelta { get; set; }
	public float maxMagDelta  { get; set; }
	public float maxSpeed { get; set; }

	public float behaviorWeight { get; set; }

	private float smooth; //for rotating
	public float walkingSpeed { get; set; }

	//just for wandering
	public float randomRad  { get; set; }

	//just for reaching goal
	public float charWidth  { get; set; }
	public float rayDist  { get; set; }
	public float rayDistMax  { get; set; }
	public float originalMaxSpeed { get; set; }
	public float rayDistClose { get; set; }
	public GameObject goal;

	private int count;


	//methods
	// Use this for initialization
	public virtual void Start () {
		anim = GetComponent<Animation>();

		velocity = new Vector3 ();
		nextPosition = new Vector3 (); //position where char wants to move to next
		acceleration = new Vector3 ();
		targetAccel = new Vector3 ();
		accMag = 50.0f;
		maxRadsDelta = Mathf.Deg2Rad * 20.0f;
		maxMagDelta = 100.0f;
		maxSpeed = 20.0f;

		behaviorWeight = 1.0f;

		smooth = 5.0f;
		walkingSpeed = 20.0f;

		randomRad = 0.0f;

		charWidth = 5.0f;
		rayDist = 30.0f;
		rayDistMax = rayDist;
		originalMaxSpeed = maxSpeed;
		rayDistClose = 5.0f;

		anim.CrossFade ("idle");
		count = 0;
	}
	
	// Update is called once per frame
	public virtual void Update () {
		//get the new targetAccel
		targetAccel = targetAccel.normalized;
		targetAccel = targetAccel * accMag;
		acceleration = Vector3.RotateTowards (acceleration, targetAccel, maxRadsDelta, maxMagDelta);
		nextPosition = transform.position + velocity * Time.deltaTime;
		if (velocity != new Vector3())
			RotateTo (nextPosition);

		transform.position += velocity * Time.deltaTime;
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);

		float mag = velocity.magnitude;
		if (mag <= walkingSpeed && mag > 0.0f) {
			anim.CrossFade ("Walk");
		} else if (mag > walkingSpeed) {
			anim.CrossFade ("Run");
		} else {
			anim.CrossFade ("idle");
		}
	}

	void RotateTo(Vector3 pos){
		//maxDistance is the maximum ray distance
		Quaternion destinationRotation;
		Vector3 relativePosition;
		relativePosition = pos - transform.position;
		Debug.DrawRay(transform.position,relativePosition*10,Color.red);
		Debug.DrawRay(transform.position,targetAccel.normalized*30,Color.red);
		Debug.DrawRay(transform.position,velocity.normalized*20,Color.green);
		Debug.DrawRay(transform.position,acceleration.normalized*10,Color.blue);
//		Debug.DrawRay(transform.position - transform.right*charWidth, transform.forward.normalized * rayDist, Color.yellow);
//		Debug.DrawRay(transform.position + transform.right*charWidth, transform.forward.normalized * rayDist, Color.yellow);
//		Debug.DrawRay(transform.position - transform.right*charWidth, (transform.forward - transform.right).normalized * 1.5f * rayDist, Color.yellow);
//		Debug.DrawRay(transform.position + transform.right*charWidth, (transform.forward + transform.right).normalized * 1.5f * rayDist, Color.yellow);
		//Debug.DrawRay(transform.position, (target.transform.position - transform.position) * 50.0f, Color.yellow);
		destinationRotation = Quaternion.LookRotation (relativePosition);
		transform.rotation = Quaternion.Slerp (transform.rotation, destinationRotation, Time.deltaTime * smooth);
	}

	
	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		//Gizmos.DrawWireSphere (transform.position, rayDist);
	}

	//deals with obstacle avoidance
	public virtual Vector3 calculateAcceleration(Vector3 targetPosition) {
		//rayDist = 30.0f;
		rayDist = Mathf.Min (rayDistMax, (targetPosition - transform.position).magnitude);
		Collider[] hits = Physics.OverlapSphere (transform.position, rayDist);
		Vector3 accel = new Vector3 (targetPosition.x - transform.position.x, 0.0f,targetPosition.z - transform.position.z);
		accel = accel.normalized * accMag;
		bool hitLeft = Physics.Raycast (transform.position - transform.right*charWidth, transform.forward, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right*charWidth, transform.forward, rayDist);
		bool hitForward = hitLeft || hitRight;
		Debug.Log (hitLeft + " " + hitRight + " " + hitForward);
		bool hitDirect = Physics.Raycast (transform.position, targetPosition - transform.position, rayDist);
		if (!hitForward && !hitDirect) {
			return accel;
		}
		else if (!hitForward && hitDirect){
			return transform.forward.normalized * accMag;
		}
		else {
			count += 1;
			Debug.Log ("checkForHits count: " + count);
			accel = checkForHits(accel, hits, true);
			return accel;
		}
	}

	public virtual Vector3 checkForHits(Vector3 accel, Collider[] hits, bool reachGoal){
		foreach (Collider obstacle in hits) {
			if(obstacle.gameObject != this.gameObject && obstacle.gameObject.name != "Ground") {
				Debug.Log(obstacle.name);
				Vector3 closest = obstacle.ClosestPointOnBounds(transform.position);
				RaycastHit hit;
				Physics.Raycast (transform.position, closest - transform.position, out hit, rayDist);
				Debug.Log ("DRAWING");
				Debug.Log (closest + " " + transform.position);
				Debug.DrawRay(transform.position, (closest - transform.position) * 50000, Color.yellow);
				Vector3 normal = hit.normal.normalized * accMag;
				float obstacleDist = hit.distance;

				if (reachGoal){
					//if (obstacleDist < (Vector3.Distance(goal.transform.position, transform.position))){
						accel = accel * obstacleDist/rayDist + normal * (1.0f - (obstacleDist/rayDist));
						Debug.Log ("in here, hitting obstacle: " + obstacle.gameObject.name + " len is: " + hits.Length);
						Debug.DrawRay (hit.point, normal * (1.0f - (obstacleDist/rayDist)));
						accel = accel.normalized * accMag;
					//}
				}
				else {
					accel = accel * obstacleDist/rayDist + normal * (1.0f - (obstacleDist/rayDist));
					
					Debug.DrawRay (hit.point, normal * (1.0f - (obstacleDist/rayDist)));
					accel = accel.normalized * accMag;
				}
			}
		}
		return accel;
	}
}
