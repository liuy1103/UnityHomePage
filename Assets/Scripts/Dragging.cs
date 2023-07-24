using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 


public class Dragging : MonoBehaviour
{
    private float dist;
    private bool dragging = false;
    private Vector3 offset;
    private Transform toDrag;

    public Material newMaterial1;
    public Material newMaterial2;
    private Dictionary<Renderer, Material> originalMaterials;

    void Start()
    {
        this.enabled = false;
    }

    void Update()
    {
        Vector3 v3;

        if (Input.touchCount != 1)
        {
            dragging = false;
            RestoreMaterials(toDrag);
            return;
        }

        Touch touch = Input.touches[0];
        Vector3 pos = touch.position;

        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) && touch.phase != TouchPhase.Began)
        {
            return;
        }

        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Furniture")
            {
                toDrag = hit.transform;

                //save original material
                originalMaterials = GetAllMaterials(toDrag);
                //change material to new one
                SetMaterialRecursively(toDrag, newMaterial1);

                dist = hit.transform.position.z - Camera.main.transform.position.z;
                v3 = new Vector3(pos.x, pos.y, dist);
                v3 = Camera.main.ScreenToWorldPoint(v3);
                offset = toDrag.position - v3;
                dragging = true;
            }
        }

        if (dragging && touch.phase == TouchPhase.Moved)
        {
            v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            v3 = Camera.main.ScreenToWorldPoint(v3);
            
            // Lock the Y position to the original Y position of the object
            Vector3 targetPosition = v3 + offset;
            targetPosition.y = toDrag.position.y; 

            float smoothSpeed = 0.25f;  // Adjust this to change the smoothing factor
            toDrag.position = Vector3.Lerp(toDrag.position, targetPosition, smoothSpeed);

            // Check if the object is colliding with a wall or furniture
            bool collision = CheckCollision(toDrag.gameObject);

            if (collision)
            {
                SetMaterialRecursively(toDrag, newMaterial2);
            }
            else
            {
                SetMaterialRecursively(toDrag, newMaterial1);
            }
        }
    

        if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            // Revert to the original material when the touch ends
            RestoreMaterials(toDrag);
            dragging = false;
        }
    }

    void OnEnable()
    {
        dragging = false;
}

    void OnDisable()
    {
        dragging = false;
    }

    private void SetMaterialRecursively(Transform t, Material material)
    {
        Renderer renderer = t.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        foreach (Transform child in t)
        {
            SetMaterialRecursively(child, material);
        }
    }

    private Dictionary<Renderer, Material> GetAllMaterials(Transform t)
    {
        Dictionary<Renderer, Material> materials = new Dictionary<Renderer, Material>();
        Renderer renderer = t.GetComponent<Renderer>();
        if (renderer != null)
        {
            materials[renderer] = renderer.material;
        }

        foreach (Transform child in t)
        {
            Dictionary<Renderer, Material> childMaterials = GetAllMaterials(child);
            foreach (KeyValuePair<Renderer, Material> pair in childMaterials)
            {
                materials[pair.Key] = pair.Value;
            }
        }
        return materials;
    }

    private void RestoreMaterials(Transform t)
    {
        if (t != null && originalMaterials != null)
        {
            foreach (KeyValuePair<Renderer, Material> pair in originalMaterials)
            {
                pair.Key.material = pair.Value;
            }
            originalMaterials.Clear();
        }
    }

    private bool CheckCollision(GameObject obj)
    {
        // Adjust this value to change the collision tolerance
        //float collisionTolerance = 0.9f;
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();

        foreach (var collider in colliders)
        {
            Collider[] hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation, ~0, QueryTriggerInteraction.Ignore);
            foreach (var hitCollider in hitColliders)
            {
                if ((hitCollider.gameObject.CompareTag("Wall") || hitCollider.gameObject.CompareTag("Furniture")) && hitCollider.transform != toDrag)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
