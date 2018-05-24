using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ThirdPersonCamera : MonoBehaviour {
	public Transform lookAt;

	private float distance = 10.0f;
	private float currentX = 0.0f;
	private float currentY = 4.0f;
	public float sensivityX = 4.0f;
	public float sensivityY = 2.0f;
	// Use this for initialization
	void Start () {
		// Cursor.visible = false;
		// Cursor.lockState = CursorLockMode.Confined;
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetButtonDown ("Fire2") && Cursor.visible) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Input.GetButtonUp ("Fire2") && !Cursor.visible) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		if (Input.GetButton ("Fire2")) {
			currentX += Input.GetAxis ("Mouse X") * sensivityX;
			currentY -= Input.GetAxis ("Mouse Y") * sensivityY;
		}

		distance = Mathf.Clamp (distance + Input.GetAxis ("Mouse ScrollWheel"), 0.0f, 20.0f);
	}
	void LateUpdate () {
		Vector3 dir = new Vector3 (0, 0, -distance);
		Quaternion rotation = Quaternion.Euler (currentY, currentX, 0);
		transform.position = lookAt.position + rotation * dir;
		transform.LookAt (lookAt);
	}
}
