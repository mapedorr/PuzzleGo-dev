using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	// readonly: once this class is constructed for the first time, the directions
	// can't be changed
	public static float spacing = 2f;
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
	public List<Transform> capturePositions;
	public float capturePositionIconSize = .4f;
	public Color capturePositionIconColor = Color.blue;

	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
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

	int m_currentCapturePosition = 0;
	public int CurrentCapturePosition
	{
		get { return m_currentCapturePosition; }
		set { m_currentCapturePosition = value; }
	}

	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	PlayerMover m_playerMover;
	PlayerCompass m_playerCompass;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	void Awake ()
	{
		spacing = 2f;

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

	public List<EnemyManager> FindEnemiesAt (Node node)
	{
		List<EnemyManager> foundEnemies = new List<EnemyManager> ();
		EnemyManager[] enemies = Object.FindObjectsOfType<EnemyManager> () as EnemyManager[];

		foreach (EnemyManager enemy in enemies)
		{
			EnemyMover mover = enemy.GetComponent<EnemyMover> ();

			if (mover.CurrentNode == node)
			{
				foundEnemies.Add (enemy);
			}
		}

		return foundEnemies;
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

		Gizmos.color = capturePositionIconColor;

		foreach (Transform capturePos in capturePositions)
		{
			Gizmos.DrawCube (capturePos.position, Vector3.one * capturePositionIconSize);
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