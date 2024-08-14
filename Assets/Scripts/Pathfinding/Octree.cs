using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Priority_Queue;
using UnityEngine;

public class Octree : MonoBehaviour
{
    [SerializeField] private float minCellSize = 2;
    [SerializeField] private LayerMask mask = -1;
    [SerializeField] private float maxMilisecondsPerFrame = 10;
    [SerializeField] private int cellCount;
    [SerializeField] private Transform player;
    [SerializeField] private Transform destination;
    //[SerializeField] private PathfindingAlgorith algorithm = PathfindingAlgorith.AStar;
    [SerializeField] private float maxActivePathfinds = 6;
    private BoxCollider boxCollider;
    private OctreeElement root;
    private Queue<OctreeElement> toBeSplit;
    private Queue<PathRequest> requests = new Queue<PathRequest>();
    private List<PathRequest> running = new List<PathRequest>();

    // Use this for initialization
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        root = new OctreeElement(null, boxCollider.bounds, 0);
        toBeSplit = new Queue<OctreeElement>();
        toBeSplit.Enqueue(root);
        cellCount++;
        float doubleMinCellSize = minCellSize * 2f;

        while (toBeSplit.Count > 0)
        {
            var elem = toBeSplit.Dequeue();

            elem.Empty = !Physics.CheckBox(elem.Bounds.center, elem.Bounds.extents, Quaternion.identity, mask, QueryTriggerInteraction.Ignore);

            if (elem.Bounds.size.magnitude > doubleMinCellSize && !elem.Empty)
            {
                elem.Split();

                foreach (var child in elem.Children)
                {
                    toBeSplit.Enqueue(child);
                    cellCount++;
                }
            }
        }

