using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private PathGenerator pathGenerator;

    public GameObject DEBUG_OBJECT;

    public Vector3 START;
    public Vector3 START_TANGENT;
    public Vector3 END;
    public Vector3 END_TANGENT;

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
        Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(PathToWorld(path[99]), TangentToRotation(angles[99]));
    }

    public void TEXT_PATH()
    {
        Debug.Log("START ANGLE: " + TangentToRotation(START_TANGENT).eulerAngles.y + "|| END ANGLE :" + TangentToRotation(END_TANGENT).eulerAngles.y);
        pathGenerator.RightBendPath(WorldToPath(START), WorldToPath(START_TANGENT), WorldToPath(END), WorldToPath(END_TANGENT).normalized, out path, out angles);

        for(int i = 0; i < 99; i++)
        {
            Debug.DrawLine(PathToWorld(path[i]), PathToWorld(path[i + 1]), Color.red, 10f);
            if (DEBUG_DROP_OBJECT_ON_EVERY_X_POINTS > 0)
            {
                if (i % DEBUG_DROP_OBJECT_ON_EVERY_X_POINTS == 0)
                    Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(PathToWorld(path[i]), TangentToRotation(angles[i]));
            }
        }
        PLACE_DEBUG_ON_PATH();
    }
}
