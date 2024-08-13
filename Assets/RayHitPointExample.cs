//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;

//public class RayHitPoint : MonoBehaviour
//{
//    private XRRayInteractor rayInteractor;

//    void Start()
//    {
//        // 이 스크립트가 붙어있는 오브젝트에서 XRRayInteractor 컴포넌트를 가져옴
//        rayInteractor = GetComponent<XRRayInteractor>();
//    }

//    void Update()
//    {
//        // Ray가 3D 오브젝트와 충돌했는지 확인
//        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
//        {
//            // 충돌 지점 정보 출력
//            Debug.Log("Ray Hit Point: " + hit.point);

//            // 원하는 작업 수행 (예: 지점에 오브젝트 생성)
//            // Instantiate(objectPrefab, hit.point, Quaternion.identity);
//        }
//        else
//        {
//            Debug.Log("Ray did not hit anything.");
//        }
//    }
//}
