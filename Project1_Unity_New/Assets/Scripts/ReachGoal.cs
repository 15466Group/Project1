using UnityEngine;
using System.Collections;

public class ReachGoal : BehaviorClass {

	public override void Start ()
	{
		base.Start ();
		accMag = 500.0f;
		behaviorWeight = 2.0f;
	}

	public override void Update(){
		//get the new targetAccel
		targetAccel = calculateAcceleration ();
		targetAccel = targetAccel.normalized;
		targetAccel = targetAccel * accMag;
		//updates the maximum speed
		veloCloseToTarget ();
		base.Update ();
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
			maxSpeed = Mathf.Min(Mathf.Pow(1.1f,distance) + 10.0f, originalMaxSpeed);
		}
	}

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
		
//		Debug.DrawRay(transform.position - transform.right*charWidth, transform.forward.normalized * hitL.distance, Color.magenta);
//		Debug.DrawRay(transform.position + transform.right*charWidth, transform.forward.normalized * hitR.distance, Color.magenta);
//		Debug.DrawRay(transform.position - transform.right*charWidth, (transform.forward - transform.right).normalized * hitLS.distance, Color.magenta);
//		Debug.DrawRay(transform.position + transform.right*charWidth, (transform.forward + transform.right).normalized * hitRS.distance, Color.magenta);
		
		float lDist = (transform.position - transform.right * charWidth - goal.transform.position).magnitude;
		float rDist = (transform.position + transform.right * charWidth - goal.transform.position).magnitude;
		bool rightClose = rDist < lDist;
		float targetDist = Vector3.Distance (transform.position, goal.transform.position);
		
		if (!hitLeft && !hitRight && !hitLeftS && !hitRightS) {
//			Debug.Log ("nothing is hitting");
			return targetAccel;
		} else if (!hitLeft && !hitRight) {
//			Debug.Log ("gostraight" + hitRight + hitLeft);
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
//			Debug.Log ("asdf-1");
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
//					Debug.Log ("asdf0");
					return (transform.forward - transform.right).normalized * accMag;
				} else {
					if (!hitRightS || (targetDist <= hitRS.distance)) {
//						Debug.Log ("asdf1");
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
	}
}
