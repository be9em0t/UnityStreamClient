using UnityEngine;
using System.Collections;

namespace MocapiLiveStream
{
	
public class DungeonCamera : MonoBehaviour {
	public GameObject target;
	Vector3 offset;
	public float damping = 1;
	
	void Start() {
		offset = transform.position - target.transform.position;
	}

	void LateUpdate_Direct() {
		Vector3 desiredPosition = target.transform.position + offset;
		transform.position = desiredPosition;
	}

	void LateUpdate() {
				Vector3 desiredPosition = target.transform.position + offset;
				Vector3 position = Vector3.Lerp (transform.position, desiredPosition, Time.deltaTime * damping);
				transform.position = position;

				transform.LookAt(target.transform.position);
		}

}

}