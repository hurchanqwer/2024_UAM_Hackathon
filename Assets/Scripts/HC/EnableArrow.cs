using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableArrow : MonoBehaviour
{
    
    public GameObject arrow;
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("EnableArea") && !SystemManager.Instance.isVisiable)
        {
            arrow.SetActive(true);
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnableArea")&& !SystemManager.Instance.isVisiable)
        {
        
            arrow.SetActive(false);
        }
    }
}
