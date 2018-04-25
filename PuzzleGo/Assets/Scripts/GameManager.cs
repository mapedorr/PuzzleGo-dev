using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// [System.Serializable] >> make this appear in the editor
[System.Serializable]
public enum Turn
{
	Player,
	Enemy
}

public class GameManager : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	public float stageDelay = 1f;
	public UnityEvent setupEvent;
	public UnityEvent startLevelEvent;
	public UnityEvent playLevelEvent;
	public UnityEvent endLevelEvent;
	public UnityEvent loseLevelEvent;

	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	bool m_hasLevelStarted = false;
	public bool HasLevelStarted { get { return m_hasLevelStarted; } set { m_hasLevelStarted = value; } }
	bool m_isGamePlaying = false;
	public bool IsGamePlaying { get { return m_isGamePlaying; } set { m_isGamePlaying = value; } }
	bool m_isGameOver = false;
	public bool IsGameOver { get { return m_isGameOver; } set { m_isGameOver = value; } }
	bool m_hasLevelFinished = false;
	public bool HasLevelFinished { get { return m_hasLevelFinished; } set { m_hasLevelFinished = value; } }

	// this will determine if it is the PC turn or the NPCs turn
	Turn m_currentTurn = Turn.Player;
	public Turn CurrentTurn { get { return m_currentTurn; } }

	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	Board m_board;
	PlayerManager m_player;
	List<EnemyManager> m_enemies;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	void Awake ()
	{
		m_board = GameObject.FindObjectOfType<Board> ().GetComponent<Board> ();
		m_player = GameObject.FindObjectOfType<PlayerManager> ().GetComponent<PlayerManager> ();
		EnemyManager[] enemies = GameObject.FindObjectsOfType<EnemyManager> () as EnemyManager[];
		m_enemies = enemies.ToList ();
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

	public void LoseLevel ()
	{
		StartCoroutine (LoseLevelRoutine ());
	}

	IEnumerator LoseLevelRoutine ()
	{
		m_isGameOver = true;

		yield return new WaitForSeconds (1.5f);

		if (loseLevelEvent != null)
		{
			loseLevelEvent.Invoke ();
		}

		yield return new WaitForSeconds (2f);

		Debug.Log ("LOSE ------------------------------------------------------------");

		RestartLevel ();
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

	public void UpdateTurn ()
	{
		if (m_currentTurn == Turn.Player && m_player != null)
		{
			if (m_player.IsTurnComplete && !AreEnemiesAllDead ())
			{
				// switch to Turn.Enemy and play enemies
				PlayEnemyTurn ();
			}
		}
		else if (m_currentTurn == Turn.Enemy)
		{
			if (IsEnemyTurnComplete ())
			{
				// if enemy turn is complete, let the player play its turn
				PlayPlayerTurn ();
			}
		}
	}

	bool IsEnemyTurnComplete ()
	{
		foreach (EnemyManager enemy in m_enemies)
		{
			if (enemy.IsDead)
			{
				continue;
			}

			if (enemy != null && !enemy.IsTurnComplete)
			{
				// if any of the enemies hasn't finished its turn, then enemy turn is still
				// running
				return false;
			}
		}

		// all enemies has finished their turns
		return true;
	}

	void PlayPlayerTurn ()
	{
		m_currentTurn = Turn.Player;
		m_player.IsTurnComplete = false;
	}

	void PlayEnemyTurn ()
	{
		m_currentTurn = Turn.Enemy;

		foreach (EnemyManager enemy in m_enemies)
		{
			if (enemy != null && !enemy.IsDead)
			{
				enemy.IsTurnComplete = false;

				// play each enemy's turn
				enemy.PlayTurn ();
			}
		}
	}

	bool AreEnemiesAllDead ()
	{
		foreach (EnemyManager enemy in m_enemies)
		{
			if (!enemy.IsDead)
			{
				return false;
			}
		}

		return true;
	}
}