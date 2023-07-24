using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The object that the camera will rotate around
    private float rotationSpeed = 8f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private float zoomSpeed = 0.5f; // Speed of zooming
    private float minFov = 40f; // Min field of view
    private float maxFov = 85f; // Max field of view
    private float prevTouchDelta = 0f;

    private bool wasZoomingLastFrame = false;
    public bool rotationEnabled = true; // Add this line

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1 && rotationEnabled)
        {
            // Get touch input
            Touch touch = Input.GetTouch(0);

            // Calculate rotation according to touch movement
            xRotation -= touch.deltaPosition.y * rotationSpeed * Time.deltaTime;
            yRotation += touch.deltaPosition.x * rotationSpeed * Time.deltaTime;

            // Clamp xRotation to not over-rotate the camera
            xRotation = Mathf.Clamp(xRotation, -50f, 50f);

            // Apply rotation to the camera
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

            // Make sure the camera is always looking at the target
            transform.position = target.position - transform.forward * 20; // 10 is the distance between the camera and the target

            wasZoomingLastFrame = false; 
        }

        if (Input.touchCount == 2)
        {
            // If there are two touches on the device...
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float touchDelta = (touchZero.position - touchOne.position).magnitude;

            if (wasZoomingLastFrame)
            {
                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDelta - touchDelta;

                // ... change the field of view based on the change in distance between the touches.
                Camera.main.fieldOfView += deltaMagnitudeDiff * zoomSpeed;

                // Clamp the field of view to make sure it's between minFov and maxFov.
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minFov, maxFov);
            }

            // Store the current touch delta magnitude for the next frame
            prevTouchDelta = touchDelta;

            wasZoomingLastFrame = true; 
        }
        else if (wasZoomingLastFrame)
        {
            // If the last frame was a zooming gesture, reset prevTouchDelta
            prevTouchDelta = 0;
            wasZoomingLastFrame = false;
        }
    }
}
