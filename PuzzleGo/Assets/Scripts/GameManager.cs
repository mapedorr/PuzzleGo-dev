using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	// ════ publics ════
	// ════ properties ════
	// ════ privates ════
	Board m_board;
	PlayerManager m_player;

	void Awake ()
	{
		m_board = Object.FindObjectOfType<Board> ().GetComponent<Board> ();
		m_player = Object.FindObjectOfType<PlayerManager> ().GetComponent<PlayerManager> ();
	}

	// Use this for initialization
	void Start ()
	{

	}
}