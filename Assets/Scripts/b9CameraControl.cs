using UnityEngine;
using System.Collections;
using System;

public class b9CameraControl : MonoBehaviour {
	
	float smooth = 5f;	            // 0 - infinity, 10 almost instant, default 2 - a public variable to adjust smoothing of camera motion
	float camDist = -2.5f;          //Initial Camera distance
	float camHeight = .8f;          //Initial Camera height
	float rotAround = 0f;           //cameraParent rotation around target
	float vertOffset = 0f;          //camera vertial offset
	float cameraZoom = 60f;         //camera FieldOfView
	
	Transform avatarTransf;         //target avatar's transform
	//Transform avatarLookAt;         //target avatar's hips
	
	Vector3 camOffset;              //camera offset
	Vector3 targetPos;              //target offset
	Vector3 LookAtTarget;
	
	public GameObject cameraParent;     //camera's parent object
    public string test;

	void Start () {
		//Create camera hierarchy

        test = (cameraParent.name).ToString();
        //Debug.Log(test);


        cameraParent = new GameObject("cameraPivot");              //create camera's parent object at 0,0,0
		camOffset = new Vector3(0f, camHeight, camDist);                   //define the camera offset
		transform.position = camOffset;                             //reposition camera relative to 0,0,0
		transform.parent = cameraParent.transform;                  //parent camera to cameraParent, so that it follows avatar

        //Debug.Log(cameraParent.name);

        //avatarTransf = GameObject.Find("CubePlayer").transform;  //get target avatar's transform
        avatarTransf = GameObject.Find(test).transform;  //get target avatar's transform
        //avatarTransf = GameObject.Find((cameraParent.name).ToString()).transform;  //get target avatar's transform
		
	}
	
	void Update () {
		//follow Avatar with Camera 
		targetPos= new Vector3(avatarTransf.position.x, avatarTransf.position.y-vertOffset, avatarTransf.position.z);
		cameraParent.transform.position = Vector3.Lerp(cameraParent.transform.position, targetPos, smooth * Time.deltaTime); 
		cameraParent.transform.rotation = Quaternion.Slerp(cameraParent.transform.rotation, avatarTransf.rotation * Quaternion.Euler(0f, rotAround, 0f), smooth * Time.deltaTime); 
		
		//zoom Camera in and out
		camera.fieldOfView = cameraZoom;
		
		//tweak Camera up and down 
		LookAtTarget = new Vector3(avatarTransf.position.x, avatarTransf.position.y + camHeight + (vertOffset/10), avatarTransf.position.z);
		transform.LookAt(LookAtTarget, Vector3.up);
		
		
		PositionChange();
	}
	
	//Camera Control
	void PositionChange () {
		//Rotate Camera Around
		if (Input.GetKey(KeyCode.Period))
		{
			rotAround = rotAround - (100f * Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.Comma))
		{
			rotAround = rotAround + (100f * Time.deltaTime);
		}
		//Camera up and down
		if (Input.GetKey(KeyCode.Quote))
		{
			vertOffset=vertOffset-(1 * Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.Slash))
		{
			vertOffset=vertOffset+(1 * Time.deltaTime);
		}
		//Camera Zoom
		if (Input.GetKey(KeyCode.PageUp))
		{
			cameraZoom = cameraZoom - (10 * Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.PageDown))
		{
			cameraZoom = cameraZoom + (10 * Time.deltaTime);
		}
		//Reset Camera
		if (Input.GetKey(KeyCode.Home))
		{
			rotAround = 0f;
			vertOffset = 0f;
			cameraZoom = 60f;
		}
		
	}
	
}