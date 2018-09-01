using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator
{
    public int NumPoints = 100;

    private float[] H0;
    private float[] H1;
    private float[] H2;
    private float[] H3;

    public PathGenerator(int numPoints = 100)
    {
        NumPoints = numPoints;

        H0 = new float[NumPoints];
        H1 = new float[NumPoints];
        H2 = new float[NumPoints];
        H3 = new float[NumPoints];

        for (int i = 0; i < NumPoints; i++)
        {
            float t = (1.0f / numPoints) * i;
            H0[i] = (2 * t * t * t - 3 * t * t + 1);
            H1[i] = (t * t * t - 2 * t * t + t);
            H2[i] = (-2 * t * t * t + 3 * t * t);
            H3[i] = (t * t * t - t * t);
        }
    }

    public Vector2[] GeneratePath(Vector2 startPoint, Vector2 startTangent, Vector2 endPoint, Vector2 endTangent, Player.ActionType action)
    {
        switch(action)
        {
            case Player.ActionType.RightBend:
                return RightBendPath(startPoint, startTangent, endPoint, endTangent);
            default:
                return new Vector2[0];
        }
    }

    public Vector2[] RightBendPath(Vector2 startPoint, Vector2 startTangent, Vector2 endPoint, Vector2 endTangent)
    {
        Vector2[] driveSpline = new Vector2[NumPoints];

        for(int i = 0; i < NumPoints; i++)
        {
            driveSpline[i] = H0[i] * startPoint + H1[i] * startTangent + H2[i] * endPoint + H3[i] * endTangent;
        }

        return driveSpline;
    }
}
