using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    public const float PATH_TIME = 2f;
	public Player player;

    private readonly Queue<Waypoint> Waypoints = new Queue<Waypoint>();
    private readonly Queue<Path> Paths = new Queue<Path>();

    private Waypoint NextWaypoint;
	
	void Start ()
    {
        Waypoints.Enqueue(new Waypoint(new Vector3(0, 0, 13), Quaternion.identity));
        Waypoints.Enqueue(new Waypoint(new Vector3(-7, 0, 27), Quaternion.Euler(new Vector3(0f, -45f, 0f))));
        Waypoints.Enqueue(new Waypoint(new Vector3(-25, 0, 25), Quaternion.Euler(new Vector3(0f, -135f, 0f))));
    }

    public void BuildNextPath()
    {
        NextWaypoint = Waypoints.Dequeue();
        Path p = PathManager.Instance.GeneratePath(transform.position, transform.rotation, NextWaypoint.Position, NextWaypoint.Rotation);
        PathManager.Instance.VisualizePath(p);
        Paths.Enqueue(p);
        Go();
    }

    public void Go()
    {
        Path path = Paths.Dequeue();
        StartCoroutine(FollowPath(path, PATH_TIME / Path.NumPoints));
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
