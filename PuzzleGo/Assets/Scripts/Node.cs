using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
	// ════ publics ════
	public GameObject geometry;
	public float scaleTime = 0.3f;
	public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
	public bool autoRun = false;
	public float delay = 1f;
	public GameObject linkPrefab;
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

			if (autoRun)
			{
				InitNode ();
			}

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
				"delay", delay
			));
		}
	}

	public List<Node> FindNeighbors (List<Node> ndoes)
	{
		List<Node> nList = new List<Node> ();
		foreach (Vector2 dir in Board.directions)
		{
			Node foundNeighbor = ndoes.Find (n => n.Coordinate == Coordinate + dir);

			if (foundNeighbor != null && !nList.Contains (foundNeighbor))
			{
				nList.Add (foundNeighbor);
			}
		}
		return nList;
	}

	public void InitNode ()
	{
		if (!m_initialized)
		{
			ShowGeometry ();
			InitNeighbors ();
			m_initialized = true;
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
				LinkNode (n);
				n.InitNode ();
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
}