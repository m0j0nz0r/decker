using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingPersona : Deck {
	public int Resonance;
	public int Compiling;
	public int Decompiling;
	public int Registering;
	public int Charisma;

	public override void Start () {
		base.Start ();
		DeviceRating = Resonance;
		Attack = Charisma;
		Sleaze = Intuition;
		DataProcessing = Logic;
		Firewall = Willpower;
	}
}
