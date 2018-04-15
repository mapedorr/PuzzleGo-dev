using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
	// ════ publics ════
	public float rotateSpeed = 20f;
	// ════ properties ════
	// ════ privates ════

	// ════ methods ════
	// Use this for initialization
	void Start ()
	{
		iTween.RotateBy (gameObject, iTween.Hash (
			"y", 360f,
			"looptype", iTween.LoopType.loop,
			"speed", rotateSpeed,
			"easetype", iTween.EaseType.linear
		));
	}
}