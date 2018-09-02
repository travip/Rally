using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public const int NumPoints = 20;

    public Vector2 StartPoint;
    public float StartAngle;

    public Vector2 EndPoint;
    public float EndAngle;

    public Vector2[] Points;
    public Vector2[] Tangents;

    public Quaternion[] Angles;

    public void GenerateAnglesFromTangents()
    {
        Angles = new Quaternion[NumPoints];
        for (int i = 0; i < NumPoints; i++)
            Angles[i] = TangentToRotation(Tangents[i]);
    }

    public static Vector2 WorldToPath(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector3 PathToWorld(Vector2 vec)
    {
        return new Vector3(vec.x, 0f, vec.y);
    }

    public static Vector2 QuaternionToTangent(Quaternion quart, float mag)
    {
        float radAngle = quart.eulerAngles.y * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radAngle), Mathf.Cos(radAngle)) * mag;
    }

    public static Quaternion TangentToRotation(Vector2 tangent)
    {
        float angle = Mathf.Atan2(tangent.x, tangent.y) * Mathf.Rad2Deg;
        return Quaternion.Euler(new Vector3(0f, angle, 0f));
    }
}
