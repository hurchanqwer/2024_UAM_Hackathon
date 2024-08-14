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
    public TextMeshProUGUI textDistance; // 거리 정보를 표시할 TextMeshProUGUI 추가

    private float movementStartTime;
    private bool isMoving;
    private Rigidbody rb; // Rigidbody 컴포넌트 참조

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트를 가져옴
        movementStartTime = 0f;
        isMoving = false;
    }

    void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) // Rigidbody가 존재하는지 확인
        {
            Vector3 velocity = rb.velocity; // 속도 벡터 가져오기
            double speed = velocity.magnitude*3.6*3; // 속도의 크기(절대값) 계산
           // Debug.Log("Speed: " + speed); // 속도를 로그에 출력

            // 속도를 텍스트로 변환하여 UI에 표시
            textspeed.text = "Speed : " + speed.ToString("F2") + " km/h";
        }
        else
        {
            Debug.LogWarning("Rigidbody가 이 게임 오브젝트에 없습니다.");
        }
        float currentAltitude = transform.position.y-20;// 오브젝트의 y좌표 - 20해서 땅
        //Debug.Log("Altitude:" + currentAltitude);//
        Altitude.text = "Altitude : "+currentAltitude.ToString("F2") + "meters";

        if (rb.velocity.magnitude > 0.1f)
        {
            if (!isMoving)
            {
                movementStartTime += Time.deltaTime;
                isMoving = true;
            }
            float elapsedTime = Time.time - movementStartTime;

            // 경과 시간을 분 단위로 변환
            int minutes = Mathf.FloorToInt(elapsedTime / 60f); // 분으로 변환하고 소수점 이하 버리기

            //Debug.Log("Time: " + minutes + " minutes");

            textTime.text = "Time : " + minutes.ToString() + " minutes";
        }
        else if (isMoving)
        {
            isMoving = false;
        }
    }
    // RobotMovementController에서 호출하여 distance 값을 업데이트하는 메서드 추가
    public void UpdateDistance(float distance)
    {
        distance = distance / 4; 
        textDistance.text = "Distance : " + distance.ToString("F2") + " meters";
    }

}