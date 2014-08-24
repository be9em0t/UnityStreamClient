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
        private float yVelocity = 0.0F;
        private float zVelocity = 0.0F;
        private float maxSpeed = Mathf.Infinity;

        //camera position
        //private Vector3 camPos;
        //private float camPosX = 25f;
        //private float camPosY = 2f;
        //private float camPosZ = 25f;

        private float cameraRight = 0f;
        private float cameraForward = 0f;
        private float cameraUp = 0f;

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
            float newPositionY = Mathf.SmoothDamp(targetFollow.transform.position.y, target.position.y, ref yVelocity, smoothTime);
            float newPositionZ = Mathf.SmoothDamp(targetFollow.transform.position.z, target.position.z, ref zVelocity, smoothTime);

            targetFollow.transform.position = new Vector3(newPositionX, newPositionY, newPositionZ);
            transform.LookAt(targetFollow.transform, Vector3.up);


            //Change Camera Poition
            GuiInput();
            //camPos = new Vector3(camPosX, camPosY, camPosZ);
            //transform.position = camPos;
            //transform.localPosition = new Vector3(camPosX, camPosY, camPosZ);

            transform.Translate(Vector3.forward * cameraForward);
            transform.Translate(Vector3.right * cameraRight);
            transform.Translate(Vector3.up * cameraUp);


        }

        //Process GUI input booleans
        void GuiInput()
        {

            ////move camera up
            //if (MocapiLiveStream.CameraGUI.High == true) { camPosY = camPosY + 0.1f; }

            ////move camera down
            //if (MocapiLiveStream.CameraGUI.Low == true) { camPosY = camPosY - 0.1f; }

            //move camera up or down
            if (MocapiLiveStream.CameraGUI.High == true) { cameraUp = cameraUp + 0.01f; }
            else if (MocapiLiveStream.CameraGUI.Low == true) { cameraUp = cameraUp - 0.01f; }
            else { cameraUp = 0f; }

            //move camera closer or away
            if (MocapiLiveStream.CameraGUI.Up == true) { cameraForward = cameraForward + 0.01f; }
            else if (MocapiLiveStream.CameraGUI.Down == true) { cameraForward = cameraForward - 0.01f; }
            else { cameraForward = 0f; }

            //move camera right or left
            if (MocapiLiveStream.CameraGUI.Right == true) { cameraRight = cameraRight + 0.01f; }
            else if (MocapiLiveStream.CameraGUI.Left == true) { cameraRight = cameraRight - 0.01f; }
            else { cameraRight = 0f; }


        }

    }
}