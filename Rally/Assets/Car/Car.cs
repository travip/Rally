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
    public RoadGenerator roadGenerator;
    public TreeGenerator treeGenerator;

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
    public int RoadsAdded = 0;

    public List<RoadSegment> DEBUG_RoadList = new List<RoadSegment>();
    public bool DEBUG_GetRoads;
    public Transform RoadsContainer;

    private Waypoint LastWaypoint;

	private RoadSegment prevRoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Straight"))
        {
            Debug.Log("Trigger enter STRAIGHT");
            other.GetComponent<BoxCollider>().enabled = false;
            other.GetComponent<RoadSegment>().nextSegment.GetComponent<BoxCollider>().enabled = true;
		}
        else if (other.CompareTag("Turn"))
        {
            Debug.Log("Trigger enter TURN");
            RoadSegment rSeg = other.GetComponent<RoadSegment>();
            other.GetComponent<BoxCollider>().enabled = false;
            rSeg.nextSegment.GetComponent<BoxCollider>().enabled = true;
			Player.Instance.GetNextAction();
            carMsg.DisplayActionFromRoad(rSeg);
        }
    }

    void Start ()
    {
        LastWaypoint = new Waypoint(transform.position, transform.rotation, false);
        roadGenerator.AddNewRoadSection();
        InitialPopulateRoads();
        ProcessNextRoadAsStraight();
        BuildAllPaths();
        carMsg.DisplayStartMessage();
    }

	public void StartGame() {
		transform.Find("ExhaustParticles").gameObject.SetActive(true);
	}

    private void InitialPopulateRoads()
    {
        for(RoadsAdded = 0; RoadsAdded < 20; RoadsAdded++)
        {
            UnprossedRoads.Enqueue(roadGenerator.currentRoads[RoadsAdded].GetComponent<RoadSegment>());
        }
    }

    public void EnteredNewRoad()
    {
        // Add a new road to the processing pipeline
        roadGenerator.GenerateNextRoad();
        RoadSegment newRoad = roadGenerator.currentRoads[RoadsAdded++].GetComponent<RoadSegment>();
        UnprossedRoads.Enqueue(newRoad);
    }

    public void RestartGame()
    {
        PlayerUI.Instance.RestartGame();
    }

    public void PathMissed()
    {
        if(++Misses == 3)
        {
			transform.Find("ExhaustParticles").gameObject.SetActive(false);
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
        EnteredNewRoad();
        while (road.RoadType == Player.ActionType.Straight)
        {
            Waypoints.Enqueue(road.GetRandomMidpoint());
            Waypoints.Enqueue(road.GetRandomEndpoint());
            road = UnprossedRoads.Dequeue();
            EnteredNewRoad();
        }

		// Process the actual turn
		road.enteredType = action;
		if (action == road.RoadType)
        {
            Waypoints.Enqueue(road.GetRandomMidpoint());
            Waypoints.Enqueue(road.GetRandomEndpoint());
            road.wasCorrect = true;
		}
        // Incorrect input, get degree of failure
        else
        {
            road.wasCorrect = false;
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
        EnteredNewRoad();

		HighlightNextRoad();
	}

    public void ProcessNextRoadAsStraight()
    {
        RoadSegment road = UnprossedRoads.Dequeue();
        EnteredNewRoad();
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

		HighlightNextRoad();
	}

	private void HighlightNextRoad() {
		// Find the next turn and highlight it (but don't process it)
		if (prevRoad != null) {
			prevRoad.UnHighLight();
		}
		RoadSegment road = UnprossedRoads.Peek();
		while (road.RoadType == Player.ActionType.Straight) {
			Waypoints.Enqueue(road.GetRandomMidpoint());
			Waypoints.Enqueue(road.GetRandomEndpoint());
			UnprossedRoads.Dequeue();
			EnteredNewRoad();
			road = UnprossedRoads.Peek();
		}
		road.HighLight();
		prevRoad = road;
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
        Paths.Enqueue(p);
        UnvisitedWaypoints.Enqueue(LastWaypoint);
    }

    public void BeginFollowPath()
    {
        if (Paths.Count > 0)
        {
            Path path = Paths.Dequeue();
			if (path.Miss)
				PathMissed();
			else
				PlayerUI.Instance.AddScore();

			if (!Crashed)
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
