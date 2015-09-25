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
	private Vector3 targetPosition;
	public Vector3 acceleration{ get; set; }
	public Vector3 targetAccel { get; set; }
	public float accMag { get; set; }
	public float maxRadsDelta { get; set; }
	public float maxMagDelta  { get; set; }
	public float maxSpeed { get; set; }

	public float behaviorWeight { get; set; }

	private float smooth; //for rotating
	private float walkingSpeed;

	//just for wandering
	public float randomRad  { get; set; }

	//just for reaching goal
	public float charWidth  { get; set; }
	public float rayDist  { get; set; }
	public float originalMaxSpeed { get; set; }
	public GameObject goal;


	//methods
	// Use this for initialization
	public virtual void Start () {
		anim = GetComponent<Animation>();

		velocity = new Vector3 ();
		targetPosition = new Vector3 (); //position where char wants to move to next
		acceleration = new Vector3 ();
		targetAccel = new Vector3 ();
		accMag = 50.0f;
		maxRadsDelta = Mathf.Deg2Rad * 20.0f;
		maxMagDelta = 100.0f;
		maxSpeed = 50.0f;

		behaviorWeight = 1.0f;

		smooth = 5.0f;
		walkingSpeed = 20.0f;

		randomRad = 0.0f;

		charWidth = 5.0f;
		rayDist = 20.0f;
		originalMaxSpeed = maxSpeed;

		anim.CrossFade ("idle");
	}
	
	// Update is called once per frame
	public virtual void Update () {
		acceleration = Vector3.RotateTowards (acceleration, targetAccel, maxRadsDelta, maxMagDelta);
		targetPosition = transform.position + velocity * Time.deltaTime;
		if (velocity != new Vector3())
			RotateTo (targetPosition);

		transform.position += velocity * Time.deltaTime;
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);

		float mag = velocity.magnitude;
		Debug.Log ("mag: " + mag);
		if (mag <= walkingSpeed && mag > 0.0f) {
			Debug.Log ("Walking");
			anim.CrossFade ("Walk");
		} else if (mag > walkingSpeed) {
			Debug.Log ("Running");
			anim.CrossFade ("Run");
		} else {
			Debug.Log ("Idle");
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
//		Debug.DrawRay(transform.position - transform.right*charWidth, transform.forward.normalized * rayDist, Color.yellow);
//		Debug.DrawRay(transform.position + transform.right*charWidth, transform.forward.normalized * rayDist, Color.yellow);
//		Debug.DrawRay(transform.position - transform.right*charWidth, (transform.forward - transform.right).normalized * 1.5f * rayDist, Color.yellow);
//		Debug.DrawRay(transform.position + transform.right*charWidth, (transform.forward + transform.right).normalized * 1.5f * rayDist, Color.yellow);
		//Debug.DrawRay(transform.position, (target.transform.position - transform.position) * 50.0f, Color.yellow);
		destinationRotation = Quaternion.LookRotation (relativePosition);
		transform.rotation = Quaternion.Slerp (transform.rotation, destinationRotation, Time.deltaTime * smooth);
	}
}
