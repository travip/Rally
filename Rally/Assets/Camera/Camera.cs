using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

	public GameObject target;
	private Vector3 offset;
	public float followLag;

	// Use this for initialization
	void Start () {
		this.offset = transform.position - target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newPos = target.transform.position + offset;
		this.transform.position = Vector3.Lerp(transform.position, newPos, followLag);
	}
}
