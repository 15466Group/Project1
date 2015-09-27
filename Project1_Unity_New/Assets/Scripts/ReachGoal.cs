using UnityEngine;
using System.Collections;

public class ReachGoal : BehaviorClass {

	public override void Start ()
	{
		base.Start ();
//		accMag = 500.0f;
//		rayDist = 30.0f;
//		rayDistMax = rayDist;
//		maxRadsDelta = Mathf.Deg2Rad * 20.0f;
		behaviorWeight = 2.0f;
	}

	public override void Update(){
		//updates the maximum speed
		Vector3 targetPosition = goal.transform.position;
		targetAccel = calculateAcceleration (targetPosition);
		veloCloseToTarget ();
		base.Update ();
	}

	//updates maxSpeed
	void veloCloseToTarget () {
		float epsilon = 0.7f;
		float xDistance = Mathf.Abs (transform.position.x - goal.transform.position.x);
		float zDistance = Mathf.Abs (transform.position.z - goal.transform.position.z);
		float distance = Mathf.Sqrt (xDistance * xDistance + zDistance * zDistance);
		
		if (distance <= epsilon) {
			maxSpeed = 0.0f; 
		}
		else { //exponential growth translated up by 10, capped at originalMaxSpeed
			maxSpeed = Mathf.Min(Mathf.Pow(1.1f,distance) + 10.0f, originalMaxSpeed);
		}
	}
}
