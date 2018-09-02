using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public const float PATH_TIME = 1f;
    public const float CAR_Y_OFFSET = 0.66f;

	public Player player;
    public Rigidbody rb;
    public BoxCollider col;
    public CarTextBox carMsg;

    private bool isMoving = false;
    public int Misses = 0;
    private bool Crashed = false;

    public Transform TextBoxAnchor;

    // Unprocessed Waypoints
    private Queue<Waypoint> Waypoints = new Queue<Waypoint>();

    // Processed paths (waiting to be executed)
    private Queue<Path> Paths = new Queue<Path>();

    private Queue<Waypoint> UnvisitedWaypoints = new Queue<Waypoint>();

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
            other.GetComponent<BoxCollider>().enabled = false;
            Player.Instance.GetNextAction();
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
        LastWaypoint = new Waypoint(transform.position, transform.rotation, false);
        ProcessNextRoadAsStraight();
        BuildAllPaths();
    }

    public void RestartGame()
    {
        Waypoints = new Queue<Waypoint>();
        Paths = new Queue<Path>();
        UnvisitedWaypoints = new Queue<Waypoint>();
        UnprossedRoads = new Queue<RoadSegment>();

        rb.isKinematic = true;
        transform.SetPositionAndRotation(new Vector3(0f, CAR_Y_OFFSET, 0f), Quaternion.identity);
        LastWaypoint = new Waypoint(transform.position, transform.rotation, false);
        isMoving = false;
        Crashed = false;
        Misses = 0;
        PlayerUI.Instance.time = 0f;

        if (DEBUG_GetRoads)
        {
            foreach (Transform t in RoadsContainer)
            {
                UnprossedRoads.Enqueue(t.GetComponent<RoadSegment>());
            }
        }
        ProcessNextRoadAsStraight();
        BuildAllPaths();
        Player.Instance.BeginCountdown();
    }

    public void PathMissed()
    {
        if(++Misses == 3)
        {
            Crashed = true;
            carMsg.DisplayCrashMessage();
            StopAllCoroutines();
            PlayerUI.Instance.DisplayGameOver();
            rb.isKinematic = false;
            rb.AddExplosionForce(1000f, -transform.forward * 0.5f, 2f);
            Player.Instance.GamePlaying = false;
        }
        else
            carMsg.DisplayMissMessage();
        PlayerUI.Instance.SetMisses(Misses);
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
            else if (diff < 0)
            {
                Debug.Log("MISS LEFT");
                Waypoints.Enqueue(road.MidMissLeft);
                Waypoints.Enqueue(road.MissLeft);
            }
        }
    }

    public void ProcessNextRoadAsStraight()
    {
        Debug.Log("Process");
        RoadSegment road = UnprossedRoads.Dequeue();
        if (road.RoadType == Player.ActionType.Straight)
        {
            Waypoints.Enqueue(road.GetRandomMidpoint());
            Waypoints.Enqueue(road.GetRandomEndpoint());
        }
        else
        {
            int diff = Player.ActionType.Straight - road.RoadType;
            // MISS RIGHT
            if (diff > 0)
            {
                Debug.Log("MISS RIGHT");
                Waypoints.Enqueue(road.MidMissRight);
                Waypoints.Enqueue(road.MissRight);
            }
            // MISS LEFT
            else if (diff < 0)
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
            if(path.Miss)
                PathMissed();
            if(!Crashed)
                StartCoroutine(FollowPath(path, PATH_TIME / Path.NumPoints));
        }
        else
        {
            ProcessNextRoadAsStraight();
            BuildAllPaths();
            BeginFollowPath();
            Debug.Log("Automatically generate straight path");
        }
    }

    private IEnumerator FollowPath(Path path, float pathTime)
    {
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
        Debug.Log("done following path");
        BeginFollowPath();
    }
}

public struct Waypoint
{
    public Vector3 Position;
    public Quaternion Rotation;

    public bool Miss;

    public float Angle
    {
        get
        {
            return Rotation.eulerAngles.y;
        }
        private set { }
    }

    public Waypoint(Vector3 pos, Quaternion rot, bool m)
    {
        Position = pos;
        Rotation = rot;
        Miss = m;
    }
}
