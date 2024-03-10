using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomScale = 1f;

    void Update()
    {
        Vector2 scrollVector = Input.mouseScrollDelta;
        mainCamera.orthographicSize -= scrollVector.y * zoomScale;
    }
}
