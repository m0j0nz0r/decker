using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragHandler : MonoBehaviour, IDragHandler {
	public void OnDrag(PointerEventData eventData) {
		transform.position = Input.mousePosition;
	}
}
