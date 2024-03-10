using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private float zoomScale = 1f;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        Vector2 scrollVector = Input.mouseScrollDelta;
        mainCamera.orthographicSize -= scrollVector.y * zoomScale;

        Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += moveVector;
    }
}
