﻿using UnityEngine;
using System.Collections;

public class WanderVLeader : Wander {
		
	// Use this for initialization
	public override void Start () {
		base.Start ();
		rotationSpeedDeg = 0.5f;
		
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}
}