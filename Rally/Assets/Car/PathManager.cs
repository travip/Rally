using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }
    private PathGenerator pathGenerator;

    public GameObject DEBUG_OBJECT;
    public GameObject Waypoint;

    public Vector3 START;
    public Vector3 START_TANGENT;
    public Vector3 END;
    public Vector3 END_TANGENT;

    // Target points
    public List<Vector3> DEBUG_POINTS;
    public List<Vector3> DEBUG_ANGLES;

    // Generated path
    private List<Vector2> PATH_POINTS = new List<Vector2>();
    private List<Vector2> ANGLE_POINTS = new List<Vector2>();

    public int DEBUG_DROP_OBJECT_ON_EVERY_X_POINTS = 0;

    Vector2[] path;
    Vector2[] angles;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        pathGenerator = new PathGenerator();
    }

    public Path GeneratePath(Vector3 start, Quaternion startAngle, Vector3 end, Quaternion endAngle)
    {
        Path path = new Path();
        pathGenerator.CubicHermiteSpline(Path.WorldToPath(start), Path.QuaternionToTangent(startAngle, 10), Path.WorldToPath(end), Path.QuaternionToTangent(endAngle, 10), 
            out path.Points, out path.Tangents);
        path.GenerateAnglesFromTangents();
        return path;
    }

    // Generated a path, also consideres segmentation from waypoints
    public Path GeneratePath(Waypoint start, Waypoint end)
    {
        Path path = new Path();
        pathGenerator.CubicHermiteSpline(Path.WorldToPath(start.Position), Path.QuaternionToTangent(end.Rotation, 10), Path.WorldToPath(end.Position), Path.QuaternionToTangent(end.Rotation, 10),
            out path.Points, out path.Tangents);
        path.isNewSegment = end.isNewSegment;
        path.GenerateAnglesFromTangents();
        return path;
    }

    public void VisualizePath(Path path)
    {
        for(int i = 0; i < Path.NumPoints; i++)
        {
            Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(Path.PathToWorld(path.Points[i]), path.Angles[i]);
        }
    }

    public void PLACE_DEBUG_ON_PATH()
    {
        Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(Path.PathToWorld(PATH_POINTS[PATH_POINTS.Count - 1]), Path.TangentToRotation(ANGLE_POINTS[PATH_POINTS.Count - 1]));
    }

    public void DEBUG_PlaceWaypoints()
    {
        foreach (Vector3 v in DEBUG_POINTS)
            Instantiate(Waypoint).transform.SetPositionAndRotation(v, Quaternion.identity);
    }

    public void TEST_PATH()
    {
        DEBUG_PlaceWaypoints();
        for (int i = 0; i < DEBUG_POINTS.Count - 1; i++)
        {
            pathGenerator.CubicHermiteSpline(Path.WorldToPath(DEBUG_POINTS[i]), Path.WorldToPath(DEBUG_ANGLES[i]), Path.WorldToPath(DEBUG_POINTS[i+1]), Path.WorldToPath(DEBUG_ANGLES[i+1]), out path, out angles);
            PATH_POINTS.AddRange(path);
            ANGLE_POINTS.AddRange(angles);
        }

        Debug.Log("Path count: " + PATH_POINTS.Count);

        for(int i = 0; i < PATH_POINTS.Count - 1; i++)
        {
            Debug.DrawLine(Path.PathToWorld(PATH_POINTS[i]), Path.PathToWorld(PATH_POINTS[i + 1]), Color.red, 100f);
            if (DEBUG_DROP_OBJECT_ON_EVERY_X_POINTS > 0)
            {
                if (i % DEBUG_DROP_OBJECT_ON_EVERY_X_POINTS == 0)
                    Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(Path.PathToWorld(PATH_POINTS[i]), Path.TangentToRotation(ANGLE_POINTS[i]));
            }
        }
        PLACE_DEBUG_ON_PATH();
    }
}
