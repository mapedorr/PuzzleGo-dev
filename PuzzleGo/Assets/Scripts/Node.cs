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
	// ════ properties ════
	Vector2 m_coordinate;
	public Vector2 Coordinate { get { return Utility.Vector2Round (m_coordinate); } }
	private List<Node> m_neighborNodes;
	public List<Node> NeighborNodes { get { return m_neighborNodes; } }
	// ════ privates ════
	Board m_board;

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
				ShowGeometry ();
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
}