using UnityEngine;
using System.Collections;

public class ReachGoal : MonoBehaviour {

	public GameObject target;

	private Vector3 acceleration;
	private Vector3 velocity;
	private float accMag;
	private float speedMax;

	private float lambda; //angle from target to character
	private float psi; //direction character is currently facing

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
