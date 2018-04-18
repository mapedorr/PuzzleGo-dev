using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCompass : MonoBehaviour
{
	// ════ publics ════
	public GameObject arrowPrefab;
	public float scale = 1f;
	public float startDistance = 0.25f;
	public float endDistance = 0.5f;
	public float moveTime = 1f;
	public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
	// ════ privates ════
	Board m_board;
	List<GameObject> m_arrows = new List<GameObject> ();

	// ════ methods ════
	void Awake ()
	{
		m_board = Object.FindObjectOfType<Board> ().GetComponent<Board> ();
		SetupArrows ();
	}

	void SetupArrows ()
	{
		if (arrowPrefab == null)
		{
			Debug.LogWarning ("PlayerCompass.SetupArrows ERROR: Missing arrow prefab!");
			return;
		}

		foreach (Vector2 dir in Board.directions)
		{
			Vector3 direction = new Vector3 (dir.normalized.x, 0f, dir.normalized.y);
			Quaternion rotation = Quaternion.LookRotation (direction);
			GameObject arrowInstance = Instantiate (arrowPrefab,
				transform.position + direction * startDistance,
				rotation);
			arrowInstance.transform.localScale = Vector3.one * scale;
			arrowInstance.transform.parent = transform;

			m_arrows.Add (arrowInstance);
		}
	}

	void MoveArrow (GameObject arrowInstance)
	{
		iTween.MoveBy (arrowInstance, iTween.Hash (
			"z", endDistance - startDistance,
			"looptype", iTween.LoopType.loop,
			"time", moveTime,
			"easetype", easeType
		));
	}

	void MoveArrows ()
	{
		foreach (GameObject arrow in m_arrows)
		{
			MoveArrow (arrow);
		}
	}

	public void ShowArrows (bool show)
	{
		if (m_board == null)
		{
			Debug.LogWarning ("PlayerCompass.ShowArrows ERROR: no Board found!");
		}

		if (m_arrows == null || m_arrows.Count != Board.directions.Length)
		{
			Debug.LogWarning ("PlayerCompass.ShowArrows ERROR: no arrows found!");
		}

		if (m_board.PlayerNode != null)
		{
			for (int i = 0; i < Board.directions.Length; i++)
			{
				Node neighbor = m_board.PlayerNode.FindNeighborAt (Board.directions[i]);
				if (neighbor == null || !show)
				{
					m_arrows[i].SetActive (false);
				}
				else
				{
					bool activeState = m_board.PlayerNode.LinkedNodes.Contains (neighbor);
					m_arrows[i].SetActive (activeState);
				}
			}
		}

		ResetArrows ();
		MoveArrows ();
	}

	void ResetArrows ()
	{
		for (int i = 0; i < Board.directions.Length; i++)
		{
			iTween.Stop (m_arrows[i]);
			Vector3 direction = new Vector3 (Board.directions[i].normalized.x, 0f,
				Board.directions[i].normalized.y);
			m_arrows[i].transform.position = transform.position + direction * startDistance;
		}
	}
}