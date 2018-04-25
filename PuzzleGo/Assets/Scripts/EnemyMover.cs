using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
	Stationary,
	Patrol,
	Spinner
}

public class EnemyMover : Mover
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	public float standTime = 1f;
	public Vector3 directionToMove = new Vector3 (0f, 0f, Board.spacing);
	public MovementType movementType = MovementType.Stationary;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	protected override void Awake ()
	{
		base.Awake ();
		// all enemies will look to its destination before moving to it
		faceDestination = true;
	}

	protected override void Start ()
	{
		base.Start ();
	}

	public void MoveOneTurn ()
	{
		switch (movementType)
		{
			case MovementType.Patrol:
				Patrol ();
				break;
			case MovementType.Stationary:
				Stand ();
				break;
			case MovementType.Spinner:
				Spin ();
				break;
		}
	}

	void Patrol ()
	{
		StartCoroutine (PatrolRoutine ());
	}

	IEnumerator PatrolRoutine ()
	{
		Vector3 startPosition = new Vector3 (m_currentNode.Coordinate.x, 0f,
			m_currentNode.Coordinate.y);

		// one node forward
		Vector3 newDestination = startPosition + transform.TransformVector (directionToMove);

		// two nodes forward
		Vector3 nextDestination = startPosition +
			transform.TransformVector (directionToMove * Board.spacing);

		Move (newDestination, 0f);

		while (isMoving)
		{
			yield return null;
		}

		if (m_board != null)
		{
			Node newDestinationNode = m_board.FindNodeAt (newDestination);
			Node nextDestinationNode = m_board.FindNodeAt (nextDestination);

			// if the enemy its at the end of the path or theres and obstacle in-between
			// the current node and the next one
			if (nextDestinationNode == null ||
				!newDestinationNode.LinkedNodes.Contains (nextDestinationNode))
			{
				// make it look at the node it came from
				destination = startPosition;
				FaceDestination ();

				// wait for the rotation animation to occur
				yield return new WaitForSeconds (rotateTime);
			}
		}

		InvokeFinishMovement ();
	}

	void Stand ()
	{
		StartCoroutine (StandRoutine ());
	}

	IEnumerator StandRoutine ()
	{
		yield return new WaitForSeconds (standTime);
		InvokeFinishMovement ();
	}

	void Spin ()
	{
		StartCoroutine (SpinRoutine ());
	}

	IEnumerator SpinRoutine ()
	{
		Vector3 localForward = new Vector3 (0f, 0f, Board.spacing);
		// the enemy will look to the node backwards him
		destination = transform.TransformVector (localForward * -1f) + transform.position;

		FaceDestination ();

		// wait for the rotation animation to occur
		yield return new WaitForSeconds (rotateTime);

		InvokeFinishMovement ();
	}

	void InvokeFinishMovement ()
	{
		if (finishMovementEvent != null)
		{
			finishMovementEvent.Invoke ();
		}
	}
}