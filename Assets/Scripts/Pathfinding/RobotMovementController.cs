using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
	private new Rigidbody rigidbody;
	private Vector3 currentDestination;
	private Vector3 lastDestination;
	private SphereCollider sphereCollider;
    private LineRenderer lineRenderer;
    // Use this for initialization
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rigidbody = GetComponent<Rigidbody>();

        // LineRenderer 컴포넌트 초기화
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.3f; // 시작 지점의 선 너비
        lineRenderer.endWidth = 0.3f;   // 끝 지점의 선 너비
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 기본 셰이더 사용
        lineRenderer.startColor = Color.green;  // 시작 색상
        lineRenderer.endColor = Color.green;    // 끝 색상
        
    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        if ((newPath == null || !newPath.isCalculating) && Vector3.SqrMagnitude(target.transform.position - lastDestination) > maxDistanceRebuildPath && (!CanSeePlayer() || Vector3.Distance(target.position, transform.position) > minFollowDistance) && !octree.IsBuilding)
        {
            lastDestination = target.transform.position;

            oldPath = newPath;
            newPath = octree.GetPath(transform.position, lastDestination);
        }

        var curPath = Path;

        if (!curPath.isCalculating && curPath != null && curPath.Path.Count > 0)
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
            lineRenderer.positionCount = curPath.Path.Count;          
            for (int i = 0; i < curPath.Path.Count; i++)
            {               
                lineRenderer.SetPosition(i, curPath.Path[i]);
            }      
        }      
    }
    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * ((2.0f * p1) +
                       (-p0 + p2) * t +
                       (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
                       (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);
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
		if (rigidbody != null)
		{
			Gizmos.color = Color.blue;
			Vector3 predictedPosition = rigidbody.position + rigidbody.velocity * Time.deltaTime;
			Gizmos.DrawWireSphere(predictedPosition, sphereCollider.radius);
		}

		if (Path != null)
		{
			var path = Path;
			for (int i = 0; i < path.Path.Count-1; i++)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(path.Path[i], minReachDistance);
				Gizmos.color = Color.red;
				Gizmos.DrawRay(path.Path[i], Vector3.ClampMagnitude(rigidbody.position - path.Path[i], pathPointRadius));
				Gizmos.DrawWireSphere(path.Path[i],pathPointRadius);
				Gizmos.DrawLine(path.path[i], path.Path[i+1]);
			}
		}
	}
}
