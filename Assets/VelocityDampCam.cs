using UnityEngine;
using System.Collections;

public class VelocityDampCam : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 3F;

    private float xVelocity = 0.0F;
    private float zVelocity = 0.0F;

    private float maxSpeed = Mathf.Infinity;


        void Update()
    {
        //float newPositionX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref yVelocity, smoothTime);
        //float newPositionY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref yVelocity, smoothTime);
        //float newPositionZ = Mathf.SmoothDamp(transform.position.z, target.position.z, ref yVelocity, smoothTime);

        //float newPositionX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref xVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
        //float newPositionZ = Mathf.SmoothDamp(transform.position.z, target.position.z, ref zVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);

        //float newPositionX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref xVelocity, smoothTime*reactionSpeed, maxSpeed, Time.deltaTime);
        //float newPositionZ = Mathf.SmoothDamp(transform.position.z, target.position.z, ref zVelocity, smoothTime*reactionSpeed, maxSpeed, Time.deltaTime);

        float newPositionX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref xVelocity, smoothTime);
        float newPositionZ = Mathf.SmoothDamp(transform.position.z, target.position.z, ref zVelocity, smoothTime);


        transform.position = new Vector3(newPositionX, transform.position.y, newPositionZ );
    }


}