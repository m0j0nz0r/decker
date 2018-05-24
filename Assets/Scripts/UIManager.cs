using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
	public GameObject player;
	// Use this for initialization
	void Start () {
		Deck d = player.GetComponent<Deck> ();

		if (d != null) {
			d.InitializeUI ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
