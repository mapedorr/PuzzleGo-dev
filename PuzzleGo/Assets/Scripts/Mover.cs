using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	public Vector3 destination;
	public bool isMoving = false;
	public iTween.EaseType easeType;
	public float moveSpeed = 1.5f;
	public float iTweenDelay = 0f;
	// the time it will take to the RotateTo tween to complete (no matter the amount)
	// of degrees to rotate to
	public float rotateTime = .5f;
	public bool faceDestination = false;
	public UnityEvent finishMovementEvent;
	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	protected Node m_currentNode;
	public Node CurrentNode { get { return m_currentNode; } }

	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	protected Board m_board;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	protected virtual void Awake ()
	{
		m_board = Object.FindObjectOfType<Board> ().GetComponent<Board> ();
	}

	protected virtual void Start ()
	{
		UpdateCurrentNode ();
	}

	public void Move (Vector3 destinationPos, float delayTime = 0.25f)
	{
		if (isMoving)
		{
			return;
		}

		if (m_board != null)
		{
			Node targetNode = m_board.FindNodeAt (destinationPos);

			if (targetNode != null && m_currentNode != null)
			{
				if (m_currentNode.LinkedNodes.Contains (targetNode))
				{
					StartCoroutine (MoveRoutine (destinationPos, delayTime));
				}
				else
				{
					Debug.Log ("Mover.Move WARNING: " + m_currentNode.name + " not connected to " + targetNode.name);
				}
			}
		}
	}

	protected virtual IEnumerator MoveRoutine (Vector3 destinationPos, float delayTime)
	{
		isMoving = true;
		destination = destinationPos;

		// optional turn to face destination
		if (faceDestination)
		{
			FaceDestination ();
			yield return new WaitForSeconds (.25f);
		}

		yield return new WaitForSeconds (delayTime);
		iTween.MoveTo (gameObject, iTween.Hash (
			"x", destinationPos.x,
			"y", destinationPos.y,
			"z", destinationPos.z,
			"delay", iTweenDelay,
			"easetype", easeType,
			"speed", moveSpeed
		));

		while (Vector3.Distance (destinationPos, transform.position) > 0.01f)
		{
			yield return null;
		}

		iTween.Stop (gameObject);
		transform.position = destinationPos;
		isMoving = false;

		UpdateCurrentNode ();
	}

	public void MoveLeft ()
	{
		Vector3 newPosition = transform.position + new Vector3 (-Board.spacing, 0f, 0f);
		Move (newPosition, 0f);
	}

	public void MoveRight ()
	{
		Vector3 newPosition = transform.position + new Vector3 (Board.spacing, 0f, 0f);
		Move (newPosition, 0f);
	}

	public void MoveForward ()
	{
		Vector3 newPosition = transform.position + new Vector3 (0f, 0f, Board.spacing);
		Move (newPosition, 0f);
	}

	public void MoveBackward ()
	{
		Vector3 newPosition = transform.position + new Vector3 (0f, 0f, -Board.spacing);
		Move (newPosition, 0f);
	}

	protected void UpdateCurrentNode ()
	{
		if (m_board != null)
		{
			m_currentNode = m_board.FindNodeAt (transform.position);
		}
	}

	protected void FaceDestination ()
	{
		// calculate vector from current position to destination position
		Vector3 relativePosition = destination - transform.position;
		// first parameter is the direction of destination, the second one is the up
		// vector
		Quaternion rotation = Quaternion.LookRotation (relativePosition, Vector3.up);
		// get the amount of degrees to rotate in the Y axis
		float newY = rotation.eulerAngles.y;
		iTween.RotateTo (gameObject, iTween.Hash (
			"y", newY,
			"delay", 0f,
			"easetype", easeType,
			"time", rotateTime
		));
	}
}