using UnityEngine;
using System.Collections;

public class Wander : Behavior {

	public GameObject ground;
	private Mesh mesh;
	private Bounds bound;
	private Vector3 newPos;
	private float radius;
	private int frameCount;

	private Vector3 targetDir;
	private Vector3 tempDir;

	// Use this for initialization
	void Start () {
		base.Start ();
//		targetAccel = new Vector3 ();
//		targetVelo = new Vector3 ();
		newPos = new Vector3 ();
		radius = 1.0f;
		frameCount = 0;
		accMagDefault = 50.0f;
		accMag = accMagDefault;
		mesh = ground.GetComponent<MeshFilter> ().mesh;
		bound = mesh.bounds;

		targetDir = transform.position + transform.forward.normalized * radius;
		tempDir = targetDir;

	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("loss: " + ground.transform.lossyScale);
		//Debug.Log ("loc: " + ground.transform.localScale);
		Debug.Log (goal.transform.position);
		//if (frameCount == 50) {
		//if(Vector3.Distance (transform.position, newPos) < 1.0f || frameCount == 50){
		/*if(Vector3.Distance (transform.position, newPos) < 1.0f){
//			float newAngle = Random.Range (0.0f, 180.0f) * Mathf.Deg2Rad;
//			float theta = Vector3.Angle(transform.right, Vector3.right) * Mathf.Deg2Rad;
//			float newX = transform.position.x + Mathf.Cos (theta + newAngle) * radius;
//			float newZ = transform.position.z + Mathf.Sin (theta + newAngle) * radius;
//			newPos = new Vector3 (newX, 0.0f, newZ);
			//float newX = Random.Range (-(0.5f * bound.size.x * 50.0f), 0.5f * bound.size.x * 50.0f);
			//float newZ = Random.Range (-(0.5f * bound.size.z * 50.0f), 0.5f * bound.size.z * 50.0f);
			float newX = Random.Range (-(ground.transform.localScale.x * 5.0f), ground.transform.localScale.x * 5.0f);
			float newZ = Random.Range (-(ground.transform.localScale.z * 5.0f), ground.transform.localScale.z * 5.0f);
			newPos = new Vector3(newX, 0.0f, newZ);
			frameCount = 0;
		}*/
//		Debug.Log ("newPos: " + newPos + " (bound.size.x/2, bound.size.z/2) : " + (bound.size.x / 2) + (bound.size.z / 2));

		if(Vector3.Angle (tempDir.normalized, targetDir.normalized) < 5.0f) {
			float theta = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
			float newX = Mathf.Cos (theta) * radius;
			float newZ = Mathf.Sin (theta) * radius;
			targetDir = new Vector3(newX, 0.0f, newZ);
			Debug.Log ("resetting");
		}

		tempDir = Vector3.RotateTowards (tempDir, targetDir, Mathf.Deg2Rad * 1.0f, 0.0f);
		Debug.DrawRay (transform.position, tempDir * 500.0f, Color.yellow);
		Debug.DrawRay (transform.position, targetDir * 500.0f, Color.yellow);
		newPos = transform.position + tempDir;

		target = newPos;
		base.Update ();
		frameCount += 1;
	}
}
