using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using UnityEngine.UI;

public class UiDatavisualization : MonoBehaviour
{
    public TextMeshProUGUI textspeed; // TextMeshPro - Text 컴포넌트를 참조하기 위한 변수
    public TextMeshProUGUI Altitude;
    public TextMeshProUGUI textTime;

    public float time;
    public float currentAltitude;

    private Rigidbody rb; // Rigidbody 컴포넌트 참조

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트를 가져옴

    }

    void Update()
    {
        
            Vector3 velocity = rb.velocity; // 속도 벡터 가져오기
            double speed = velocity.magnitude*3.6*3; // 속도의 크기(절대값) 계산
            // 속도를 텍스트로 변환하여 UI에 표시
            textspeed.text = "Wind : " + speed + " km/h";
    
        currentAltitude  = transform.position.y - 20;
        Altitude.text = "Altitude : " + currentAltitude+ "meters";

        time += Time.deltaTime;
 
          int minutes = Mathf.FloorToInt(time / 60f); // 분으로 변환하고 소수점 이하 버리기
        int sec = Mathf.FloorToInt(time % 60f);

          textTime.text = "Time : " + minutes  + "m " + sec+"s";
        }
    
    }
