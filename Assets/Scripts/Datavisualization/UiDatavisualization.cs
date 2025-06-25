using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class UiDatavisualization : MonoBehaviour
{
    public TextMeshProUGUI textspeed; 
    public TextMeshProUGUI Altitude;
    public TextMeshProUGUI textTime;

    public float time;
    public float currentAltitude;

    private Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 

    }

    void Update()
    {
        
            Vector3 velocity = rb.velocity; 
            double speed = velocity.magnitude*3.6*3; 
         
            textspeed.text = "Wind : " + speed + " km/h";
    
        currentAltitude  = transform.position.y - 20;
        Altitude.text = "Altitude : " + currentAltitude+ "meters";

        time += Time.deltaTime;
 
          int minutes = Mathf.FloorToInt(time / 60f); 
        int sec = Mathf.FloorToInt(time % 60f);

          textTime.text = "Time : " + minutes  + "m " + sec+"s";
        }
    
    }
