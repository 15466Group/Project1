using UnityEngine;
using System.Collections;

public class Wander : BehaviorClass {

	// Use this for initialization
	public override void Start (){
		base.Start ();
		accMag = 50.0f;
		maxRadsDelta = Mathf.Deg2Rad * 1.0f;
		behaviorWeight = 1.0f;
		maxSpeed = 20.0f;
		walkingSpeed = 20.0f;
//		accMag = 500.0f;
//		behaviorWeight = 1.0f;
	}
	
	// Update is called once per frame
	public override void Update(){
		randomRad = 0.0f;
		if (targetAccel == acceleration) {
			randomRad = Random.Range (0.0f, Mathf.PI * 2.0f);
			float newx = accMag * Mathf.Cos (randomRad);
			float newz = accMag * Mathf.Sin (randomRad);
			targetAccel = new Vector3 (newx, 0.0f, newz);
		}
		targetAccel = calculateAcceleration (new Vector3());
		base.Update ();
	}

	public override Vector3 calculateAcceleration(Vector3 dummyPosition){
		Collider[] hits = Physics.OverlapSphere (transform.position, rayDist);
		Collider[] closeHits = Physics.OverlapSphere (transform.position, rayDistClose);
		Vector3 accel = targetAccel;

		bool hitLeft = Physics.Raycast (transform.position - transform.right*charWidth, transform.forward, rayDist);
		bool hitRight = Physics.Raycast (transform.position + transform.right*charWidth, transform.forward, rayDist);
		bool hitForward = hitLeft || hitRight;
		accMag = 50.0f;
		maxRadsDelta = Mathf.Deg2Rad * 1.0f;
		if (!hitForward) {
			//check for anything that's not in direction of character moving, but really close, need to move asap!
			accel = checkForHits(accel, closeHits, false);
			return accel;
		}
		else {
			accMag = 500.0f;
			maxRadsDelta = Mathf.Deg2Rad * 10.0f;
			accel = base.checkForHits(accel, hits, false);
			return accel;
		}
	}

	public override Vector3 checkForHits (Vector3 accel, Collider[] hits, bool reachGoal)
	{
		float oldAccMag = accMag;
		float oldMaxRadsDelta = maxRadsDelta;
		if (hits.Length > 0) {
			accMag = 500.0f;
			maxRadsDelta = Mathf.Deg2Rad * 10.0f; 
		}
		accel = base.checkForHits (accel, hits, reachGoal);
		accMag = oldAccMag;
		maxRadsDelta = oldMaxRadsDelta;
		return accel;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere (transform.position, rayDistClose);
	}
}