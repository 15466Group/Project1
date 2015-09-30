using UnityEngine;
using System.Collections;

public class Wander : NPCBehaviour {

	private Vector3 newPos;
	private float radius;

	private Vector3 targetDir;
	private Vector3 tempDir;
	protected float rotationSpeedDeg { get; set; }

	// Use this for initialization
	public override void Start () {
		base.Start ();
		newPos = new Vector3 ();
		radius = 2.0f;
		accMagDefault = 50.0f;
		rotationSpeedDeg = 1.0f;
		accMag = accMagDefault;

		targetDir = transform.position + transform.forward.normalized * radius;
		tempDir = targetDir;
		isWanderer = true;

	}
	
	// Update is called once per frame
	public override void Update () {
		//choosing a new position to accelerate towards
		if(Vector3.Angle (tempDir.normalized, targetDir.normalized) < 5.0f) {
			float theta = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
			float newX = Mathf.Cos (theta) * radius;
			float newZ = Mathf.Sin (theta) * radius;
			targetDir = new Vector3(newX, 0.0f, newZ);
		}

		//for smooth turning, we know the new position we want to get to, choose points along a radius
		//to turn towards that new point until the tempDir and the targetDir are about the same
		tempDir = Vector3.RotateTowards (tempDir, targetDir, Mathf.Deg2Rad * rotationSpeedDeg, 0.0f);
		Debug.DrawRay (transform.position, tempDir.normalized * rayDist * 1.25f, Color.yellow);
		Debug.DrawRay (transform.position, targetDir.normalized * rayDist * 1.25f, Color.yellow);
		newPos = transform.position + tempDir;

		target = newPos;
		base.Update ();
	}
}
