//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;

//public class RayHitPoint : MonoBehaviour
//{
//    private XRRayInteractor rayInteractor;

//    void Start()
//    {
//        // �� ��ũ��Ʈ�� �پ��ִ� ������Ʈ���� XRRayInteractor ������Ʈ�� ������
//        rayInteractor = GetComponent<XRRayInteractor>();
//    }

//    void Update()
//    {
//        // Ray�� 3D ������Ʈ�� �浹�ߴ��� Ȯ��
//        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
//        {
//            // �浹 ���� ���� ���
//            Debug.Log("Ray Hit Point: " + hit.point);

//            // ���ϴ� �۾� ���� (��: ������ ������Ʈ ����)
//            // Instantiate(objectPrefab, hit.point, Quaternion.identity);
//        }
//        else
//        {
//            Debug.Log("Ray did not hit anything.");
//        }
//    }
//}
