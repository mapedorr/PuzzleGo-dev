using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
	// ════ publics ════
	public GameObject geometry;
	public float scaleTime = 0.3f;
	public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
	public float delay = 1f;
	public GameObject linkPrefab;
	public LayerMask obstacleLayer;
	public bool isLevelGoal = false;
	// ════ properties ════
	Vector2 m_coordinate;
	public Vector2 Coordinate { get { return Utility.Vector2Round (m_coordinate); } }
	private List<Node> m_neighborNodes = new List<Node> ();
	public List<Node> NeighborNodes { get { return m_neighborNodes; } }
	private List<Node> m_linkedNodes = new List<Node> ();
	public List<Node> LinkedNodes { get { return m_linkedNodes; } }
	// ════ privates ════
	Board m_board;
	bool m_initialized = false;

	// ════ methods ════
	void Awake ()
	{
		m_board = Object.FindObjectOfType<Board> ();
		m_coordinate = new Vector2 (transform.position.x, transform.position.z);
	}

	// Use this for initialization
	void Start ()
	{
		if (geometry != null)
		{
			geometry.transform.localScale = Vector3.zero;

			if (m_board != null)
			{
				m_neighborNodes = FindNeighbors (m_board.AllNodes);
			}
		}
	}

	public void ShowGeometry ()
	{
		if (geometry != null)
		{
			iTween.ScaleTo (geometry, iTween.Hash (
				"time", scaleTime,
				"scale", Vector3.one,
				"easetype", easeType,
				"delay", delay,
				"oncomplete", "GeometryVisible",
				"oncompletetarget", gameObject
			));
		}
	}

	public void GeometryVisible ()
	{
		m_board.VisibleNodes++;
		if (isLevelGoal)
		{
			m_board.DrawGoal ();
		}
	}

	public List<Node> FindNeighbors (List<Node> nodes)
	{
		List<Node> nList = new List<Node> ();
		foreach (Vector2 dir in Board.directions)
		{
			Node foundNeighbor = FindNeighborAt (nodes, dir);

			if (foundNeighbor != null && !nList.Contains (foundNeighbor))
			{
				nList.Add (foundNeighbor);
			}
		}
		return nList;
	}

	public Node FindNeighborAt (List<Node> nodes, Vector2 dir)
	{
		return nodes.Find (n => n.Coordinate == Coordinate + dir);
	}

	public Node FindNeighborAt (Vector2 dir)
	{
		return FindNeighborAt (NeighborNodes, dir);
	}

	public void InitNode ()
	{
		if (!m_initialized)
		{
			ShowGeometry ();
			InitNeighbors ();
			m_initialized = true;
			m_board.ActiveNodes++;
		}
	}

	void InitNeighbors ()
	{
		StartCoroutine (InitNeighborsRoutine ());
	}

	IEnumerator InitNeighborsRoutine ()
	{
		yield return new WaitForSeconds (delay);

		foreach (Node n in m_neighborNodes)
		{
			if (!m_linkedNodes.Contains (n))
			{
				Obstacle obstacle = FindObstacle (n);
				if (obstacle == null)
				{
					LinkNode (n);
					n.InitNode ();
				}
			}
		}
	}

	void LinkNode (Node targetNode)
	{
		if (linkPrefab != null)
		{
			GameObject linkInstance = Instantiate (linkPrefab, transform.position, Quaternion.identity);
			linkInstance.transform.parent = transform;

			Link link = linkInstance.GetComponent<Link> ();
			if (link != null)
			{
				link.DrawLink (transform.position, targetNode.transform.position);
			}
			if (!m_linkedNodes.Contains (targetNode))
			{
				m_linkedNodes.Add (targetNode);
			}
			if (!targetNode.LinkedNodes.Contains (this))
			{
				targetNode.LinkedNodes.Add (this);
			}
		}
	}

	Obstacle FindObstacle (Node targetNode)
	{
		Vector3 checkDirection = targetNode.transform.position - transform.position;
		RaycastHit raycastHit;

		if (Physics.Raycast (transform.position, checkDirection, out raycastHit,
				Board.spacing + 0.1f, obstacleLayer))
		{
			return raycastHit.collider.GetComponent<Obstacle> ();
		}

		return null;
	}

	void OnDrawGizmos ()
	{
		if (isLevelGoal)
		{
			Gizmos.color = new Color (0f, 1f, 0f, 0.5f);
			Gizmos.DrawSphere (transform.position, 0.2f);
		}
	}
}