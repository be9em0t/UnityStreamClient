using UnityEngine;
using System.Collections;

namespace MocapiLiveStream
{
	
public class Player : MonoBehaviour {
	public float movementSpeed = 10;
	public float turningSpeed = 60;
	public static Vector3 scalePlayer;
		
	void Start() {
			scalePlayer = new Vector3(1f,1f,2f);
			
		}
		
	void Update() {
		
		float horizontal = Input.GetAxis("Horizontal") * turningSpeed * Time.deltaTime;
		transform.Rotate(0, horizontal, 0);
		transform.localScale = scalePlayer;
		
		float vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
		transform.Translate(0, 0, vertical);
	}
}

}