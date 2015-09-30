using UnityEngine;
using System.Collections;

public class FleeOrWander : NPCBehaviour {

	public GameObject goal;
	private Flee fleeScript;
	private Wander wanderScript;
	private float wanderDistance;

	// Use this for initialization
	public override void Start () {
		fleeScript = GetComponent<Flee>();
		wanderScript = GetComponent<Wander> ();
		base.Start ();
		wanderDistance = 2.0f * rayDist;
		if (Vector3.Distance (transform.position, goal.transform.position) >= wanderDistance) {
			fleeScript.enabled = false;
			wanderScript.enabled = true;
		} else {
			fleeScript.enabled = true;
			wanderScript.enabled = false;
		}
	}
	
	// Update is called once per frame
	public override void Update () {
		if (Vector3.Distance (transform.position, goal.transform.position) >= wanderDistance) {
			fleeScript.enabled = false;
			wanderScript.enabled = true;
		} else {
			fleeScript.enabled = true;
			wanderScript.enabled = false;
		}
	}
}
