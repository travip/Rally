using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public const float PATH_TIME = 1f;
	public Player player;

    private bool isMoving = false;

    // Unprocessed Waypoints
    private readonly Queue<Waypoint> Waypoints = new Queue<Waypoint>();

    // Processed paths (waiting to be executed)
    private readonly Queue<Path> Paths = new Queue<Path>();

    private readonly Queue<Waypoint> UnvisitedWaypoints = new Queue<Waypoint>();

    // Road Segments WITHOUT an associated Player Input
    // Once a input is received, pop the next road segment and generate waypoints
    public Queue<RoadSegment> UnprossedRoads = new Queue<RoadSegment>();

    public List<RoadSegment> DEBUG_RoadList = new List<RoadSegment>();
    public bool DEBUG_GetRoads;
    public Transform RoadsContainer;

    private Waypoint LastWaypoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Straight"))
        {
            Debug.Log("Trigger enter STRAIGHT");
            other.GetComponent<BoxCollider>().enabled = false;
        }
        else if (other.CompareTag("Turn"))
        {
            Debug.Log("Trigger enter TURN");
            Player.Instance.GetNextAction();
            other.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void Start ()
    {
        if(DEBUG_GetRoads)
        {
            foreach(Transform t in RoadsContainer)
            {
                UnprossedRoads.Enqueue(t.GetComponent<RoadSegment>());
            }
        }
    }

    // First road should always be straight
    public void Begin()
    {
        if(UnprossedRoads.Count == 0)
        {
            Debug.Log("BEGIN FAILED, no starting road?");
        }
        else
        {
            RoadSegment road = UnprossedRoads.Dequeue();
            Waypoints.Enqueue(road.CentreMidpoint);
            Waypoints.Enqueue(road.CentreEndpoint);
            BuildAllPaths();
        }
    }

    public void OnReachNewTurn()
    {
        Player.Instance.GetNextAction();
    }

    private void NoInputNextSection()
    {

    }

    private void ProcessNextTurn(Player.ActionType action)
    {
        RoadSegment road = UnprossedRoads.Dequeue();
        while(road.RoadType == Player.ActionType.Straight)
        {
            Waypoints.Enqueue(road.GetRandomMidpoint());
            Waypoints.Enqueue(road.GetRandomEndpoint());
            road = UnprossedRoads.Dequeue();
        }

        // Process the actual turn
        if (action == road.RoadType)
        {
            Waypoints.Enqueue(road.GetRandomMidpoint());
            Waypoints.Enqueue(road.GetRandomEndpoint());
            Debug.Log("You did gud");
        }
        // Incorrect input, get degree of failure
        else
        {
            int diff = action - road.RoadType;
            // MISS RIGHT
            if (diff > 0)
            {
                Debug.Log("MISS RIGHT");
                Waypoints.Enqueue(road.MidMissRight);
                Waypoints.Enqueue(road.MissRight);
            }
            // MISS LEFT
            else if (diff < -0)
            {
                Debug.Log("MISS LEFT");
                Waypoints.Enqueue(road.MidMissLeft);
                Waypoints.Enqueue(road.MissLeft);
            }
        }
    }

    public void OnPlayerInput(Player.ActionType action)
    {
        ProcessNextTurn(action);
        BuildAllPaths();
        if (!isMoving)
            BeginFollowPath();
    }

    public void BuildAllPaths()
    {
        while (Waypoints.Count > 0)
            BuildNextPath();
    }

    public void BuildNextPath()
    {
        Waypoint NextWaypoint = Waypoints.Dequeue();
        Path p = PathManager.Instance.GeneratePath(LastWaypoint, NextWaypoint);
        LastWaypoint = NextWaypoint;
        PathManager.Instance.VisualizePath(p);
        Paths.Enqueue(p);
        UnvisitedWaypoints.Enqueue(LastWaypoint);
    }

    public void BeginFollowPath()
    {
        if (Paths.Count > 0)
        {
            Debug.Log("BeginFollowPath");
            Path path = Paths.Dequeue();
            StartCoroutine(FollowPath(path, PATH_TIME / Path.NumPoints));
        }
        else
        {
            Debug.Log("Begin follow path with no paths available");
        }
    }

    private IEnumerator FollowPath(Path path, float pathTime)
    {
        isMoving = true;
        for (int i = 0; i < Path.NumPoints - 1; i++)
        {
            float elapsedTime = 0f;
            while (elapsedTime < pathTime)
            {
                transform.position = Vector3.Lerp(Path.PathToWorld(path.Points[i]), Path.PathToWorld(path.Points[i+1]), (elapsedTime / pathTime));
                transform.rotation = Quaternion.Lerp(path.Angles[i], path.Angles[i + 1], (elapsedTime / pathTime));
                elapsedTime += Time.deltaTime;
                yield return null; 
            }
        }
        isMoving = false;
        BeginFollowPath();
    }
}

public struct Waypoint
{
    public Vector3 Position;
    public Quaternion Rotation;

    public float Angle
    {
        get
        {
            return Rotation.eulerAngles.y;
        }
        private set { }
    }

    public Waypoint(Vector3 pos, Quaternion rot)
    {
        Position = pos;
        Rotation = rot;
    }
}
