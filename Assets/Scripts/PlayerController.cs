using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
	public float baseSpeed = 10.0f;
	public float jumpHeight = 20;
	public float gravity = 20;
	private float currentSpeed;
	private CharacterController controller;
	private Vector3 moveDirection = Vector3.zero;
	private void move() {
		Vector3 moveVector = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")).normalized;
		moveVector = moveVector * currentSpeed;
		moveDirection.x = moveVector.x;
		moveDirection.z = moveVector.z;
	}
	private void rotate() {
		transform.rotation = Quaternion.Euler (
			transform.rotation.eulerAngles.x,
			Camera.main.transform.rotation.eulerAngles.y,
			0.0f
		);
	}
	private void jump() {
		if (controller.isGrounded) {
			moveDirection.y = 0;
			if (Input.GetButton ("Jump")) {
				moveDirection.y = jumpHeight;
			}
		} else {
			moveDirection.y -= gravity * Time.deltaTime;
		}
	}
	private void applyMovement() {
		controller.Move (controller.transform.TransformDirection(moveDirection) * Time.deltaTime);
	}
	private void clickSelect() {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			ClearSelection ();

			if (Physics.Raycast(ray, out hit)) {
				if (hit.collider.gameObject.GetComponent<Icon> () != null) {
					SelectObject (hit.collider.gameObject);
				}
			}

		}
	}
	private void SelectObject(GameObject o) {
		o.GetComponent<cakeslice.Outline> ().eraseRenderer = false;
		GameManager.selectedObject = o;
	}
	private void ClearSelection() {
		if (GameManager.selectedObject != null) {
			GameManager.selectedObject.GetComponent<cakeslice.Outline> ().eraseRenderer = true;
			GameManager.selectedObject = null;
		}
	}

	private void testAction() {
		if(Input.GetButtonDown("Test Action")) {
			GetComponent<Deck>().FormatDevice(GameManager.selectedObject);
		}
	}

	void Start () {
		controller = GetComponent<CharacterController> ();
		currentSpeed = baseSpeed;
	}

	void Update () {
		move ();
		jump ();
		rotate ();
		applyMovement ();
		clickSelect ();
		testAction ();
	}
}