        CalculateNeighborsRecursive(root);
    }

    // Update is called once per frame
    void Update()
    {
        if (requests.Count > 0 && running.Count < maxActivePathfinds)
        {
            var newRequest = requests.Dequeue();
            ThreadPool.QueueUserWorkItem(GetPathAstar, newRequest);
            running.Add(newRequest);
        }

        if (running.Count > 0)
        {
            for (int i = running.Count - 1; i >= 0; i--)
            {
                if (!running[i].isCalculating)
                {
                    running.RemoveAt(i);
                }
            }
        }
    }

    private void CalculateNeighborsRecursive(OctreeElement element)
    {
        if (element.Children == null)
        {
            element.Neigbors = new OctreeElement[6][];
            for (int i = 0; i < 6; i++)
            {
                List<OctreeElement> neighbors = new List<OctreeElement>();
                GetNeighbors(element, (OctreeElement.Dir)i, neighbors);
                element.Neigbors[i] = neighbors.ToArray();
            }
        }
        else
        {
            for (int i = 0; i < element.Children.Length; i++)
            {
                CalculateNeighborsRecursive(element.Children[i]);
            }
        }
    }

    public PathRequest GetPath(Vector3 from, Vector3 to)
    {
        PathRequest request = new PathRequest() { from = from, to = to, isCalculating = true };
        requests.Enqueue(request);
        return request;
    }

    public void GetPathAstar(object context)
    {
        PathRequest request = (PathRequest)context;

        try
        {
            FastPriorityQueue<OctreeElementQueueElemenet> fronteer = new FastPriorityQueue<OctreeElementQueueElemenet>(16);
            Dictionary<OctreeElement, OctreeElement> cameFrom = new Dictionary<OctreeElement, OctreeElement>();
            Dictionary<OctreeElement, float> weights = new Dictionary<OctreeElement, float>();
            OctreeElement startNode = GetNode(request.from);
            OctreeElement endNode = GetNode(request.to);
            if (startNode == null || endNode == null) return;
            weights.Add(startNode, startNode.cost);
            fronteer.Enqueue(new OctreeElementQueueElemenet(startNode), startNode.cost);
            OctreeElement current;
            OctreeElement closest = startNode;
            float closestDistance = Vector3.SqrMagnitude(startNode.Bounds.center - request.to);
            long lastMiliseconds = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (fronteer.Count > 0)
            {
                current = fronteer.Dequeue().Element;
                if (current == endNode) break;
                //still building path
                if (current.Neigbors != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        for (int n = 0; n < current.Neigbors[i].Length; n++)
                        {
                            var next = current.Neigbors[i][n];
                            if (!next.Empty && next != endNode) continue;
                            float sqrDistance = Vector3.SqrMagnitude(next.Bounds.center - request.to);
                            if (sqrDistance < closestDistance)
                            {
                                closestDistance = sqrDistance;
                                closest = next;
                            }
                            float distance = (next.Bounds.center - request.to).sqrMagnitude;
                            float newWeight = weights[current] + next.cost;
                            if (!weights.ContainsKey(next) || newWeight < weights[next])
                            {
                                weights[next] = newWeight;
                                cameFrom[next] = current;
                                if (fronteer.MaxSize <= fronteer.Count)
                                {
                                    fronteer.Resize(fronteer.MaxSize * 2);
                                }
                                fronteer.Enqueue(new OctreeElementQueueElemenet(next), newWeight + distance);
                            }
                        }
                    }
                }

                if (maxMilisecondsPerFrame > 0 && stopwatch.ElapsedMilliseconds - lastMiliseconds > maxMilisecondsPerFrame)
                {
                    lastMiliseconds = stopwatch.ElapsedMilliseconds;
                    Thread.Sleep(1);
                }
            }

            current = endNode;
            bool particalPath = false;
            while (current != startNode)
            {
                if (!cameFrom.TryGetValue(current, out current))
                {
                    particalPath = true;
                    current = closest;
                }
                request.path.Insert(0, current.Bounds.center);
            }

            if (!particalPath)
            {
                request.path.Add(request.to);
            }

            // 경로 2 생성
            request.path2.AddRange(request.path); //나다

            request.isCalulated = true;
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            request.isCalculating = false;
        }
        float tolerance = 0.5f; // 간소화 정도를 결정하는 허용 오차 (값을 조정해 필요에 맞게 설정)
        request.path = PathSimplification.SimplifyPath(request.path, tolerance);
        request.path2 = PathSimplification.SimplifyPath(request.path2, tolerance);//나다
        SmoothPath(request.path, 10);
        SmoothPath(request.path2, 10);//나다
    }

    public class PathSimplification
    {
        public static List<Vector3> SimplifyPath(List<Vector3> path, float tolerance)
        {
            if (path == null || path.Count < 3)
            {
                return path;
            }

            bool[] keep = new bool[path.Count];
            keep[0] = true;
            keep[path.Count - 1] = true;

            DouglasPeucker(path, keep, tolerance, 0, path.Count - 1);

            List<Vector3> result = new List<Vector3>();
            for (int i = 0; i < keep.Length; i++)
            {
                if (keep[i])
                {
                    result.Add(path[i]);
                }
            }

            return result;
        }

        private static void DouglasPeucker(List<Vector3> path, bool[] keep, float tolerance, int startIndex, int endIndex)
        {
            if (endIndex <= startIndex + 1)
            {
                return;
            }

            float maxDistance = 0f;
            int maxIndex = startIndex;

            Vector3 startPoint = path[startIndex];
            Vector3 endPoint = path[endIndex];

            for (int i = startIndex + 1; i < endIndex; i++)
            {
                float distance = PerpendicularDistance(path[i], startPoint, endPoint);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }

            if (maxDistance > tolerance)
            {
                keep[maxIndex] = true;

                DouglasPeucker(path, keep, tolerance, startIndex, maxIndex);
                DouglasPeucker(path, keep, tolerance, maxIndex, endIndex);
            }
        }

        private static float PerpendicularDistance(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 line = lineEnd - lineStart;
            Vector3 pointLine = point - lineStart;

            float area = Vector3.Cross(line, pointLine).magnitude;
            float lineLength = line.magnitude;

            return area / lineLength;
        }
    }

    public void SmoothPath(List<Vector3> path, int subdivisions)
    {
        if (path == null || path.Count < 2) return;

        List<Vector3> smoothedPath = new List<Vector3>();

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 p0 = i == 0 ? path[i] : path[i - 1];
            Vector3 p1 = path[i];
            Vector3 p2 = path[i + 1];
            Vector3 p3 = i == path.Count - 2 ? path[i + 1] : path[i + 2];

            for (int j = 0; j < subdivisions; j++)
            {
                float t = j / (float)subdivisions;
                smoothedPath.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        smoothedPath.Add(path[path.Count - 1]);

        path.Clear();
        path.AddRange(smoothedPath);
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

    private static int getNeighborStartingDepth;
    //records neighbor search traversal
    private static int[] neighborPathPositions = new int[32];

    private void GetNeighbors(OctreeElement startNode, OctreeElement.Dir dir, ICollection<OctreeElement> neighbors)
    {
        getNeighborStartingDepth = startNode.Depth;
        var topmostNeighbor = GetNeighborRec(startNode, dir);
        if (topmostNeighbor != null) GetAllChildrentInDir(topmostNeighbor, dir, neighbors);
    }

    private OctreeElement GetNeighborRec(OctreeElement startNode, OctreeElement.Dir dir)
    {
        OctreeElement parent = startNode.Parent;
        if (parent == null) return null;

        //find local neighbor
        int localIndex = Array.IndexOf(parent.Children, startNode);
        StorePositionAtDepth(parent.Depth, localIndex);
        int localNeighborIndex = OctreeElement.localNeighborIndex[localIndex][(int)dir];
        if (localNeighborIndex >= 0)
        {
            return parent.Children[localNeighborIndex];
        }

        OctreeElement topmostNeighbor = GetNeighborRec(parent, dir);
        //this means the edge of the octree volume so we return null
        if (topmostNeighbor == null) return null;

        //find the lowest mirrored child of the parent neighbor
        OctreeElement lowerMostReflectedChild = GetLowestChild(topmostNeighbor, dir, getNeighborStartingDepth);
        return lowerMostReflectedChild;
    }

    private OctreeElement GetLowestChild(OctreeElement start, OctreeElement.Dir dir, int maxDepth)
    {
        if (start.Children != null && start.Depth < maxDepth)
        {
            OctreeElement.Pos reflectedPos = OctreeElement.ReflectedPos[(int)dir][neighborPathPositions[start.Depth]];
            return GetLowestChild(start.Children[(int)reflectedPos], dir, maxDepth);
        }
        return start;
    }

    private void GetAllChildrentInDir(OctreeElement start, OctreeElement.Dir dir, ICollection<OctreeElement> elements)
    {
        if (start.Children != null)
        {
            var oppositeDir = (int)OctreeElement.OppositeDirs[(int)dir];
            for (int i = 0; i < OctreeElement.PosInDir[oppositeDir].Length; i++)
            {
                GetAllChildrentInDir(start.Children[(int)OctreeElement.PosInDir[oppositeDir][i]], dir, elements);
            }
        }
        else
        {
            elements.Add(start);
        }
    }

    private void StorePositionAtDepth(int depth, int pos)
    {
        if (depth >= neighborPathPositions.Length)
        {
            Array.Resize(ref neighborPathPositions, depth + 1);
        }

        neighborPathPositions[depth] = pos;
    }

    private static Vector3 tmpGetNodePos;

    private OctreeElement GetNode(Vector3 position)
    {
        tmpGetNodePos = position;
        return GetNode(root);
    }

    private OctreeElement GetNode(OctreeElement parent)
    {
        if (parent.Bounds.Contains(tmpGetNodePos))
        {
            if (parent.Children != null)
            {
                for (int i = 0; i < parent.Children.Length; i++)
                {
                    OctreeElement child = GetNode(parent.Children[i]);
                    if (child != null) return child;
                }
            }
            else
            {
                return parent;
            }
        }
        return null;
    }

    public bool IsBuilding { get { return toBeSplit.Count > 0; } }

    public class PathRequest
    {
        internal Vector3 from;
        internal Vector3 to;
        internal List<Vector3> path;
        internal List<Vector3> path2;//나다
        internal bool isCalulated;
        internal bool isCalculating;

        public PathRequest()
        {
            path = new List<Vector3>();
            path2 = new List<Vector3>();//나다
        }

        public List<Vector3> Path
        {
            get { return path; }
        }
        public List<Vector3> Path2
        {
            get { return path2; }//나다
        }
        public void Reset()
        {
            isCalulated = false;
            isCalculating = false;
            path.Clear();
            path2.Clear();//나다
        }
    }

    public enum PathfindingAlgorith
    {
        AStar,
        Greedy
    }

    public class OctreeElementQueueElemenet : FastPriorityQueueNode
    {
        public OctreeElement Element { get; set; }

        public OctreeElementQueueElemenet(OctreeElement element)
        {
            Element = element;
        }
    }

    public class OctreeElement
    {
        public static readonly Vector3[] splitDirs = { new Vector3(1, -1, -1), new Vector3(1, -1, 1), new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1) };
        public static readonly int[][] localNeighborIndex =
        {
            new []{-1,4,2,-1,1,-1},new []{-1,5,3,-1,-1,0},new []{-1,6,-1,0,3,-1},new []{-1,7,-1,1,-1,2},new []{0,-1,6,-1,5,-1},new []{1,-1,7,-1,-1,4},
            new []{2,-1,-1,4,7,-1},new []{3,-1,-1,5,-1,6}

        };

        public static readonly Dir[] OppositeDirs = { Dir.R, Dir.L, Dir.D, Dir.U, Dir.B, Dir.F };
        //def new[]{ Pos.LBD, Pos.LFD, Pos.LBU, Pos.LFU, Pos.RBD, Pos.RFD, Pos.RBU, Pos.RFU }
        public static readonly Pos[][] ReflectedPos =
        {
            new[]{ Pos.RBD, Pos.RFD, Pos.RBU, Pos.RFU, Pos.LBD, Pos.LFD, Pos.LBU, Pos.LFU },
            new[]{ Pos.RBD, Pos.RFD, Pos.RBU, Pos.RFU, Pos.LBD, Pos.LFD, Pos.LBU, Pos.LFU },
            new[]{ Pos.LBU, Pos.LFU, Pos.LBD, Pos.LFD, Pos.RBU, Pos.RFU, Pos.RBD, Pos.RFD },
            new[]{ Pos.LBU, Pos.LFU, Pos.LBD, Pos.LFD, Pos.RBU, Pos.RFU, Pos.RBD, Pos.RFD },
            new[]{ Pos.LFD, Pos.LBD, Pos.LFU, Pos.LBU, Pos.RFD, Pos.RBD, Pos.RFU, Pos.RBU },
            new[]{ Pos.LFD, Pos.LBD, Pos.LFU, Pos.LBU, Pos.RFD, Pos.RBD, Pos.RFU, Pos.RBU }
        };

        public static readonly Pos[][] PosInDir =
        {
            new[] {Pos.LBD, Pos.LFD, Pos.LBU, Pos.LFU},
            new[] {Pos.RBD, Pos.RFD, Pos.RBU, Pos.RFU},
            new[]{ Pos.LBU, Pos.LFU, Pos.RBU, Pos.RFU },
            new[]{ Pos.LBD, Pos.LFD, Pos.RBD, Pos.RFD},
            new[]{ Pos.LFD, Pos.LFU, Pos.RFD, Pos.RFU },
            new[]{ Pos.LBD, Pos.LBU, Pos.RBD, Pos.RBU}
        };
        public Bounds Bounds;
        public OctreeElement[] Children;
        public OctreeElement Parent;
        public OctreeElement[][] Neigbors;
        public float cost = 1;
        public int Depth;
        public bool Empty;

        public OctreeElement(OctreeElement parent, Bounds bounds, int depth)
        {
            Parent = parent;
            Bounds = bounds;
            Depth = depth;
        }

        public void Split()
        {
            Children = new OctreeElement[splitDirs.Length];
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i] = new OctreeElement(this, new Bounds(Bounds.center + Vector3.Scale(splitDirs[i], Bounds.extents / 2f), Bounds.extents), Depth + 1);
            }
        }

        public enum Dir
        {
            L, R, U, D, F, B
        }

        public enum Pos
        {
            LBD, LFD, LBU, LFU, RBD, RFD, RBU, RFU
        }
    }
}
