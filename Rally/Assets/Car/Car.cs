using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

	public Player player;
	public float maxSpeed;
	public float acceleration;
	private Rigidbody rb;
	

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
		//rb.velocity = rb.velocity + transform.forward * acceleration;
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
	}

	private void OnTriggerEnter (Collider other) {
		
	}

	private void ReachDecisionPoint(Player.ActionType correctAction) {
		Player.ActionType nextAction = player.GetNextAction();
		if (nextAction == correctAction) {
			// All good
		}
		else {

		}
	}
}
