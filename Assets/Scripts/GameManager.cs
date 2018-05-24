using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;
	public static GameObject selectedObject = null;

	public struct RollResult {
		public int successes;
		public int glitch;

		public RollResult(int _successes, int _glitch) {
			successes = _successes;
			glitch = _glitch;
		}
	}
	public RollResult Roll(int pool, int limit, bool explode) {
		RollResult returnValue = new RollResult(0, 0);
		int rollResult;

		while (pool > 0) {
			pool--;
			rollResult = Random.Range (1, 7);

			if (rollResult > 4)
				returnValue.successes++;
			else if (rollResult == 1)
				returnValue.glitch++;

			if (explode && rollResult == 6)
				pool++;
		}

		if (!explode) {
			returnValue.successes = Mathf.Clamp (returnValue.successes, 0, limit);
		}

		return returnValue;
	}

	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}
}
