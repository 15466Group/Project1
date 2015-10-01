using UnityEngine;
using System.Collections;

public class FleeOrWander : NPCBehaviour {

	public GameObject goal;
	private Flee fleeScript;
	private Wander wanderScript;
	private float fleeDist;
	private float interDist;
	private float wanderDist;
	private float actualDist;

	//indicates the last zone the guy was in, 0 for flee zone and 1 for wander zone
	private int lastZone;

	// Use this for initialization
	public override void Start () {
		fleeScript = GetComponent<Flee>();
		wanderScript = GetComponent<Wander> ();
		base.Start ();
		fleeDist = 3.0f * rayDist;
		interDist = 6.0f * rayDist;

		actualDist = Vector3.Distance (transform.position, goal.transform.position);

		if (actualDist < fleeDist) {
			lastZone = 0;
//			wanderScript.restrictForward (true);
//			wanderScript.biasOverride = transform.position - goal.transform.position;
			fleeScript.enabled = true;
			wanderScript.enabled = false;
		} else if (actualDist < interDist) {
			//biasDir = (transform.position - goal.transform.position).normalized;
			//overridingBias = true;
//			wanderScript.enabled = true;
//			fleeScript.enabled = false;
			if(lastZone == 0) {
				fleeScript.enabled = true;
				wanderScript.enabled = false;
			}
			else {
				fleeScript.enabled = false;
				wanderScript.enabled = true;
			}
		} else {
			//biasDir = Vector3.zero;
			//overridingBias = false;
//			wanderScript.restrictForward(false);
			lastZone = 1;
			fleeScript.enabled = false;
			wanderScript.enabled = true;
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (transform.position, fleeDist);
		Gizmos.DrawWireSphere (transform.position, interDist);
	}

	// Update is called once per frame
	public override void Update () {
		actualDist = Vector3.Distance (transform.position, goal.transform.position);
		
		if (actualDist < fleeDist) {
			lastZone = 0;
//			wanderScript.restrictForward(true);
//			wanderScript.biasOverride = transform.position - goal.transform.position;
			fleeScript.enabled = true;
			wanderScript.enabled = false;
		} else if (actualDist < interDist) {
			//biasDir = (transform.position - goal.transform.position).normalized;
			//overridingBias = true;
//			wanderScript.enabled = true;
//			fleeScript.enabled = false;
			if(lastZone == 0) {
				fleeScript.enabled = true;
				wanderScript.enabled = false;
			}
			else {
				fleeScript.enabled = false;
				wanderScript.enabled = true;
			}
		} else {
			//biasDir = Vector3.zero;
			//overridingBias = false;
//			wanderScript.restrictForward(false);
			lastZone = 1;
			fleeScript.enabled = false;
			wanderScript.enabled = true;
		}
	}
}
