using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{

    public float speed = 10.0f;
    public Transform cameraTarget;

    private Camera thisCamera;
    private Vector3 worldDefalutForward;

    private void Start()
    {
        thisCamera = GetComponent<Camera>();
        worldDefalutForward = transform.forward;
    }

    public void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * speed;

        thisCamera.fieldOfView += scroll;
    }
}