using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CameraMovement : MonoBehaviour
{
    public float rotateSpeed = 1f;
    public float yaw = 0;
    public float pitch = 67f;
    public float distance = 9.5f;
    public float yawSpeed = .5f;
    public float pitchSpeed = .5f;
    public float pitchMax = 89.9f;
    public float pitchMin = 5f;
    public AnimationCurve scrollSpeedCurve;
    public float distanceMax = 15f;
    public float distanceMin = 1f;
    public float scrollSmoothTime = 0.1f;

    Camera cam;
    bool isDraggin = false;
    Vector3 dragStart;
    bool isRotating = false;
    Vector3 lastMousePosition;
    float targetDistance;
    float scrollSmoothSpeed = 0;
    Rect mapRect;

    public event Action OnChange;

    private static CameraMovement _instance;
    public static CameraMovement instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<CameraMovement>();
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        cam = Camera.main;
    }

    // Use this for initialization
    void Start()
    {
        targetDistance = distance;
        ApplyRotation();
        mapRect = MapController.instance.GetGridAsRect();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject() || isDraggin || isRotating)
        {
            // start a move
            if (Input.GetMouseButtonDown(1))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // rotation start
                    lastMousePosition = Input.mousePosition;
                    isRotating = true;

                }
                else
                {
                    // drag start
                    dragStart = GetMouseOnPlane();
                    isDraggin = true;
                }
            }

            if (isDraggin)
            {
                Vector3 point = GetMouseOnPlane();
                Vector3 delta = dragStart - point;
                
                transform.position = ClampPointIntoMapRect(transform.position + new Vector3(delta.x, 0, delta.z));
            }
            else if (isRotating)
            {
                yaw += (lastMousePosition.x - Input.mousePosition.x) * yawSpeed;
                pitch += (lastMousePosition.y - Input.mousePosition.y) * pitchSpeed;

                pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

                ApplyRotation();

                lastMousePosition = Input.mousePosition;

            }

            // scroll
            if (Input.mouseScrollDelta.y != 0 || scrollSmoothSpeed != 0) {

                targetDistance -= Input.mouseScrollDelta.y * scrollSpeedCurve.Evaluate(Mathf.InverseLerp(distanceMin, distanceMax, targetDistance));
                targetDistance = Mathf.Clamp(targetDistance, distanceMin, distanceMax);

                distance = Mathf.SmoothDamp(distance, targetDistance, ref scrollSmoothSpeed, scrollSmoothTime);

                ApplyRotation();
            }

            // notify listeners
            if (scrollSmoothSpeed != 0 || isDraggin || isRotating)
            {
                if (OnChange != null) OnChange();
            }

            if (Input.GetMouseButtonUp(1))
            {
                isDraggin = false;
                isRotating = false;
            }
        }
    }

    Vector3 ClampPointIntoMapRect(Vector3 point)
    {
        point.x = Mathf.Clamp(point.x, mapRect.xMin, mapRect.xMax);
        point.z = Mathf.Clamp(point.z, mapRect.yMin, mapRect.yMax);

        return point;
    } 

    void ApplyRotation()
    {
        // yaw, pitch, distance
        cam.transform.position = transform.position;
        cam.transform.eulerAngles = new Vector3(-pitch, -yaw);
        cam.transform.position = cam.transform.position + cam.transform.forward * distance;
        cam.transform.LookAt(transform.position);
    }

    Vector3 GetMouseOnPlane(Vector3 pixelPosition)
    {
        Ray ray = cam.ScreenPointToRay(pixelPosition);
        float dist = 0;

        Plane plane = new Plane(Vector3.up, 0);

        if (plane.Raycast(ray, out dist))
        {
            Vector3 point = ray.origin + ray.direction * dist;

            return point;
        }

        // How was the plane not hit???
        return new Vector3();
    }

    Vector3 GetMouseOnPlane()
    {
        return GetMouseOnPlane(Input.mousePosition);
    }
}
