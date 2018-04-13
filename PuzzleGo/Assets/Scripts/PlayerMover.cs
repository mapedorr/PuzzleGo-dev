using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {
	public Vector3 destination;
	public bool isMoving = false;
	public iTween.EaseType easeType;

	public float moveSpeed = 1.5f;
	public float delay = 0f;

	// Use this for initialization
	void Start () {
		
	}

	public void Move(Vector3 destinationPos, float delayTime = 0.25f) {
		
	}
}
