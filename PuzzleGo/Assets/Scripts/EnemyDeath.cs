﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	public Vector3 offscreenOffset = new Vector3 (0f, 10f, 0f);
	public float deathDelay = 0f;
	public float offscreenDelay = 1f;
	public float iTweenDelay = 0f;
	public iTween.EaseType easeType = iTween.EaseType.easeInOutQuint;
	// the time it will take to iTween to move the piece out of the board
	public float moveTime = .5f;

	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	// TODO: define properties
	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	Board m_board;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	void Awake ()
	{
		m_board = Object.FindObjectOfType<Board> ().GetComponent<Board> ();
	}

	public void MoveOffboard (Vector3 target)
	{
		iTween.MoveTo (gameObject, iTween.Hash (
			"x", target.x,
			"y", target.y,
			"z", target.z,
			"delay", iTweenDelay,
			"easetype", easeType,
			"time", moveTime
		));
	}

	public void Die ()
	{
		StartCoroutine (DieRoutine ());
	}

	IEnumerator DieRoutine ()
	{
		yield return new WaitForSeconds (deathDelay);

		Vector3 offscreenPos = transform.position + offscreenOffset;

		MoveOffboard (offscreenPos);

		yield return new WaitForSeconds (moveTime + offscreenDelay);

		if (m_board.capturePositions.Count != 0 &&
			m_board.CurrentCapturePosition < m_board.capturePositions.Count)
		{
			Vector3 capturePos = m_board.capturePositions[m_board.CurrentCapturePosition].position;
			transform.position = capturePos + offscreenOffset;

			MoveOffboard (capturePos);

			yield return new WaitForSeconds (moveTime);

			m_board.CurrentCapturePosition++;
			m_board.CurrentCapturePosition = Mathf.Clamp (
				m_board.CurrentCapturePosition,
				0,
				m_board.capturePositions.Count - 1);
		}
	}
}