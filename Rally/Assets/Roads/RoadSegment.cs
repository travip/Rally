using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public Player.ActionType RoadType;
    public bool wasCorrect = false;

    [SerializeField]
    private Transform[] endPoints = new Transform[3];
    [SerializeField]
    private Transform[] midPoints = new Transform[3];

    [SerializeField]
    private Transform midMissLeft, midMissRight, missLeft, missRight;

    public RoadSegment NextSegment;
    public RoadSegment LastSegment;

    public Transform RoadCentre;
    public List<BoxCollider> NoTreeZones;

	public Color highlightTint;
	private Color previousTint;

	public RoadSegment nextSegment;

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
        Transform point = midPoints[Random.Range(0, 2)];
        //Transform point = midPoints[1];
        return new Waypoint(point.position, point.rotation, false);
    }

    public Waypoint GetRandomEndpoint()
    {
        Transform point = endPoints[Random.Range(0, 2)];
        //Transform point = endPoints[1];
        return new Waypoint(point.position, point.rotation, false);
    }

    public Waypoint CentreEndpoint
    {
        get
        {
            return new Waypoint(endPoints[1].position, endPoints[1].rotation, false);
        }
        private set { }
    }

    public Waypoint CentreMidpoint
    {
        get
        {
            return new Waypoint(midPoints[1].position, midPoints[1].rotation, false);
        }
        private set { }
    }

    public Waypoint MidMissLeft
    {
        get
        {
            return new Waypoint(midMissLeft.position, midMissLeft.rotation, false);
        }
        private set { }
    }

    public Waypoint MidMissRight
    {
        get
        {
            return new Waypoint(midMissRight.position, midMissRight.rotation, false);
        }
        private set { }
    }

    public Waypoint MissLeft
    {
        get
        {
            return new Waypoint(missLeft.position, missLeft.rotation, true);
        }
        private set { }
    }

    public Waypoint MissRight
    {
        get
        {
            return new Waypoint(missRight.position, missRight.rotation, true);
        }
        private set { }
    }

	public void HighLight() {
		Material mat = transform.Find("model").GetChild(0).GetComponent<Renderer>().material;
		previousTint = mat.color;
		mat.color = highlightTint;
	}

	public void UnHighLight() {
		transform.Find("model").GetChild(0).GetComponent<Renderer>().material.color = previousTint;
	}
}
