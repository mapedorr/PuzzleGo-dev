using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	// TODO: define public variables
	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	// TODO: define properties
	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	PlayerManager m_player;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	void Awake ()
	{
		m_player = Object.FindObjectOfType<PlayerManager> ().GetComponent<PlayerManager> ();
	}

	public void Attack ()
	{
		if (m_player != null)
		{
			m_player.Die ();
		}
	}
}