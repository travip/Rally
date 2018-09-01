using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
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

    public static Vector2 WorldToPath(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector3 PathToWorld(Vector2 vec)
    {
        return new Vector3(vec.x, 0f, vec.y);
    }

    public static Quaternion TangentToRotation(Vector2 tangent)
    {
        float angle = Mathf.Atan2(tangent.x, tangent.y) * Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0f, angle, 0f));
    }

    private void Awake()
    {
        pathGenerator = new PathGenerator(100);
    }

    public void PLACE_DEBUG_ON_PATH()
    {
        Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(PathToWorld(PATH_POINTS[PATH_POINTS.Count - 1]), TangentToRotation(ANGLE_POINTS[PATH_POINTS.Count - 1]));
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
            pathGenerator.RightBendPath(WorldToPath(DEBUG_POINTS[i]), WorldToPath(DEBUG_ANGLES[i]), WorldToPath(DEBUG_POINTS[i+1]), WorldToPath(DEBUG_ANGLES[i+1]), out path, out angles);
            PATH_POINTS.AddRange(path);
            ANGLE_POINTS.AddRange(angles);
        }

        Debug.Log("Path count: " + PATH_POINTS.Count);

        for(int i = 0; i < PATH_POINTS.Count - 1; i++)
        {
            Debug.DrawLine(PathToWorld(PATH_POINTS[i]), PathToWorld(PATH_POINTS[i + 1]), Color.red, 100f);
            if (DEBUG_DROP_OBJECT_ON_EVERY_X_POINTS > 0)
            {
                if (i % DEBUG_DROP_OBJECT_ON_EVERY_X_POINTS == 0)
                    Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(PathToWorld(PATH_POINTS[i]), TangentToRotation(ANGLE_POINTS[i]));
            }
        }
        PLACE_DEBUG_ON_PATH();
    }
}
