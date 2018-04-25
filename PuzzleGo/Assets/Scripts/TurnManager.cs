﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	protected bool m_isTurnComplete;
	public bool IsTurnComplete { get { return m_isTurnComplete; } set { m_isTurnComplete = value; } }

	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	protected GameManager m_gameManager;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	protected virtual void Awake ()
	{
		m_gameManager = Object.FindObjectOfType<GameManager> ().GetComponent<GameManager> ();
	}

	// complete the turn and notify the GameManager
	public virtual void FinishTurn ()
	{
		m_isTurnComplete = true;

		if (m_gameManager != null)
		{
			m_gameManager.UpdateTurn ();
		}
	}
}