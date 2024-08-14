using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�
using UnityEngine.UI;

public class UiDatavisualization : MonoBehaviour
{
    public TextMeshProUGUI textspeed; // TextMeshPro - Text ������Ʈ�� �����ϱ� ���� ����
    public TextMeshProUGUI Altitude;
    public TextMeshProUGUI textTime;

    public float time;
    public float currentAltitude;

    private Rigidbody rb; // Rigidbody ������Ʈ ����

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ�� ������

    }

    void Update()
    {
        
            Vector3 velocity = rb.velocity; // �ӵ� ���� ��������
            double speed = velocity.magnitude*3.6*3; // �ӵ��� ũ��(���밪) ���
            // �ӵ��� �ؽ�Ʈ�� ��ȯ�Ͽ� UI�� ǥ��
            textspeed.text = "Wind : " + speed + " km/h";
    
        currentAltitude  = transform.position.y - 20;
        Altitude.text = "Altitude : " + currentAltitude+ "meters";

        time += Time.deltaTime;
 
          int minutes = Mathf.FloorToInt(time / 60f); // ������ ��ȯ�ϰ� �Ҽ��� ���� ������
        int sec = Mathf.FloorToInt(time % 60f);

          textTime.text = "Time : " + minutes  + "m " + sec+"s";
        }
    
    }
