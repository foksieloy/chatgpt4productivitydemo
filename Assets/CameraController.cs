using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public float zoomSpeed = 500f;
    public Vector2 zoomLimits = new(5f, 15f);

    private Camera cam;

    public float initialZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Confined;

        // Set the initial zoom level
        Camera.main.orthographicSize = initialZoom;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(newZoom, zoomLimits.x, zoomLimits.y);

        transform.position = pos;

        // Panning with touch input
        if (Input.touchSupported && Input.touchCount == 1 && !CheckPointerOverUI())
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 deltaPosition = cam.ScreenToWorldPoint(touch.position - touch.deltaPosition) - cam.ScreenToWorldPoint(touch.position);
                deltaPosition.z = 0;
                transform.position += deltaPosition;
            }
        }

        // Panning with arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 localVector3 = new(horizontal, vertical, 0);
        transform.position += panSpeed * Time.deltaTime * localVector3;

        // Center on the biggest GrazingCreature with a mouse click or touch input
        if ((Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space) || (Input.touchSupported && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)) && !CheckPointerOverUI())
        {
            CenterOnBiggestCreature();
        }
    }

    GameObject FindBiggestCreature()
    {
        GameObject[] grazingCreatures = GameObject.FindGameObjectsWithTag("GrazingCreature");
        GameObject[] hunterCreatures = GameObject.FindGameObjectsWithTag("HunterCreature");
        GameObject biggestCreature = null;
        float maxFoodRating = 0f;

        if (hunterCreatures.Length > 0)
        {
            foreach (GameObject hunterCreature in hunterCreatures)
            {
                HunterCreatureController hunterController = hunterCreature.GetComponent<HunterCreatureController>();
                if (hunterController.FoodRating > maxFoodRating)
                {
                    maxFoodRating = hunterController.FoodRating;
                    biggestCreature = hunterCreature;
                }
            }
        }
        else
        {
            foreach (GameObject grazingCreature in grazingCreatures)
            {
                GrazingCreatureController grazingController = grazingCreature.GetComponent<GrazingCreatureController>();
                if (grazingController.FoodRating > maxFoodRating)
                {
                    maxFoodRating = grazingController.FoodRating;
                    biggestCreature = grazingCreature;
                }
            }
        }

        return biggestCreature;
    }

    bool CheckPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject() || (Input.touchSupported && Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));
    }

    void CenterOnBiggestCreature()
    {
        GameObject biggestCreature = FindBiggestCreature();
        if (biggestCreature != null)
        {
            Vector3 targetPosition = new(biggestCreature.transform.position.x, biggestCreature.transform.position.y, transform.position.z);
            transform.position = targetPosition;
        }
    }

}
