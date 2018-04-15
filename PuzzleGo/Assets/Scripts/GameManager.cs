using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// ════ publics ════
	public float stageDelay = 1f;
	public UnityEvent setupEvent;
	public UnityEvent startLevelEvent;
	public UnityEvent playLevelEvent;
	public UnityEvent endLevelEvent;
	// ════ properties ════
	bool m_hasLevelStarted = false;
	public bool HasLevelStarted { get { return m_hasLevelStarted; } set { m_hasLevelStarted = value; } }
	bool m_isGamePlaying = false;
	public bool IsGamePlaying { get { return m_isGamePlaying; } set { m_isGamePlaying = value; } }
	bool m_isGameOver = false;
	public bool IsGameOver { get { return m_isGameOver; } set { m_isGameOver = value; } }
	bool m_hasLevelFinished = false;
	public bool HasLevelFinished { get { return m_hasLevelFinished; } set { m_hasLevelFinished = value; } }
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
		if (m_player != null && m_board != null)
		{
			StartCoroutine ("RunGameLoop");
		}
		else
		{
			Debug.LogWarning ("GAMEMANAGER Error: no player or board found!");
		}
	}

	IEnumerator RunGameLoop ()
	{
		yield return StartCoroutine ("StartLevelRoutine");
		yield return StartCoroutine ("PlayLevelRoutine");
		yield return StartCoroutine ("EndLevelRoutine");
	}

	IEnumerator StartLevelRoutine ()
	{
		if (setupEvent != null)
		{
			setupEvent.Invoke ();
		}

		m_player.playerInput.InputEnabled = false;
		while (!m_hasLevelStarted)
		{
			// show start screen
			// user presses button to start
			// HasLevelStarted = true;
			yield return null;
		}

		if (startLevelEvent != null)
		{
			startLevelEvent.Invoke ();
		}
	}

	IEnumerator PlayLevelRoutine ()
	{
		m_isGamePlaying = true;
		yield return new WaitForSeconds (stageDelay);

		if (playLevelEvent != null)
		{
			playLevelEvent.Invoke ();
		}

		while (m_board.VisibleNodes != m_board.ActiveNodes)
		{
			yield return null;
		}

		m_player.playerInput.InputEnabled = true;

		while (!m_isGameOver)
		{
			yield return null;
			// check for Game Over condition
			// win
			// player reaches the end of the level
			m_isGameOver = IsWinner ();

			// lose
			// player dies

		}
	}

	IEnumerator EndLevelRoutine ()
	{
		m_player.playerInput.InputEnabled = false;

		if (endLevelEvent != null)
		{
			endLevelEvent.Invoke ();
		}

		// show end screen
		while (!m_hasLevelFinished)
		{
			// user presses button to continue

			// HasLevelFinished = true;
			yield return null;
		}

		RestartLevel ();
	}

	void RestartLevel ()
	{
		Scene scene = SceneManager.GetActiveScene ();
		SceneManager.LoadScene (scene.name);
	}

	public void PlayLevel ()
	{
		m_hasLevelStarted = true;
	}

	bool IsWinner ()
	{
		if (m_board.PlayerNode != null)
		{
			return (m_board.PlayerNode == m_board.GoalNode);
		}
		return false;
	}
}