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
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotateLag);
        //Vector3 newPos = target.transform.position + offset;
        //transform.position = Vector3.Lerp(transform.position, newPos, followLag);
        //rot = Quaternion.Lerp(transform.rotation, target.transform.rotation, rotateLag);

        //transform.rotation = Quaternion.Euler(40f, rot.eulerAngles.y, rot.eulerAngles.z);
    }
}
