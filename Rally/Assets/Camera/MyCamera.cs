using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour {

	public Transform target;
	private Vector3 offset;
	public float followLag;
    public float rotateLag;
    Quaternion rot;

    // Use this for initialization
    void Start ()
    {
		this.offset = transform.position - target.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, followLag);
		transform.LookAt(target.parent);
        //transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotateLag);
    }
}
