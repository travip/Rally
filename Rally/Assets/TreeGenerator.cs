﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public List<RoadSegment> RoadsToUse;
    public List<BoxCollider> InvalidPlacements;
    public List<Transform> Trees;

    public float treeGenerationHalfWidth;
    public float treeGenerationHalfHeight;
    public Vector3 treeCenter;

    public const int ITER_MAX = 1000;
    public int iter = 0;

    public float expectedTreeDenity;
    public int numTrees;

    // MEAN / DEV for tree/rock SIZES
    public float mean;
    public float stdDev;

    public GameObject[] TreePrefabs;

    private void Awake()
    {
        RoadsToUse = new List<RoadSegment>();
    }

    private void Start()
    {
    }

    public void AddCollidersFromRoads(RoadSegment[] roads)
    {
        foreach(RoadSegment r in roads)
        {
            InvalidPlacements.AddRange(r.NoTreeZones);
        }
    }

    public void AddCollidersFromRoads(List<RoadSegment> roads)
    {
        foreach (RoadSegment r in roads)
        {
            InvalidPlacements.AddRange(r.NoTreeZones);
        }
    }

    public void AddCollidersFromRoads(RoadSegment roads)
    {
         InvalidPlacements.AddRange(roads.NoTreeZones);
    }

    public void GenerateTrees(RoadSegment road)
    {
        Trees = new List<Transform>();
        for(int i = 0; i < numTrees; i++)
        {
			int toSpawn = Random.Range(0, TreePrefabs.Length);
			GameObject newTree = Instantiate(TreePrefabs[toSpawn], transform);
            newTree.transform.position = GenerateRandomPosition(road);
            Trees.Add(newTree.transform);
        }

        DeleteInvalidTrees(road);
    }

    private void DeleteInvalidTrees(RoadSegment road)
    {
        foreach(Transform t in Trees)
        {
            foreach(BoxCollider b in road.NoTreeZones)
            {
                if (b.bounds.Contains(t.position))
                {
                    Destroy(t.gameObject);
                    break;
                }
            }
        }
    }

    public void DeleteAllTrees()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }

    // Generated a valid point for a road
    private Vector3 GenerateRandomPosition(RoadSegment road)
    {
		Transform t = road.transform;
        return new Vector3(Random.Range(-treeGenerationHalfWidth, treeGenerationHalfWidth), 
                           0f, 
                           Random.Range(-treeGenerationHalfHeight, treeGenerationHalfHeight)) 
						   + t.position;
    }

    private float RandomNormal()
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randNormal;
    }
}
