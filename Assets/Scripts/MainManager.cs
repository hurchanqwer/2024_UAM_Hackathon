using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MainManager : MonoBehaviour
{
    public XRRayInteractor rightRayInteractor;
    public Transform player;
    public Transform start;
    public Transform destination;
    public Transform UAM;
    public Transform shadow;
    public Transform current;
    Transform CreatedUAM;
    public VectorFieldManager vectorFieldManager;
  
    public Octree octree;
    //if (RightActivate.action.ReadValue<float>() > 0.1f) 
    public int flag = 0;
    WaitForSeconds wait_0_1;
    // Start is called before the first frame update

    void OnEnable()
    {
        
        flag = 0;
        octree.gameObject.SetActive(false);
        player.position = new Vector3(705, 889, -670);
        player.rotation = Quaternion.Euler(0, 180, 0);
        StartCoroutine("DesSet");
    }

    private void OnDisable()
    {
        
        StopAllCoroutines();
    }
    public IEnumerator DesSet()
    {
        while (flag == 0)
        {
            if (rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                if (current == null)
                {
                    current = Instantiate(destination, hit.point, Quaternion.identity);
                }
                else { current.position = hit.point; }
            }

            yield return wait_0_1;
        }
        current = null;
        yield return 0;
    }

    public void OnActivate()
    {
        if (flag > 1) return;
        flag++;
        if (flag == 1)
        {
            StopCoroutine(DesSet());
            StartCoroutine(StartSet());
        }
        else if (flag == 2)
        {
            StartCoroutine(MainSet());
        }

    }
    public IEnumerator MainSet()
    {
        StopCoroutine(StartSet());
        octree.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        CreatedUAM = Instantiate(UAM, current.position, Quaternion.identity);
        Instantiate(shadow, current.position, Quaternion.identity);
        player.parent = CreatedUAM.transform;
        player.transform.localPosition = new Vector3(0, 2, -4);
        player.transform.localRotation = Quaternion.Euler(13, 0, 0);

        Destroy(current.gameObject);
        current = null;
        vectorFieldManager.CreateVectorField();
       
    
    }
    public IEnumerator StartSet()
    {

        while (flag == 1)
        { // Ray가 3D 오브젝트와 충돌했는지 확인
            if (rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                if (current == null)
                {
                    current = Instantiate(start, hit.point, Quaternion.identity);
                }
                else { current.position = hit.point; }
            }
            yield return wait_0_1;
        }

        yield return 0;
    }

    public void VisiableMode()
    {
        vectorFieldManager.VectorVisiableMode();
    }


    public void CamFix()
    {
      
        if (transform.parent == null)
            {
                player.parent = CreatedUAM;
                player.localPosition = new Vector3(0, 2, -4);
                player.localRotation = Quaternion.Euler(13, 0, 0);
        }
         else { player.transform.parent = null; }
       
    }
    private void Update()
    {
        //촬영용
        if (Input.GetKeyDown(KeyCode.V))
        {
            VisiableMode();
        }
    }

}




