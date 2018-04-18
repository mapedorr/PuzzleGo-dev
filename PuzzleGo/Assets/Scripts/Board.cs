using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	// ════ publics ════
	public static float spacing = 2f;
	// readonly: once this class is constructed for the first time, the directions
	// can't be changed
	public static readonly Vector2[] directions = {
		new Vector2 (spacing, 0f),
		new Vector2 (-spacing, 0f),
		new Vector2 (0f, spacing),
		new Vector2 (0f, -spacing)
	};
	public GameObject goalPrefab;
	public float drawGoalTime = 2f;
	public float drawGoalDelay = 2f;
	public iTween.EaseType drawGoalEaseType = iTween.EaseType.easeOutExpo;
	// ════ properties ════
	List<Node> m_allNodes;
	public List<Node> AllNodes { get { return m_allNodes; } }
	Node m_playerNode;
	public Node PlayerNode { get { return m_playerNode; } }
	Node m_goalNode;
	public Node GoalNode { get { return m_goalNode; } }
	int m_visibleNodes = 0;
	public int VisibleNodes { get { return m_visibleNodes; } set { m_visibleNodes = value; } }
	int m_activeNodes = 0;
	public int ActiveNodes { get { return m_activeNodes; } set { m_activeNodes = value; } }
	// ════ privates ════
	PlayerMover m_playerMover;
	PlayerCompass m_playerCompass;

	// ════ methods ════
	void Awake ()
	{
		m_playerMover = Object.FindObjectOfType<PlayerMover> ().GetComponent<PlayerMover> ();
		m_playerCompass = m_playerMover.GetComponentInChildren<PlayerCompass> ();
		FillNodeList ();

		m_goalNode = FindGoalNode ();
	}

	public void FillNodeList ()
	{
		Node[] nList = GameObject.FindObjectsOfType<Node> ();
		m_allNodes = new List<Node> (nList);
	}

	public Node FindNodeAt (Vector3 pos)
	{
		Vector2 boardCoord = Utility.Vector2Round (new Vector2 (pos.x, pos.z));
		return m_allNodes.Find (n => n.Coordinate == boardCoord);
	}

	Node FindGoalNode ()
	{
		return m_allNodes.Find (n => n.isLevelGoal);
	}

	public Node FindPlayerNode ()
	{
		if (m_playerMover != null && !m_playerMover.isMoving)
		{
			return FindNodeAt (m_playerMover.transform.position);
		}

		return null;
	}

	public void UpdatePlayerNode ()
	{
		m_playerNode = FindPlayerNode ();
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = new Color (0f, 1f, 1f, 0.5f);
		if (m_playerNode != null)
		{
			Gizmos.DrawSphere (m_playerNode.transform.position, 0.2f);
		}
	}

	public void DrawGoal ()
	{
		if (goalPrefab != null && m_goalNode != null)
		{
			GameObject goalInstance = Instantiate (goalPrefab,
				m_goalNode.transform.position,
				Quaternion.identity);

			iTween.ScaleFrom (goalInstance, iTween.Hash (
				"scale", Vector3.zero,
				"time", drawGoalTime,
				"delay", drawGoalDelay,
				"easetype", drawGoalEaseType
			));

			if (m_playerCompass != null)
			{
				m_playerCompass.ShowArrows (true);
			}
		}
	}

	public void InitBoard ()
	{
		if (m_playerNode != null)
		{
			m_playerNode.InitNode ();
		}
	}
}