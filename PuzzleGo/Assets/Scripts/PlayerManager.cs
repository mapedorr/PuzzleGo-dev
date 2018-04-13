using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerMover))]
[RequireComponent (typeof (PlayerInput))]
public class PlayerManager : MonoBehaviour
{
	// ════ publics ════
	public PlayerMover playerMover;
	public PlayerInput playerInput;

	void Awake ()
	{
		playerMover = GetComponent<PlayerMover> ();
		playerInput = GetComponent<PlayerInput> ();
		playerInput.InputEnabled = true;
	}

	// Update is called once per frame
	void Update ()
	{
		if (playerMover.isMoving)
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
}