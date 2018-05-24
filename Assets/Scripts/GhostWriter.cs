using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class GhostWriter : MonoBehaviour {
	public float interval = 0.01f;

	private Text _text;
	private string[] _targetText;
	private bool _isWriting;
	private int _step = 0;
	private bool _block = true;
	private float _lastUpdateTime = 0;

	public void Awake() {
		_text = GetComponent<Text> ();
	}

	public void SetText(string text) {
		_targetText = text.Split(' ');
		_isWriting = true;
		_step = 0;
		_text.text = "";
		_lastUpdateTime = Time.realtimeSinceStartup;
		_block = true;
	}


	private void _UpdateText() {
		if (_step < _targetText.Length) {
			if (_block) {
				_text.text += '\u220E';
				_block = false;
			} else {
				_text.text = _text.text.Remove (_text.text.Length - 1, 1) + _targetText [_step] + ' ';
				_step++;
				_block = true;
			}
		} else {
			_isWriting = false;
		}
	}

	public void Update() {
		if (_isWriting) {
			if (Time.realtimeSinceStartup - _lastUpdateTime > interval) {
				_lastUpdateTime = Time.realtimeSinceStartup;
				_UpdateText ();
			}
		}
	}
}