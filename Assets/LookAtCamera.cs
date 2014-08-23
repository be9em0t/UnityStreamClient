using UnityEngine;
using System.Collections;

namespace MocapiLiveStream
{
	
public class LookAtCamera : MonoBehaviour {

	public GameObject target;
	public static Vector3 posCamera;
	//public static float zoomly;

	void LateUpdate() {
		transform.LookAt(target.transform);
			//transform.Translate(transform.position.x, transform.position.y, transform.position.z);
		camera.fieldOfView = MocapiLiveStream.Gui.Zoom;
	}


	// Use this for initialization
	void Start () {
			//posCamera = new Vector3(5f,MocapiLiveStream.Gui.CamHeight,5f);  //no such value in Gui anymore
			transform.Translate(posCamera);
			
	}
	
	// Update is called once per frame
	void Update () {
	}
}

}