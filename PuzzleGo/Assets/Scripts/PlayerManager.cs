using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (PlayerMover))]
[RequireComponent (typeof (PlayerInput))]
[RequireComponent (typeof (PlayerDeath))]
public class PlayerManager : TurnManager
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	public PlayerMover playerMover;
	public PlayerInput playerInput;
	public UnityEvent deathEvent;

	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	// TODO: define properties

	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	Board m_board;

	// ══════════════════════════════════════════════════════════════ METHODS ════
	protected override void Awake ()
	{
		base.Awake ();

		// cache references to PlayerMover and PlayerInput
		playerMover = GetComponent<PlayerMover> ();
		playerInput = GetComponent<PlayerInput> ();
		playerInput.InputEnabled = true;

		m_board = Object.FindObjectOfType<Board> ().GetComponent<Board> ();
	}

	// Update is called once per frame
	void Update ()
	{
		// if the PC is currently moving, ignore player input
		if (playerMover.isMoving || m_gameManager.CurrentTurn != Turn.Player)
		{
			return;
		}

		playerInput.GetKeyInput ();

		// restrict diagonal movement
		if (playerInput.V == 0)
		{
			if (playerInput.H < 0)
			{
				playerMover.MoveLeft ();
			}
			else if (playerInput.H > 0)
			{
				playerMover.MoveRight ();
			}
		}
		else if (playerInput.H == 0)
		{
			if (playerInput.V > 0)
			{
				playerMover.MoveForward ();
			}
			else if (playerInput.V < 0)
			{
				playerMover.MoveBackward ();
			}
		}
	}

	public void Die ()
	{
		if (deathEvent != null)
		{
			deathEvent.Invoke ();
		}
	}

	void CaptureEnemies ()
	{
		if (m_board != null)
		{
			List<EnemyManager> enemies = m_board.FindEnemiesAt (m_board.PlayerNode);

			if (enemies.Count != 0)
			{
				foreach (EnemyManager enemy in enemies)
				{
					if (enemy != null)
					{
						enemy.Die ();
					}
				}
			}
		}
	}

	public override void FinishTurn ()
	{
		CaptureEnemies ();
		base.FinishTurn ();
	}
}