using UnityEngine;
using System.Collections;

namespace MocapiLiveStream
{

    public class FixedCamLocal : MonoBehaviour
    {


        private GameObject targetFollow;     //Runtime object that will follow the target and work as camera aim)
        private Transform target;            //the target that we follow

        //damping parameters
        private float smoothTime = .3F;
        private float xVelocity = 0.0F;
        private float zVelocity = 0.0F;
        private float maxSpeed = Mathf.Infinity;

        //camera position
        private Vector3 camPos;
        private float camPosX = 25f;
        private float camPosY = 2f;
        private float camPosZ = 25f;

        void Start()
        {

            targetFollow = new GameObject("targetFollow");              // Create Runtime object that will follow the target and work as camera aim)
            target = GameObject.Find("Hips").transform;                 // Get target avatar's transform

            //targetChild = new GameObject("targetChild");              // ?? 
            //transform.parent = cameraRoot.transform;                  // ??


        }

        // Update is called once per frame
        void Update()
        {

            //Damp position and velocity of the Camera Aim
            float newPositionX = Mathf.SmoothDamp(targetFollow.transform.position.x, target.position.x, ref xVelocity, smoothTime);
            float newPositionZ = Mathf.SmoothDamp(targetFollow.transform.position.z, target.position.z, ref zVelocity, smoothTime);
            targetFollow.transform.position = new Vector3(newPositionX, transform.position.y, newPositionZ);
            transform.LookAt(targetFollow.transform, Vector3.up);

            //Change Camera Poition
            GuiInput();
            //camPos = new Vector3(camPosX, camPosY, camPosZ);
            //transform.position = camPos;
            transform.localPosition = new Vector3(camPosX, camPosY, camPosZ);
        }

        //Process GUI input booleans
        void GuiInput()
        {

            if (MocapiLiveStream.CameraGUI.Up == true) { camPosY = camPosY + 0.1f; }
            if (MocapiLiveStream.CameraGUI.Down == true) { camPosY = camPosY - 0.1f; }
            if (MocapiLiveStream.CameraGUI.Left == true) { camPosZ = camPosZ + 0.1f; }
            if (MocapiLiveStream.CameraGUI.Right == true) { camPosZ = camPosZ - 0.1f; }

        }

    }
}