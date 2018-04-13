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
	// ════ properties ════
	private List<Node> m_allNodes;
	public List<Node> AllNodes { get { return m_allNodes; } }
	// ════ privates ════

	void Awake ()
	{
		FillNodeList ();
	}

	public void FillNodeList ()
	{
		Node[] nList = GameObject.FindObjectsOfType<Node> ();
		m_allNodes = new List<Node> (nList);
	}
}