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

    private float movementStartTime;
    private bool isMoving;
    private Rigidbody rb; // Rigidbody ������Ʈ ����

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ�� ������
        movementStartTime = 0f;
        isMoving = false;
    }

    void Update()
    {
        if (rb != null) // Rigidbody�� �����ϴ��� Ȯ��
        {
            Vector3 velocity = rb.velocity; // �ӵ� ���� ��������
            double speed = velocity.magnitude*3.6*3; // �ӵ��� ũ��(���밪) ���
            // �ӵ��� �ؽ�Ʈ�� ��ȯ�Ͽ� UI�� ǥ��
            textspeed.text = "Speed : " + speed.ToString("F2") + " km/h";
        }

        float currentAltitude = transform.position.y-20;// ������Ʈ�� y��ǥ - 20�ؼ� ��

        Altitude.text = "Altitude : "+currentAltitude.ToString("F2") + "meters";

        if (rb.velocity.magnitude > 0.1f)
        {
            if (!isMoving)
            {
                movementStartTime += Time.deltaTime;
                isMoving = true;
            }
            float elapsedTime = Time.time - movementStartTime;

            // ��� �ð��� �� ������ ��ȯ
            int minutes = Mathf.FloorToInt(elapsedTime / 60f); // ������ ��ȯ�ϰ� �Ҽ��� ���� ������


            textTime.text = "Time : " + minutes.ToString() + " minutes";
        }
        else if (isMoving)
        {
            isMoving = false;
        }
    }
}