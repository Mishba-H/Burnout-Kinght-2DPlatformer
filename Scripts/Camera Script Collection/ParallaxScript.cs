using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    [SerializeField] private Vector2 parallaxEffectMultiplier;

    private void Start() 
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;    
    }

    void LateUpdate() 
    {
        Vector3 deltaCameraPosition = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaCameraPosition.x * parallaxEffectMultiplier.x,
            deltaCameraPosition.y * parallaxEffectMultiplier.y, 0);
        lastCameraPosition = cameraTransform.position;
    }
}
