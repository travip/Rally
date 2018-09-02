using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour {

	private Material mat;
	public Transform[] ends;

	public void GetDestroyed () {
		mat = GetComponent<Renderer>().material;
		StartCoroutine(FadeOut(0.5f));
	}

	private IEnumerator FadeOut (float transTime) {
		float elapsedTime = 0.0f;
		Color c = Color.white;
		Quaternion startRot = transform.rotation;
		Quaternion endRot = ends[Random.Range(0, ends.Length)].rotation;

		while (elapsedTime < transTime) {
			float timeScaler = elapsedTime / transTime;
			c.a = Mathf.Lerp(1f, 0f, timeScaler);
			transform.rotation = Quaternion.Lerp(startRot, endRot, timeScaler);
			mat.color = c;
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		Destroy(gameObject);
	}

	private void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Straight") || other.CompareTag("Turn")) {
			Debug.Log("baaaaaaaaa");
			Destroy(gameObject);
		}
	}
}
