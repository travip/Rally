using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public Player.ActionType RoadType;

    private Transform[] endPoints = new Transform[3];
    private Transform[] midPoints = new Transform[3];

    public RoadSegment NextSegment;
    public RoadSegment LastSegment;

    public Vector3 GetEndConnector()
    {
        return endPoints[2].position;
    }

    public Vector3 GetStartConnector()
    {
        return transform.position;
    }

    public Vector3 GetRandomMidpoint()
    {
        return midPoints[Random.Range(0, 2)].position;
    }

    public Vector3 GetRandomEndpoint()
    {
        return endPoints[Random.Range(0, 2)].position;
    }
}
