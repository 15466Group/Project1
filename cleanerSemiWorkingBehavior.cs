using UnityEngine;
using System.Collections;

[System.Serializable]
public class Behavior : MonoBehaviour {
	
	//superclass for wander and reach goal behaviors
	//accelerate object towards a position (either randomly generated [wander] or a goal [reachgoal])
	
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
	
	public bool isWanderer { get; set; }
	
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
		closeRayDistDefault = 7.0f;
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
		transform.position += velocity * Time.deltaTime;
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, speedMax);
		//update speedMax
		veloCloseToTarget ();
		targetPosition = transform.position + velocity * Time.deltaTime;
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
	
	void RotateTo(Vector3 targetPosition){ //bafsfd
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
	
	void OnDrawGizmos(){
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere (transform.position, closeRayDist);
	}
	
	//even if something isn't directly in front of the character, should still avoid it if it's too close
	//cus he cant turn instantaneously
	Vector3 checkCloseCalls(Vector3 acceleration) {
		Collider[] hits = Physics.OverlapSphere (transform.position, closeRayDist);
		//don't include hitting self
		if (hits.Length > 1) {
			Vector3 accumulator = obstacleAvoidance(rayDist, hits);
			return accumulator.normalized * accMag;
		} else {
			return acceleration;
		}
	}
	
	public virtual Vector3 calculateAcceleration(Vector3 target) {
		RaycastHit hitL;
		RaycastHit hitR;
		float weight = charWeight;
		
		bool hitLeft = Physics.Raycast (transform.position - transform.right.normalized * charWidth, transform.forward, out hitL, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right.normalized * charWidth, transform.forward, out hitR, rayDist);
		
		//doesn't matter if an object is behind the goal, can still reach it withtout bumping into the object
		float distToTarget = Vector3.Distance (transform.position, target);
		if (distToTarget < hitL.distance && distToTarget < hitR.distance) {
			return (target - transform.position).normalized * accMag;
		}
		
		Debug.DrawRay(transform.position,transform.forward * 50.0f,Color.red);
		if (hitRight || hitLeft) {
			Collider[] hits = Physics.OverlapSphere(transform.position, rayDist);
			Vector3 accumulator = obstacleAvoidance(rayDist, hits);
			accumulator += transform.forward.normalized;
			return checkCloseCalls(accumulator.normalized * accMag);
		}
		else {
			bool potentialHit = Physics.Raycast (transform.position, target - transform.position, rayDist);
			if(potentialHit) {
				return checkCloseCalls(transform.forward.normalized * accMag);
			}
			else {
				return checkCloseCalls((target - transform.position).normalized * accMag);
			}
		}
	}
	
	Vector3 obstacleAvoidance(float radius, Collider[] hits){
		float weight = charWeight;
		Vector3 accumulator = new Vector3 ();
		foreach (Collider obstacle in hits) {
			if (obstacle.gameObject != this.gameObject && obstacle.gameObject.name != "Ground") {
				if (!obstacle.gameObject.name.Contains("samuz")){
					weight = obstacleWeight;
				} else {
					//the wanderer only moves out of the way for other wanderer's
					//the reachers always move out of the way for every character
					if(isWanderer && obstacle.gameObject.GetComponent ("Wander") == null) {
						weight = 0.0f;
					}
					else {
						weight = charWeight;
					}
				}
				//doing this to find the closest point on the bounds of the obstacle,
				//	note that builtin ClosestPointOnBounds does not work on rotated objects, as we found out the hard way...
				RaycastHit hit = new RaycastHit();
				RaycastHit hitN;
				RaycastHit[] rays;
				rays = Physics.RaycastAll(transform.position, obstacle.transform.position - transform.position);
				foreach (RaycastHit h in rays){
					if (h.collider == obstacle){
						hit = h;
						break;
					}
				}
				Vector3 normal = hit.normal;
				//raycast from position to potential closest point on bounds (may miss the object though in the else case)
				bool closest = Physics.Raycast (transform.position, normal * (-1.0f), out hitN, radius);
				if (closest) {
					//as distance gets smaller, hitN.distance/rayDist is smaller so closer to 1.0f, bigger, closer to 0.0f
					accumulator += hitN.normal.normalized * weight * (1.0f - (hitN.distance / radius));
				} else {
					float estim = Mathf.Min (rayDist, hit.distance);
					accumulator += (transform.position - obstacle.transform.position).normalized * weight * (1.0f - (estim / radius));
				}
			}
		}
		return accumulator;
	}
	
	void veloCloseToTarget () {
		float epsilon = 0.7f;
		float xDistance = Mathf.Abs (transform.position.x - target.x);
		float zDistance = Mathf.Abs (transform.position.z - target.z);
		float distance = Mathf.Sqrt (xDistance * xDistance + zDistance * zDistance);
		if (distance <= epsilon) {
			speedMax = 0.0f; 
		}
		else { //exponential growth translated up by 10, capped at originalMaxSpeed
			speedMax = Mathf.Min(Mathf.Pow(1.1f,distance) + 10.0f, speedMaxDefault);
		}
	}
}
