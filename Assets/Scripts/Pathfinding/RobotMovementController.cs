using System.Collections.Generic;
using UnityEngine;

public class RobotMovementController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float maxDistanceRebuildPath = 1;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float minReachDistance = 2f;
    [SerializeField] private float minFollowDistance = 4f;
    [SerializeField] private float pathPointRadius = 0.2f;
    [SerializeField] private Octree octree;
    [SerializeField] private LayerMask playerSeeLayerMask = -1;
    [SerializeField] private GameObject playerObject;

    private Octree.PathRequest oldPath;
    private Octree.PathRequest newPath;
    private Octree.PathRequest oldPath2;
    private Octree.PathRequest newPath2;
    private new Rigidbody rigidbody;
    private Vector3 currentDestination;
    private Vector3 lastDestination;
    private SphereCollider sphereCollider;
    private LineRenderer lineRenderer1;
    private LineRenderer lineRenderer2;
    private UiDatavisualization uiDataVisualization; // UiDatavisualization 스크립트 참조
    private Vector3 AirForcePosition;
    private float distancewindpush;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rigidbody = GetComponent<Rigidbody>();
        uiDataVisualization = FindObjectOfType<UiDatavisualization>(); // UiDatavisualization 스크립트 찾기

        if (sphereCollider == null || rigidbody == null)
        {
            Debug.LogError("Required component missing.");
            return;
        }

        // 첫 번째 LineRenderer 초기화
        lineRenderer1 = gameObject.AddComponent<LineRenderer>();
        lineRenderer1.startWidth = 0.3f;
        lineRenderer1.endWidth = 0.3f;
        lineRenderer1.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer1.startColor = Color.green;
        lineRenderer1.endColor = Color.green;

        // 두 번째 LineRenderer 초기화
        lineRenderer2 = gameObject.AddComponent<LineRenderer>();
        lineRenderer2.startWidth = 0.3f;
        lineRenderer2.endWidth = 0.3f;
        lineRenderer2.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer2.startColor = Color.blue;
        lineRenderer2.endColor = Color.blue;
    }

    void FixedUpdate()
    {
        if ((newPath == null || !newPath.isCalculating) && Vector3.SqrMagnitude(target.transform.position - lastDestination) > maxDistanceRebuildPath && (!CanSeePlayer() || Vector3.Distance(target.position, transform.position) > minFollowDistance) && !octree.IsBuilding)
        {
            lastDestination = target.transform.position;

            oldPath = newPath;
            newPath = octree.GetPath(transform.position, lastDestination);
        }

        var curPath = Path;

        if (curPath != null && !curPath.isCalculating && curPath.Path.Count > 0)
        {
            if (Vector3.Distance(transform.position, target.position) < minFollowDistance && CanSeePlayer())
            {
                curPath.Reset();
            }

            currentDestination = curPath.Path[0] + Vector3.ClampMagnitude(rigidbody.position - curPath.Path[0], pathPointRadius);
            rigidbody.velocity += Vector3.ClampMagnitude(currentDestination - transform.position, 1) * Time.deltaTime * acceleration;

            float sqrMinReachDistance = minReachDistance * minReachDistance;

            Vector3 predictedPosition = rigidbody.position + rigidbody.velocity * Time.deltaTime;
            float shortestPathDistance = Vector3.SqrMagnitude(predictedPosition - currentDestination);
            int shortestPathPoint = 0;

            // 현재 위치에서 가까운 경로 점들을 제거하여 경로를 줄입니다.
            for (int i = 0; i < curPath.Path.Count; i++)
            {
                float sqrDistance = Vector3.SqrMagnitude(rigidbody.position - curPath.Path[i]);
                if (sqrDistance <= sqrMinReachDistance)
                {
                    if (i < curPath.Path.Count)
                    {
                        curPath.Path.RemoveRange(0, i + 1);
                    }
                    shortestPathPoint = 0;
                    break;
                }

                float sqrPredictedDistance = Vector3.SqrMagnitude(predictedPosition - curPath.Path[i]);
                if (sqrPredictedDistance < shortestPathDistance)
                {
                    shortestPathDistance = sqrPredictedDistance;
                    shortestPathPoint = i;
                }
            }

            if (shortestPathPoint > 0)
            {
                curPath.Path.RemoveRange(0, shortestPathPoint);
            }

            // LineRenderer로 경로 그리기
            lineRenderer1.positionCount = curPath.Path.Count;
            for (int i = 0; i < curPath.Path.Count; i++)
            {
                lineRenderer1.SetPosition(i, curPath.Path[i]);
            }
        }

        // 두 번째 경로 처리
        if (newPath2 == null)
        {
            newPath2 = octree.GetPath(transform.position, lastDestination);
        }

        var curPath2 = Path2;

        if (curPath2 != null && curPath2.Path2 != null && curPath2.Path2.Count > 0 && !curPath2.isCalculating)
        {
            // 두 번째 경로는 생성 후 고정, 갱신하지 않음
            if (lineRenderer2.positionCount == 0)
            {
                lineRenderer2.positionCount = curPath2.Path2.Count;
                for (int i = 0; i < curPath2.Path2.Count; i++)
                {
                    lineRenderer2.SetPosition(i, curPath2.Path2[i]);
                }
            }
        }
        else
        {
            Debug.LogError("curPath2 or curPath2.Path2 is null");
        }
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, List<Vector3> path)
    {
        if (lineRenderer == null || path == null)
        {
            Debug.LogError("LineRenderer or path is null.");
            return;
        }

        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i]);
        }
    }

    private bool CanSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, transform.position - target.position), out hit, Vector3.Distance(transform.position, target.position) + 1, playerSeeLayerMask))
        {
            return hit.transform.gameObject == playerObject;
        }
        return false;
    }

    private Octree.PathRequest Path
    {
        get
        {
            if ((newPath == null || newPath.isCalculating) && oldPath != null)
            {
                return oldPath;
            }
            return newPath;
        }
    }

    private Octree.PathRequest Path2
    {
        get
        {
            if ((newPath2 == null || newPath2.isCalculating) && oldPath2 != null)
            {
                return oldPath2;
            }
            return newPath2;
        }
    }

    public bool HasTarget
    {
        get
        {
            return Path != null && Path.Path.Count > 0;
        }
    }

    public Vector3 CurrentTargetPosition
    {
        get
        {
            if (Path != null && Path.Path.Count > 0)
            {
                return currentDestination;
            }
            else
            {
                return target.position;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Path != null)
        {
            var path = Path;
            for (int i = 0; i < path.Path.Count - 1; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(path.Path[i], minReachDistance);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(path.Path[i], Vector3.ClampMagnitude(rigidbody.position - path.Path[i], pathPointRadius));
                Gizmos.DrawWireSphere(path.Path[i], pathPointRadius);
                Gizmos.DrawLine(path.Path[i], path.Path[i + 1]);
            }
        }
        if (Path2 != null)
        {
            var path2 = Path2;
            for (int i = 0; i < path2.Path2.Count - 1; i++)
            {
                Gizmos.DrawLine(path2.Path2[i], path2.Path2[i + 1]);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Vector3 currentPosition = transform.position;
        Vector3 contactPathPosition = other.transform.position;

        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        // Path 리스트를 순회하며 가장 가까운 점을 찾는다
        for (int i = 0; i < Path.Path.Count; i++)
        {
            float distance = Vector3.Distance(contactPathPosition, Path.Path[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        // 유효한 인덱스를 찾았는지 확인
        if (closestIndex != -1)
        {
            Vector3 previousPathPosition = closestIndex > 0 ? Path.Path[closestIndex - 1] : Path.Path[closestIndex];

            // 이전 path와 접촉한 path 사이의 벡터 (직선)
            Vector3 pathVector = contactPathPosition - previousPathPosition;

            // currentPosition에서 pathVector에 대한 수직 거리 계산
            float distance = Vector3.Cross(pathVector, currentPosition - previousPathPosition).magnitude / pathVector.magnitude;

            // 거리 출력 및 UI에 업데이트
            Debug.Log("Distance from path to current position: " + distance);

            // UiDatavisualization 스크립트에 distance 값을 전달
            uiDataVisualization.UpdateDistance(distance);
        }
        else
        {
            Debug.LogError("Closest path point not found.");
        }
    }
}