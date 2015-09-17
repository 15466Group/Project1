using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	private Vector3 movement;
	public Animation anim;

	private Vector3 velocity;
	private Vector3 acceleration;
	private float accMag;
	private Vector3 targetAccel;
	private float maxRadsDelta;
	private float maxMagDelta;
	private float maxSpeed;

	private float smooth;
	private float randomRad;
	private float idleSpeed;
	private float walkingSpeed;
	private Vector3 targetPosition;

	private float timer;


	// Use this for initialization
	void Start () {
		accMag = 50.0f;
		maxSpeed = 20.0f;
		velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		acceleration = new Vector3 (0.0f, 0.0f, 0.0f);
		targetAccel = new Vector3 (0.0f, 0.0f, 0.0f);
		targetPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		maxMagDelta = 100.0f;
		maxRadsDelta = Mathf.Deg2Rad * 15.0f;

		idleSpeed = 0.0f;
		walkingSpeed = 8.0f;
		anim.CrossFade ("idle");
		smooth = 5.0f;
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		//transform.Translate (velocity * Time.deltaTime, Space.World);
		transform.position += velocity * Time.deltaTime;
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, maxSpeed);

		if (timer >= 4.0f) {
			randomRad = 0.0f;
			if (targetAccel == acceleration) {
				randomRad = Random.Range (0.0f, Mathf.PI * 2.0f);
				float newx = accMag * Mathf.Cos (randomRad);
				float newz = accMag * Mathf.Sin (randomRad);
				targetAccel = new Vector3 (newx, 0.0f, newz);
			}
			timer = 0.0f;
		}

		acceleration = Vector3.RotateTowards (acceleration, targetAccel, maxRadsDelta, maxMagDelta);

		targetPosition = transform.position + velocity * Time.deltaTime;

		RotateTo (targetPosition);

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
		Debug.DrawRay(transform.position,velocity*10,Color.green);
		Debug.DrawRay(transform.position,acceleration*10,Color.blue);
		destinationRotation = Quaternion.LookRotation (relativePosition);
		transform.rotation = Quaternion.Slerp (transform.rotation, destinationRotation, Time.deltaTime * smooth);
	}
}



