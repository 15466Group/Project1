using UnityEngine;
using System.Collections;

public class ReachGoal: NPCBehaviour {

	public GameObject goal;

	// Use this for initialization
	public override void Start () {
		base.Start ();
		target = goal.transform.position;
		acceleration = base.calculateAcceleration (target);
		isWanderer = false;
		isReachingGoal = true;
	}

	public override void Update () {
		target = goal.transform.position;
		base.Update ();
	}
}