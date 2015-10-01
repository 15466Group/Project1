using UnityEngine;
using System.Collections;

public class GoalControl : MonoBehaviour {
	
	
	
	void Start()
	{
		
	}
	void Update()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		
		Vector3 velocity = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		velocity = velocity * 100.0f;
		transform.position += velocity * Time.deltaTime;
		
	}
}
