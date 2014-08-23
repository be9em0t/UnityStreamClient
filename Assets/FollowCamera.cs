using UnityEngine;
using System.Collections;

namespace MocapiLiveStream
{
	
	
public class FollowCamera : MonoBehaviour {
	public GameObject target;
	public float damping = .1f;
	Vector3 offset;
	
	void Start() {
		offset = target.transform.position - transform.position;
	}
	
	void LateUpdate() {
		//float desiredAngle = target.transform.eulerAngles.y;
		//Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);

		float currentAngle = transform.eulerAngles.y;
		float desiredAngle = target.transform.eulerAngles.y;
		//float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);
		float angle = Mathf.LerpAngle(currentAngle, desiredAngle, damping);

		Quaternion rotation = Quaternion.Euler(0, angle, 0);

		transform.position = target.transform.position - (rotation * offset);
		
		transform.LookAt(target.transform);
	}
}

}