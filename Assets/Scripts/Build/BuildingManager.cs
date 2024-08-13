using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static Octree.OctreeElement;

public class BuildingManager : MonoBehaviour
{
    public GameObject[] objects;
    private GameObject pendingobject;
    private Vector3 pos;

    [SerializeField] private LayerMask layerMask;
    public Transform FBXParent;
    public float rotateAmount;

    public float gridSize;
    public bool gridOn;
    [SerializeField] private Toggle gridToggle;

    public XRRayInteractor RightInteractor;
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
        if(RightInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)&& hit.transform.Equals("Ground"));
        {
            Debug.Log(hit.transform.name);
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


    public void ChooseButton()
    {
        Debug.Log("1");
        if (RightInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            switch (hit.transform.name) {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    SelectObject(int.Parse(hit.transform.name));
                    break; 
           
               case "Ground": 
                     { PlaceObject();  break; }
                case "Grid":
                    ToggleGrid();
                    break;
            }
        }
    }
}
