using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeightedRoad {
	public GameObject prefab;
	public float weight;
}

public class RoadGenerator : MonoBehaviour {

	public TreeGenerator treeGenerator;
	public WeightedRoad[] AllRoadPrefabs;

    public GameObject straightRoadPrefab;

    public int roadsAhead = 5;
	public float castForward = 50f;
	public float closeEnough = 10f;
	public float tooClose = 5f;

	public List<Transform> currentRoads;


	// Use this for initialization
	void Awake () {

		// Calculate road weights as a percentage
		ReCalculateWeights(AllRoadPrefabs);
        System.Array.Sort<WeightedRoad>(AllRoadPrefabs, (x, y) => x.weight.CompareTo(y.weight));
		currentRoads = new List<Transform>();
	}

    public List<RoadSegment> CurrentRoadSegments
    {
        get
        {
            List<RoadSegment> segs = new List<RoadSegment>();
            foreach (Transform t in currentRoads)
                segs.Add(t.GetComponent<RoadSegment>());
            return segs;
        }
        private set { }
    }

	void ReCalculateWeights (WeightedRoad[] roads) {
		float total = 0;
		for (int i = 0; i < roads.Length; i++) {
			total += roads[i].weight;
		}
		for (int i = 0; i < roads.Length; i++) {
			roads[i].weight = roads[i].weight / total;
		}
	}

    public void AddNewRoadSection()
    {
		GameObject newRoad = Instantiate(straightRoadPrefab, transform);
		newRoad.GetComponent<BoxCollider>().enabled = true;
		currentRoads.Add(newRoad.transform);

		for (int i = 0; i < roadsAhead; i++)
            GenerateNextRoad();
    }

