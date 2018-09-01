#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class DebugMenu {
	

	private static Vector3 position;
	private static Quaternion rotation;

	[MenuItem("CONTEXT/Transform/Copy Global Transform", false, 151)]
	public static void CopyGlobalPosition() {
		position = Selection.activeTransform.position;
		rotation = Selection.activeTransform.rotation;
	}

	[MenuItem("CONTEXT/Transform/Paste Global Transform", false, 201)]
	public static void PasteGlobalPosition() {
		Transform[] selections = Selection.transforms;
		foreach (Transform selection in selections) {
			Undo.RecordObject(selection, "Paste Global Transform" + selection.name);
			selection.position = position;
			selection.localRotation = rotation;
		}
	}

	[MenuItem("Debug/Align roads")]
	public static void AlignRoads() {
		Debug.Log("Aligning all roads in the \"Roads\" gameobject");
		Transform roads = GameObject.Find("Roads").transform;
		Vector3 pos = Vector3.zero;
		Quaternion rot = Quaternion.identity;

		foreach (Transform road in roads) {
			road.position = pos;
			road.rotation = rot;
			Transform end = road.Find("positionNodes").Find("end2");
			pos = end.position;
			rot = end.rotation;
		}
	}
}
#endif