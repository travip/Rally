#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class DebugMenu {
	// [MenuItem("Debug/Print Global Position")]

	private static Vector3 position;
	private static Quaternion rotation;

	[MenuItem("CONTEXT/Transform/Copy Global Transform", false, 151)]
	public static void CopyGlobalPosition () {
		//if (Selection.activeGameObject != null) {
		//	Debug.Log(Selection.activeGameObject.name + " is at " + Selection.activeGameObject.transform.position);
		//}

		position = Selection.activeTransform.position;
		rotation = Selection.activeTransform.rotation;
	}

	[MenuItem("CONTEXT/Transform/Paste Global Transform", false, 201)]
	public static void PasteGlobalPosition () {
		Transform[] selections = Selection.transforms;
		foreach (Transform selection in selections) {
			Undo.RecordObject(selection, "Paste Global Transform" + selection.name);
			selection.position = position;
			selection.localRotation = rotation;
		}
	}
}
#endif