	int startToRemove = 10;
	int numToRemove = 10;
	int lastRemovedAt = 0;
	public void GenerateNextRoad() {
		int extraToGenerate = 0;
		if (currentRoads.Count == 0) {
			GameObject newFirstRoad1 = Instantiate(straightRoadPrefab, transform);
			newFirstRoad1.GetComponent<BoxCollider>().enabled = true;
			currentRoads.Add(newFirstRoad1.transform);
		}
		Transform lastEnd = GetEndNode(currentRoads[currentRoads.Count - 1]);

		// Get a list of roads that are close enough to check. Dont check the last road, we cant collide with that
		List<Transform> closeRoads = new List<Transform>();
		for (int i = 0; i < currentRoads.Count - 1; i++) {
			if (Vector3.Distance(lastEnd.position, currentRoads[i].position) <= closeEnough) {
				closeRoads.Add(currentRoads[i]);
			}
		}

		// Check random road segment types until we find one that fits
		//List<int> nums = new List<int>();
		//for (int i = 0; i < RoadPrefabs.Length; i++) {
		//	nums.Add(i);
		//}
		List<WeightedRoad> RoadPrefabs = new List<WeightedRoad>();
		for (int i = 0; i < AllRoadPrefabs.Length; i++) {
			RoadPrefabs.Add(AllRoadPrefabs[i]);
		}

		bool testthing = false;
		bool found = false;
		int random = 0;
		int sanityBreaker = 100;
		while (!found) {
			sanityBreaker -= 1;
			if (sanityBreaker <= 0) {
				//Debug.Log("Breaking out beacuse we reached sanity breaker!");
				break;
			}
			//if (currentRoads.Count >= 30) testthing = true;
			if (RoadPrefabs.Count == 0 || testthing) {
				// Failed to make a valid road. Lets delete the last few and try again from there.
				if (currentRoads.Count - numToRemove == lastRemovedAt) {
					numToRemove += 5;
				}

				if (numToRemove > currentRoads.Count)
					numToRemove = currentRoads.Count;
				//Debug.Log("");
				//Debug.Log("                  No path possible here!! noooooo. deleting " + numToRemove + " roads and trying again");
				//Debug.Log("");

				for (int i = 1; i <= numToRemove; i++) {
					Transform t = currentRoads[currentRoads.Count - 1];
					//Debug.Log("Deleting Road " + t.gameObject.name);
					Destroy(t.gameObject);
					currentRoads.RemoveAt(currentRoads.Count - 1);
					extraToGenerate += 1;
				}
				//for (int i = 0; i < RoadPrefabs.Count; i++) {
				//	nums.Add(i);
				//}

				
				for (int i = 0; i < AllRoadPrefabs.Length; i++) {
					RoadPrefabs.Add(AllRoadPrefabs[i]);
				}
				lastRemovedAt = currentRoads.Count;

				///////////////////////////////////////////
				// DUPLICATED CODE FROM ABOVE HOORAY
				if (currentRoads.Count == 0) {
					GameObject newFirstRoad1 = Instantiate(straightRoadPrefab, transform);
					newFirstRoad1.GetComponent<BoxCollider>().enabled = true;
					currentRoads.Add(newFirstRoad1.transform);
				}
				lastEnd = GetEndNode(currentRoads[currentRoads.Count - 1]);

				// Get a list of roads that are close enough to check. Dont check the last road, we cant collide with that
				for (int i = 0; i < currentRoads.Count - 1; i++) {
					if (Vector3.Distance(lastEnd.position, currentRoads[i].position) <= closeEnough) {
						closeRoads.Add(currentRoads[i]);
					}
				}
				// DUPLICATED CODE FROM ABOVE HOORAY
				///////////////////////////////////////////
			}

			float randomWeight = Random.Range(0.0f, 1.0f); // 0.5
			random = 0;
			for (int i = 0; i < RoadPrefabs.Count; i++) {
				if (RoadPrefabs[i].weight >= randomWeight) { // 0.03 >= 0.5? NO
					random = i;
					break;
				}
				randomWeight -= RoadPrefabs[i].weight;
			}
			//random = nums[Random.Range(0, nums.Count)];
			//nums.Remove(random);
			RoadPrefabs.RemoveAt(random);

			//Debug.Log("Trying to generate: " + AllRoadPrefabs[random].prefab.name);
			Transform potentialEnd = GetEndNode(AllRoadPrefabs[random].prefab.transform);
			if (lastEnd.parent.localScale.x == -1) {
				lastEnd.localScale = new Vector3(-1, 1, 1);
			}
			Vector3 potentialPos = lastEnd.TransformPoint(potentialEnd.position);
			Vector3 potentialRot = lastEnd.eulerAngles + potentialEnd.rotation.eulerAngles;

			Vector3[] newNodes = new Vector3[5];
			newNodes[0] = lastEnd.TransformPoint(GetMiddleNode(AllRoadPrefabs[random].prefab.transform).position);
			newNodes[1] = potentialPos;
			Quaternion quat = Quaternion.AngleAxis(potentialRot.y, Vector3.up);
			newNodes[2] = potentialPos + (quat * Vector3.forward) * castForward;
			lastEnd.localScale = Vector3.one;

			// Add nodes between these nodes
			newNodes[3] = Vector3.Lerp(newNodes[0], newNodes[1], 0.5f);
			newNodes[4] = Vector3.Lerp(newNodes[1], newNodes[2], 0.5f);

			// Check if this road crosses a previous road, or makes the next road impossible to not cross a road
			// Rather than checking lines, just check distance of 
			// new roads (middle, end, projected) compared to 
			// old roads (start, middle, end)
			bool dammit = false;

			////Debug.Log("!!!!!!!!!!!!!! Number of closeRoads: " + closeRoads.Count);
			foreach (Transform road in closeRoads) {
				Vector3[] oldNodes = new Vector3[5];
				oldNodes[0] = road.position;
				oldNodes[1] = GetMiddleNode(road).position;
				oldNodes[2] = GetEndNode(road).position;
				// Add nodes between these nodes
				oldNodes[3] = Vector3.Lerp(oldNodes[0], oldNodes[1], 0.5f);
				oldNodes[4] = Vector3.Lerp(oldNodes[1], oldNodes[2], 0.5f);
				//for (int i = 0; i < 3; i++)
				//	//Debug.Log("oldNodes[" + i + "]: " + oldNodes[i]);
				//for (int i = 0; i < 3; i++)
				//	//Debug.Log("newNodes[" + i + "]: " + newNodes[i]);

				for (int i = 0; i < 5; i++) {
					for (int j = 0; j < 5; j++) {
						if (Vector3.Distance(newNodes[i], oldNodes[j]) < tooClose) {
							//Debug.Log("Road too close: " + road.gameObject.name);
							////Debug.Log("Node " + i + " on NEW: " + newNodes[i] + " : " +
							//	"Node " + j + " on OLD: " + oldNodes[j] + "  |  " + 
							//	Vector3.Distance(newNodes[i], oldNodes[j]));
							// This road won't work
							dammit = true;
							break;
						}
					}
					if (dammit)
						break;
				}
				if (dammit)
					break;
			}

			if (!dammit)
				found = true;
			else {
				// Recalculate weights
				float total = 0;
				for (int i = 0; i < RoadPrefabs.Count; i++) {
					total += RoadPrefabs[i].weight;
				}
				for (int i = 0; i < RoadPrefabs.Count; i++) {
					WeightedRoad rd = RoadPrefabs[i];
					rd.weight = RoadPrefabs[i].weight / total;
					RoadPrefabs[i] = rd;
				}
			}
		}

		GameObject newRoad = Instantiate(
			AllRoadPrefabs[random].prefab,
			lastEnd.position,
			lastEnd.rotation,
			transform
		);
		newRoad.name = currentRoads.Count + " " + AllRoadPrefabs[random].prefab;
		Transform prevRoad = currentRoads[currentRoads.Count - 1];
		RoadSegment roadSeg = newRoad.GetComponent<RoadSegment>();
		prevRoad.GetComponent<RoadSegment>().nextSegment = roadSeg;
		//Debug.Log("!!!!!! Instantiating: " + newRoad.name);
		currentRoads.Add(newRoad.transform);

		if (extraToGenerate > 0)
			Debug.Log("Extra: " + extraToGenerate);
		for (int i = 0; i < extraToGenerate; i++) {
			GenerateNextRoad();
		}

		treeGenerator.GenerateTrees(roadSeg);
	}

