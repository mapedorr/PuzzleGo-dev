using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	public Vector3 destination;
	public bool isMoving = false;
	public iTween.EaseType easeType;
	public float moveSpeed = 1.5f;
	public float iTweenDelay = 0f;
	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	protected Board m_board;
	protected Node m_currentNode;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	protected virtual void Awake ()
	{
		m_board = Object.FindObjectOfType<Board> ().GetComponent<Board> ();
	}

	protected virtual void Start ()
	{
		UpdateCurrentNode ();
	}

	protected void Move (Vector3 destinationPos, float delayTime = 0.25f)
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
}