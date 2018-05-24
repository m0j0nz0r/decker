using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Icon : MonoBehaviour {
	public int DataProcessing;
	public int Firewall;

	public Guid Owner;

	public Guid GUID;

	public struct MARK {
		public Guid Owner;
		public int count;
	}

	public List<MARK> marks;


	public virtual int GetAttribute(string attribute) {
		switch (attribute) {
		case "DataProcessing":
			return DataProcessing;
		case "Firewall":
			return Firewall;
		}
		return 0;
	}

	public virtual void SetAttribute(string attribute, int value) {
		switch (attribute) {
		case "DataProcessing":
			DataProcessing = value;
			break;
		case "Firewall":
			Firewall = value;
			break;
		}
	}

	public virtual void Start() {
		GUID = Guid.NewGuid ();
		marks = new List<MARK> ();

		MARK m = new MARK ();

		m.Owner = GUID;
		m.count = 4;

		marks.Add (m);

		Debug.Log (GUID);
	}
}
