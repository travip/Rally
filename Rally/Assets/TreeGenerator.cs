using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public List<RoadSegment> RoadsToUse;

    public float treeGenerationHalfWidth;
    public float treeGenerationHalfHeight;

    public const int ITER_MAX = 1000;
    public int iter = 0;

    public float expectedTreeDenity;
    public int numTrees;

    // MEAN / DEV for tree/rock SIZES
    public float mean;
    public float stdDev;

    public GameObject TreeObject1;
    public GameObject TreeObject2;
    public GameObject RockObject;

    public RoadSegment DEBUG_ROAD;
    public GameObject DEBUG_CUBE;

    private void Awake()
    {
        RoadsToUse = new List<RoadSegment>();
    }

    private void Start()
    {
        GenerateTreesForRoadSegment(DEBUG_ROAD);
    }

    private void GenerateTreesForRoadSegment(RoadSegment road)
    {
        Debug.Log("Making new trees");
        //Find a valid place to put the tree
        for(int i = 0; i < numTrees; i++)
        {
            Vector3 treePos = GenerateRandomPosition(road);
            GameObject newTree = Instantiate(TreeObject1);
            newTree.transform.position = treePos;
            Debug.Log("Made a new tree!");
            
        }
    }

    // Generated a valid point for a road
    private Vector3 GenerateRandomPosition(RoadSegment road)
    {
        iter = 0;
        bool valid = false;
        Vector3 treeLoc = Vector3.zero;
        while (!valid)
        {
            iter++;
            valid = true;
            treeLoc = new Vector3(Random.Range(-treeGenerationHalfWidth, treeGenerationHalfWidth), 0f, Random.Range(-treeGenerationHalfHeight, treeGenerationHalfHeight)) + road.CentreMidpoint.Position;

            foreach (BoxCollider box in road.NoTreeZones)
            {
                if (box.bounds.Contains(treeLoc))
                    valid = false;
            }
            if (iter >= ITER_MAX)
            {
                Debug.Log("MAX ITERATIOONS EXCEEDED");
                return Vector3.zero;
            }
        }

        return treeLoc;
    }

    private float RandomNormal()
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randNormal;
    }
}
