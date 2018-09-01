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

    public bool DEBUG_DROP_OBJECT_ON_ALL_POINTS = false;

    Vector2[] currentPath;

    public static Vector2 WorldToPath(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector3 PathToWorld(Vector2 vec)
    {
        return new Vector3(vec.x, 0f, vec.y);
    }

    private void Awake()
    {
        pathGenerator = new PathGenerator(100);
    }

    public void PLACE_DEBUG_ON_PATH()
    {
        Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(START, Quaternion.identity);
        Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(END, Quaternion.identity);
    }

    public void TEXT_PATH()
    {
        currentPath = new Vector2[100];
        currentPath = pathGenerator.RightBendPath(WorldToPath(START), WorldToPath(START_TANGENT), WorldToPath(END), WorldToPath(END_TANGENT));

        for(int i = 0; i < 99; i++)
        {
            Debug.DrawLine(PathToWorld(currentPath[i]), PathToWorld(currentPath[i + 1]), Color.red, 10f);
            if(DEBUG_DROP_OBJECT_ON_ALL_POINTS)
                Instantiate(DEBUG_OBJECT).transform.SetPositionAndRotation(PathToWorld(currentPath[i]), Quaternion.identity);
        }
        PLACE_DEBUG_ON_PATH();
    }
}
