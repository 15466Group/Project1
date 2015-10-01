using UnityEngine;
using System.Collections;

public class WanderVLeader : Wander {
		
	// Use this for initialization
	public override void Start () {
		base.Start ();
		rotationSpeedDegDefault= 0.1f;
		speedMaxDefault = 20.0f;
		
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}
}
