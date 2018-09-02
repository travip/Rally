using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    public const float TANGENT_MAGNITUDE = 5f;
    private PathGenerator pathGenerator;

    public GameObject DEBUG_OBJECT;
    public GameObject Waypoint;

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
        pathGenerator.CubicHermiteSpline(Path.WorldToPath(start), Path.QuaternionToTangent(startAngle, TANGENT_MAGNITUDE), Path.WorldToPath(end), Path.QuaternionToTangent(endAngle, TANGENT_MAGNITUDE), 
            out path.Points, out path.Tangents);
        path.GenerateAnglesFromTangents();
        return path;
    }

    // Generated a path, also consideres segmentation from waypoints
    public Path GeneratePath(Waypoint start, Waypoint end)
    {
        Path path = new Path();
        pathGenerator.CubicHermiteSpline(Path.WorldToPath(start.Position), Path.QuaternionToTangent(start.Rotation, TANGENT_MAGNITUDE), Path.WorldToPath(end.Position), Path.QuaternionToTangent(end.Rotation, TANGENT_MAGNITUDE),
            out path.Points, out path.Tangents);
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
}
