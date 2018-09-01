using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public Player.ActionType RoadType;

    [SerializeField]
    private Transform[] endPoints = new Transform[3];
    [SerializeField]
    private Transform[] midPoints = new Transform[3];

    public Transform FailLeft;
    public Transform FailRight;
    public Transform MissLeft;
    public Transform MissRight;

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

    public Waypoint GetRandomMidpoint()
    {
        //Transform point = midPoints[Random.Range(0, 2)];
        Transform point = midPoints[2];
        return new Waypoint(point.position, point.rotation, false);
    }

    public Waypoint GetRandomEndpoint()
    {
        //Transform point = endPoints[Random.Range(0, 2)];
        Transform point = endPoints[2];
        return new Waypoint(point.position, point.rotation, true);
    }
}
