using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveBtn : MonoBehaviour, IPointerClickHandler
{
    public CameraController cameraController; // Reference to the camera controller

    public Button yourButton;
    public Dragging draggingScript;
    private bool toggle;

    private void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        //disable dragging initially
        draggingScript.enabled = false;
        cameraController.rotationEnabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)  
    {
        Invoke("ToggleDrag", 0.1f);
    }

    public void ToggleDrag()
    {
        Debug.Log("Before toggle: " + draggingScript.enabled);
        draggingScript.enabled = !draggingScript.enabled;
        Debug.Log("After toggle: " + draggingScript.enabled);
        cameraController.rotationEnabled = !draggingScript.enabled;
    }
}
