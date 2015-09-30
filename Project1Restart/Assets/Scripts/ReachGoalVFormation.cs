using UnityEngine;
using System.Collections;

public class ReachGoalVFormation: Behavior {
	
	public GameObject goal;
	private static int counter = 0;
	private int VFormID;
	
	// Use this for initialization
	public override void Start () {
		base.Start ();
		counter ++;
		VFormID = counter;
		Debug.Log ("ID " + VFormID + " assigned");
		target = calculateVPosition ();
		acceleration = base.calculateAcceleration (target);
		isWanderer = false;
		speedMaxDefault = 50.0f;
		accMagDefault = 500.0f;
		rayDistDefault = 50.0f;
		closeRayDistDefault = 20.0f;
	}
	
	public override void Update () {
		target = calculateVPosition ();
		base.Update ();
	}

	Vector3 calculateVPosition () {
		Vector3 goalBackwards = (-1.0f) * goal.transform.forward.normalized;
		Vector3 goalRight = goal.transform.right.normalized;
		Vector3 goalLeft = (-1.0f) * goal.transform.right.normalized;
		if (VFormID % 2 == 0) {
			return goal.transform.position + Vector3.ClampMagnitude((goalBackwards + goalRight).normalized * 10000.0f, (VFormID / 2) * (rayDistDefault * 2.0f/3.0f));
		} else {
			return goal.transform.position + Vector3.ClampMagnitude((goalBackwards + goalLeft).normalized * 10000.0f, ((VFormID + 1) / 2) * (rayDistDefault * 2.0f/3.0f));
		}
	}
}