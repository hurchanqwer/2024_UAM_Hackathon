using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    public GameObject[] objects;
    private GameObject pendingobject;
    private Vector3 pos;
    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;
    public Transform FBXParent;
    public float rotateAmount;

    public float gridSize;
    public bool gridOn;
    [SerializeField] private Toggle gridToggle;


    void Start()
    {
        if (gridToggle == null)
        {
            gridToggle = FindObjectOfType<Toggle>();
            gridToggle.isOn = false;
            gridOn = true;
            gridToggle.isOn=true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (pendingobject != null) 
        {
            if (gridOn)
            {
                pendingobject.transform.position = new Vector3(
                RoundToNearestGird(pos.x),
                RoundToNearestGird(pos.y),
                RoundToNearestGird(pos.z)
                );
            }
            else { pendingobject.transform.position = pos; }

            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject();
            }
            if (Input.GetKeyDown(KeyCode.R))
            { 
            RotateObject();
            }

        }
    }

    public void PlaceObject()
    {
        pendingobject = null;
    }
    public void RotateObject()
    {
        pendingobject.transform.Rotate(Vector3.up, rotateAmount);
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            pos = hit.point;
        }
    }
    public void SelectObject(int index)
    {
        pendingobject = Instantiate(objects[index], pos ,transform.rotation, FBXParent);
    }

    public void ToggleGrid()
    {
        if (gridToggle != null)
        {
            gridOn = gridToggle.isOn;
        }
        else
        {
            Debug.LogWarning("gridToggle is not assigned.");
        }
    }
    float RoundToNearestGird(float pos) 
    {
        float xDiff = pos % gridSize;
        pos -= xDiff;
        if (xDiff > (gridSize / 2))
        {
            pos += gridSize;
        }
        return pos;
    }
}
