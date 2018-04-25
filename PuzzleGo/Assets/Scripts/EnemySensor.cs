using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	// check if the PC is in the node forward to the enemy (in the direction it is
	// looking at)
	public Vector3 directionToSearch = new Vector3 (0f, 0f, 2f);
	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	bool m_foundPlayer = false;
	public bool FoundPlayer { get { return m_foundPlayer; } }
	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	Node m_nodeToSearch;
	Board m_board;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	void Awake ()
	{
		m_board = Object.FindObjectOfType<Board> ().GetComponent<Board> ();
	}

	public void UpdateSensor (Node currentNode)
	{
		// convert looking direction to a world space position
		Vector3 worldSpacePositionToSearch = transform.TransformVector (directionToSearch) +
			transform.position;

		if (m_board != null)
		{
			m_nodeToSearch = m_board.FindNodeAt (worldSpacePositionToSearch);

			if (!currentNode.LinkedNodes.Contains (m_nodeToSearch))
			{
				// if the nodes are not connected, the enemy can't kill the player
				m_foundPlayer = false;
				return;
			}

			if (m_nodeToSearch == m_board.PlayerNode)
			{
				m_foundPlayer = true;
			}
		}
	}

	void Update () { }

}