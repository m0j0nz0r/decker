using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Window : MonoBehaviour {
	public string Hotkey;

	private void Toggle() {
		CanvasGroup canvasGroup = GetComponent<CanvasGroup> ();
		if (canvasGroup.blocksRaycasts) {
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0f;
		} else {
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1;
		}
	}

	void Update() {
		if (Input.GetButtonDown (Hotkey)) {
			Toggle ();
		}
	}
}
