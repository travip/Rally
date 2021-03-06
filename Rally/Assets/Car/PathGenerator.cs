﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator
{
    private float[] H0;
    private float[] H1;
    private float[] H2;
    private float[] H3;

    private float[] H0d;
    private float[] H1d;
    private float[] H2d;
    private float[] H3d;

    public PathGenerator()
    {

        H0 = new float[Path.NumPoints];
        H1 = new float[Path.NumPoints];
        H2 = new float[Path.NumPoints];
        H3 = new float[Path.NumPoints];

        H0d = new float[Path.NumPoints];
        H1d = new float[Path.NumPoints];
        H2d = new float[Path.NumPoints];
        H3d = new float[Path.NumPoints];

        for (int i = 0; i < Path.NumPoints; i++)
        {
            float t = (1.0f / Path.NumPoints) * i;
            H0[i] = (2 * t * t * t - 3 * t * t + 1);
            H1[i] = (t * t * t - 2 * t * t + t);
            H2[i] = (-2 * t * t * t + 3 * t * t);
            H3[i] = (t * t * t - t * t);

            H0d[i] = (6 * t * t - 6 * t);
            H1d[i] = (3 * t * t - 4 * t + 1);
            H2d[i] = (-6 * t * t + 6 * t);
            H3d[i] = (3 * t * t - 2 * t);
        }
    }

    public void CubicHermiteSpline(Vector2 startPoint, Vector2 startTangent, Vector2 endPoint, Vector2 endTangent, out Vector2[] path, out Vector2[] tangent)
    {
        path = new Vector2[Path.NumPoints];
        tangent = new Vector2[Path.NumPoints];

        for (int i = 0; i < Path.NumPoints; i++)
        {
            path[i] = H0[i] * startPoint + H1[i] * startTangent + H2[i] * endPoint + H3[i] * endTangent;
            tangent[i] = H0d[i] * startPoint + H1d[i] * startTangent + H2d[i] * endPoint + H3d[i] * endTangent;
        }
    }
}
