using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public const float PATH_TIME = 2f;
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

    private Waypoint LastWaypoint;
	
	void Start ()
    {
        //Waypoints.Enqueue(new Waypoint(new Vector3(0, 0, 13), Quaternion.identity));
        //Waypoints.Enqueue(new Waypoint(new Vector3(-7, 0, 27), Quaternion.Euler(new Vector3(0f, -45f, 0f))));
        //Waypoints.Enqueue(new Waypoint(new Vector3(-25, 0, 25), Quaternion.Euler(new Vector3(0f, -135f, 0f))));
        if(DEBUG_RoadList.Count >= 0)
        {
            foreach (RoadSegment r in DEBUG_RoadList)
                UnprossedRoads.Enqueue(r);
        }
    }

    public void OnPlayerInput(Player.ActionType action)
    {
        RoadSegment road = UnprossedRoads.Peek();
        // Correct input
        if (action == road.RoadType)
        {
            UnprossedRoads.Dequeue();
            Waypoints.Enqueue(road.GetRandomMidpoint());
            Waypoints.Enqueue(road.GetRandomEndpoint());
            Debug.Log("You did gud");
            BuildAllPaths();
            if (!isMoving)
                BeginFollowPath();
        }
        else
        {
            Debug.Log("u fucked up");
        }
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
        if (Paths.Count >= 0)
        {
            Debug.Log("BeginFollowPath");
            Path path = Paths.Dequeue();
            if(path.isNewSegment)
            {
                Debug.Log("NEW SEGMENT!");
                Player.Instance.GetNextAction();
            }
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

    public bool isNewSegment;

    public float Angle
    {
        get
        {
            return Rotation.eulerAngles.y;
        }
        private set { }
    }

    public Waypoint(Vector3 pos, Quaternion rot, bool seg)
    {
        Position = pos;
        Rotation = rot;
        isNewSegment = seg;
    }
}
