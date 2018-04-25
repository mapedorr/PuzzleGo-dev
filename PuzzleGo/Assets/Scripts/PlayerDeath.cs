using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
	// ══════════════════════════════════════════════════════════════ PUBLICS ════
	public Animator playerAnimController;
	public string playerDeathTrigger = "IsDead";

	// ═══════════════════════════════════════════════════════════ PROPERTIES ════
	// TODO: define properties

	// ═════════════════════════════════════════════════════════════ PRIVATES ════
	// TODO: define prrivates

	// ══════════════════════════════════════════════════════════════ METHODS ════
	public void Die ()
	{
		if (playerAnimController != null)
		{
			playerAnimController.SetTrigger (playerDeathTrigger);
		}
	}
}