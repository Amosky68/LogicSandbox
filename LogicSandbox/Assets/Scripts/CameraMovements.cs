using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovements : MonoBehaviour
{

    public Camera maincamera;

    public float mouseScrollSpeed = 1f;
    public float mouseMoveSpeed = 1f;
    public float SmoothZoomSpeed = 7f;
    public float SmoothCenterSpeed = 7f;
    public float MinZoomValue = 0.2f;
    public float MaxZoomValue = 15f;

    public bool followPlayer = true;
    public GameObject Player;

    private Vector3 dragOrigin;
    public float Tozoomvalue;
    public Vector3 ToCenter;

    public int MouseButton;


    // Start is called before the first frame update
    void Start()
    {
        maincamera.orthographic = true;
        Tozoomvalue = maincamera.orthographicSize;
    }



    // Update is called once per frame
    void Update()
    {

        if (followPlayer)
        {
            ToCenter = Player.transform.position;
        }

        ApplyDrag();
        CalculateZoom();
        ApplyZoom();
        ApplyCameraCenter();

    }


    void ApplyDrag()
    {
        if (Input.GetMouseButtonDown(MouseButton)) { 
            dragOrigin = maincamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(MouseButton)) {
            Vector3 difference = dragOrigin - maincamera.ScreenToWorldPoint(Input.mousePosition);
            ToCenter = maincamera.transform.position + difference;
        }
    }

    void CalculateZoom() {
        float deltazoomvalue = Input.mouseScrollDelta.y;
        if (deltazoomvalue != 0f && ! Input.GetKey(KeyCode.LeftControl))
        {

            float newZvalue = Tozoomvalue;
            newZvalue *= (float)System.Math.Pow((1 - 0.05d * mouseScrollSpeed), (double)deltazoomvalue);

            Tozoomvalue = Mathf.Clamp(newZvalue, MinZoomValue, MaxZoomValue);
        }
    }

    void ApplyZoom()
    {
        float orthographicSizeDifference = Tozoomvalue - maincamera.orthographicSize;
        maincamera.orthographicSize += orthographicSizeDifference * SmoothZoomSpeed * Time.deltaTime;
    }

    void ApplyCameraCenter()
    {
        if (ToCenter != null)
        {
            Vector3 CentersDifference = ToCenter - maincamera.transform.position;
            maincamera.transform.position += CentersDifference * SmoothCenterSpeed * Time.deltaTime;
            maincamera.transform.position = new Vector3(maincamera.transform.position.x , maincamera.transform.position.y , -10f);

        }
    }

}