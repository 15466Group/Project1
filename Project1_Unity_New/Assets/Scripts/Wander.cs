using UnityEngine;
using System.Collections;

public class Wander : BehaviorClass {

	// Use this for initialization
/*	public override void Start (){
		base.Start ();
		accMag = 50.0f;
		maxRadsDelta = Mathf.Deg2Rad * 1.0f;
		behaviorWeight = 1.0f;
	}
	
	// Update is called once per frame
	public override void Update(){
//		randomRad = 0.0f;
//		if (targetAccel == acceleration) {
//			randomRad = Random.Range (0.0f, Mathf.PI * 2.0f);
//			float newx = accMag * Mathf.Cos (randomRad);
//			float newz = accMag * Mathf.Sin (randomRad);
//			targetAccel = new Vector3 (newx, 0.0f, newz);
//		}
		Vector3 targetPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		if (Vector3.Distance(targetPosition, transform.position) <= 0.7f) {
			//calculate new targetPosition
			//jk what if inside obstacle??
			targetPosition = new Vector3 (300.0f, 0.0f, 300.0f);
			Debug.Log ("yay");
		}
		targetAccel = calculateAcceleration (targetPosition);
		base.Update ();
	}*/
	Vector3 targetPosition;
	public override void Start ()
	{
		base.Start ();
		accMag = 500.0f;
		rayDist = 30.0f;
		rayDistMax = rayDist;
		behaviorWeight = 2.0f;
	}
	
	public override void Update(){
		//updates the maximum speed
		targetAccel = calculateAcceleration (targetPosition);
		veloCloseToTarget ();
		base.Update ();
	}
	
	//updates maxSpeed
	void veloCloseToTarget () {
		float epsilon = 50.0f;
		float xDistance = Mathf.Abs (transform.position.x - goal.transform.position.x);
		float zDistance = Mathf.Abs (transform.position.z - goal.transform.position.z);
		float distance = Mathf.Sqrt (xDistance * xDistance + zDistance * zDistance);

		if (distance <= epsilon) {
			targetPosition = randomPoint ();
		}
	}

	Vector3 randomPoint () {
		randomRad = Random.Range (0.0f, Mathf.PI * 2.0f);
		float randomXRange = Random.Range (10.0f, 50.0f);
		float randomZRange = Random.Range (10.0f, 50.0f);
		float newx = randomXRange * Mathf.Cos (randomRad);
		float newz = randomZRange * Mathf.Sin (randomRad);
		return new Vector3 (newx, 0.0f, newz);
	}
}
