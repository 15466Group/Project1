using UnityEngine;
using System.Collections;

public class ReachGoal : MonoBehaviour {

	public GameObject target;
	public Animation anim;

	private Vector3 acceleration;
	private Vector3 velocity;
	private float accMag;
	public float maxSpeed;

	private Vector3 targetAccel;
	private Vector3 targetPosition;

	private float idleSpeed;
	public float walkingSpeed;

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
	}
	
	// Update is called once per frame
	void Update () {
		//want him to accelerate towards the target.position
		transform.position += velocity * Time.deltaTime;
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);
		
		acceleration = Vector3.RotateTowards (acceleration, targetAccel, maxRadsDelta, maxMagDelta);
		targetPosition = transform.position + velocity * Time.deltaTime;
		RotateTo (targetPosition);


		targetAccel = new Vector3 (target.transform.position.x - transform.position.x, 0.0f,target.transform.position.z - transform.position.z);
		targetAccel = targetAccel.normalized;
		targetAccel = targetAccel * accMag;


		float mag = velocity.magnitude;
		if (mag <= walkingSpeed && mag > idleSpeed) {
			anim.CrossFade ("Walk");
		} else if (mag > walkingSpeed) {
			anim.CrossFade ("Run");
		} else 
			anim.CrossFade ("idle");
	}

	void RotateTo(Vector3 targetPosition){
		Quaternion destinationRotation;
		Vector3 relativePosition;
		relativePosition = targetPosition - transform.position;
		Debug.DrawRay(transform.position,relativePosition*10,Color.red);
		Debug.DrawRay(transform.position,targetAccel*10,Color.red);
		Debug.DrawRay(transform.position,velocity*10,Color.green);
		Debug.DrawRay(transform.position,acceleration*10,Color.blue);
		destinationRotation = Quaternion.LookRotation (relativePosition);
		transform.rotation = Quaternion.Slerp (transform.rotation, destinationRotation, Time.deltaTime * smooth);
	}
}