	Transform GetEndNode(Transform road) {
		return road.transform.Find("positionNodes").Find("end2");
	}

	Transform GetMiddleNode (Transform road) {
		return road.transform.Find("positionNodes").Find("middle2");
	}

	//static bool FasterLineSegmentIntersection (Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {

	//	Vector2 a = p2 - p1;
	//	Vector2 b = p3 - p4;
	//	Vector2 c = p1 - p3;

	//	float alphaNumerator = b.y * c.x - b.x * c.y;
	//	float alphaDenominator = a.y * b.x - a.x * b.y;
	//	float betaNumerator = a.x * c.y - a.y * c.x;
	//	float betaDenominator = a.y * b.x - a.x * b.y;

	//	bool doIntersect = true;

	//	if (alphaDenominator == 0 || betaDenominator == 0) {
	//		doIntersect = false;
	//	}
	//	else {

	//		if (alphaDenominator > 0) {
	//			if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) {
	//				doIntersect = false;

	//			}
	//		}
	//		else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) {
	//			doIntersect = false;
	//		}

	//		if (doIntersect && betaDenominator > 0) {
	//			if (betaNumerator < 0 || betaNumerator > betaDenominator) {
	//				doIntersect = false;
	//			}
	//		} else if (betaNumerator > 0 || betaNumerator < betaDenominator) {
	//			doIntersect = false;
	//		}
	//	}

	//	return doIntersect;
	//}
}
