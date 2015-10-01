using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public GameObject cameraLead;
	private Vector3 offset = new Vector3 (0, 200, -50);
	
	void Start()
	{
		
	}
	void Update()
	{
		/*float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		
		Vector3 velocity = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		velocity = velocity * 100.0f;
		transform.position += velocity * Time.deltaTime;*/

		transform.position = cameraLead.transform.position + offset;
		transform.forward = cameraLead.transform.position - transform.position;
	}
}
