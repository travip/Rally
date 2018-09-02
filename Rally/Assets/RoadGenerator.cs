using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {

	public GameObject[] RoadPrefabs;

	public int roadsAhead = 5;
	public float castForward = 50f;
	public float closeEnough = 10f;
	public float tooClose = 5f;

	private List<Transform> currentRoads;


	// Use this for initialization
	void Start () {
		currentRoads = new List<Transform>();
		currentRoads.Add(Instantiate(RoadPrefabs[0]).transform);
		for (int i = 0; i < roadsAhead; i++) {
			GenerateNextRoad();
		}
	}
	
	void GenerateNextRoad() {
		Transform lastEnd = GetEndNode(currentRoads[currentRoads.Count - 1]);

		// Get a list of roads that are close enough to check. Dont check the last road, we cant collide with that
		List<Transform> closeRoads = new List<Transform>();
		for (int i = 0; i < currentRoads.Count - 1; i++) {
			if (Vector3.Distance(lastEnd.position, currentRoads[i].position) >= closeEnough) {
				closeRoads.Add(currentRoads[i]);
			}
		}

		// Check random road segment types until we find one that fits
		List<int> nums = new List<int>();
		for (int i = 0; i < RoadPrefabs.Length; i++) {
			nums.Add(i);
		}

		bool found = false;
		int random = 0;
		while (!found) {
			if (nums.Count == 0) {
				Debug.Log("No path possible here!! noooooo");
				break;
			}

			random = nums[Random.Range(0, nums.Count)];
			nums.Remove(random);

			Debug.Log("Trying to generate: " + RoadPrefabs[random].name);
			Transform potentialEnd = GetEndNode(RoadPrefabs[random].transform);
			if (lastEnd.parent.localScale.x == -1) {
				lastEnd.localScale = new Vector3(-1, 1, 1);
			}
			Vector3 potentialPos = lastEnd.TransformPoint(potentialEnd.position);

			// This part is NOT WORKING PROPERLY
			Vector3 potentialRot = lastEnd.TransformDirection(potentialEnd.rotation.eulerAngles);
			////
			Vector3[] newNodes = new Vector3[3];
			newNodes[0] = lastEnd.TransformPoint(GetMiddleNode(RoadPrefabs[random].transform).position);
			newNodes[1] = potentialPos;

			// This part is NOT WORKING PROPERLY
			newNodes[2] = potentialPos + potentialRot * castForward;
			Debug.Log("potentialPos: " + potentialPos);
			Debug.Log("potentialRot: " + potentialRot);
			Debug.Log("newNodes[2]: " + newNodes[2]);
			////
			lastEnd.localScale = Vector3.one;

			// Check if this road crosses a previous road, or makes the next road impossible to not cross a road
			// For prev road: Make a line from start to middle2 to end2. 
			// For new road: Make a line from start to middle2 to end2 and project forward another half-length
			// If the lines cross, this road type cant be used.

			// Rather than checking lines, just check distance of 
			// new roads middle, end, projected 
			//	compared to 
			// old roads start, middle, end
			bool dammit = false;
			foreach (Transform road in closeRoads) {
				Vector3[] oldNodes = new Vector3[3];
				oldNodes[0] = road.position;
				oldNodes[1] = GetMiddleNode(road).position;
				oldNodes[2] = GetEndNode(road).position;

				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 3; j++) {
						if (Vector3.Distance(newNodes[i], oldNodes[j]) < tooClose) {
							Debug.Log("Road too close: " + road.gameObject.name);
							Debug.Log("Node " + i + " on NEW: " + newNodes[i] + " : " +
								"Node " + j + " on OLD: " + oldNodes[j] + "  |  " + 
								Vector3.Distance(newNodes[i], oldNodes[j]));
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
		}

		GameObject newRoad = Instantiate(
			RoadPrefabs[random],
			lastEnd.position,
			lastEnd.rotation
		);
		newRoad.name = currentRoads.Count + " " + RoadPrefabs[random];
		Debug.Log("Instantiating: " + newRoad.name);
		currentRoads.Add(newRoad.transform);
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
