using UnityEngine;
using System.Collections;

public class Wander : BehaviorClass {

	// Use this for initialization
	public override void Start (){
		base.Start ();
		accMag = 50.0f;
		maxRadsDelta = Mathf.Deg2Rad * 1.0f;
		behaviorWeight = 1.0f;
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
		base.Update ();
	}
}
