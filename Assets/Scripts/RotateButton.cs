using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateButton : MonoBehaviour
{
    // This bool will keep track of whether rotation is currently enabled.
    private bool rotationEnabled = false;

    // The rotation amount in degrees.
    private float rotationAmount = 90f;

    // This is called when the button is pressed.
    public void ToggleRotation()
    {
        // Flip the rotationEnabled bool.
        rotationEnabled = !rotationEnabled;
    }

    private void Update()
    {
        // Only check for touch input if rotation is enabled.
        if (rotationEnabled)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Only act on the touch phase began, to avoid continuous rotation.
                if (touch.phase == TouchPhase.Began)
                {
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            // Check that the hit object has the Furniture tag.
                            if (hit.collider.gameObject.CompareTag("Furniture"))
                            {
                                // Rotate the object around the Y axis.
                                hit.collider.gameObject.transform.Rotate(0f, rotationAmount, 0f);
                            }
                        }
                    }
                }
            }
        }
    }
}
