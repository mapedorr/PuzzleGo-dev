using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : Mover
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	PlayerCompass m_playerCompass;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	protected override void Awake ()
	{
		base.Awake ();
		m_playerCompass = GetComponentInChildren<PlayerCompass> ();
	}

	protected override void Start ()
	{
		base.Start ();
		UpdateBoard ();
	}

	protected override IEnumerator MoveRoutine (Vector3 destinationPos, float delayTime)
	{
		if (m_playerCompass != null)
		{
			m_playerCompass.ShowArrows (false);
		}

		yield return StartCoroutine (base.MoveRoutine (destinationPos, delayTime));

		UpdateBoard ();

		if (m_playerCompass != null)
		{
			m_playerCompass.ShowArrows (true);
		}
	}

	void UpdateBoard ()
	{
		Debug.Log (m_board == null);
		if (base.m_board != null)
		{
			base.m_board.UpdatePlayerNode ();
		}
	}
}