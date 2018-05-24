using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : Persona {
	#region Stats
	public int Attack;
	public int Sleaze;

	public int Programs;
	#endregion


	#region Actions
	public void LoadProgram (Program p) {
		if (isBusy)
			throw new DeviceException ("Device is busy");

		Program[] programs = GetComponents<Program> ();

		int loadedProgramCount = 0;

		foreach (Program pr in programs) {
			if (pr.isLoaded) loadedProgramCount++;
		}

		if (loadedProgramCount < Programs)
			p.isLoaded = true;
	}

	public void SwapPrograms(Program p1, Program p2) {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		if (p1.isLoaded != p2.isLoaded) {
			p1.isLoaded = !p1.isLoaded;
			p2.isLoaded = !p2.isLoaded;
		}
	}

	public void UnloadProgram(Program p) {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		p.isLoaded = false;
	}

	public void SwitchAttributes(string a1, string a2) {
		if (isBusy)
			throw new DeviceException ("Device is busy");

		int a1Value, a2Value;

		a1Value = GetAttribute (a1);
		a2Value = GetAttribute (a2);

		SetAttribute (a1, a2Value);
		SetAttribute (a2, a1Value);
	}

	private int _overwatchScore = 0;

	private bool CheckMarks(Icon target, int needed) {
		if (target.marks.Count == 0) {
			return false;
		}

		foreach (MARK m in target.marks) {
			if (m.Owner == GUID) {
				if (m.count < needed) {
					return false;
				} else {
					return true;
				}
			} 
		}

		return false;
	}
	public int CheckOverwatchScore() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

		GameManager.RollResult roll = GameManager.instance.Roll (GetAttribute ("Electronic Warfare") + GetAttribute ("Logic"), GetAttribute ("Sleaze"), false);
		GameManager.RollResult threshold = GameManager.instance.Roll (6, 6, false);

		if (roll.successes > threshold.successes)
			return _overwatchScore;
		else
			return 0;
	}

	public void FormatDevice(GameObject d) {
		if (isBusy)
			throw new DeviceException ("Device is busy");

		if (d == null) {
			return;
		}

		Device target = d.GetComponent<Device> ();

		if (target == null) {
			return;
		}

		if (!CheckMarks (target, 3))
			return;

		int pool = GetAttribute ("Computer") + GetAttribute ("Logic");
		GameManager.RollResult attack = GameManager.instance.Roll (pool, GetAttribute("Sleaze"), false);
		Debug.LogFormat ("Attack pool: {0}\tSuccesses: {1}", pool, attack.successes);

		pool = target.GetAttribute ("Firewall") + target.GetAttribute("Willpower");
		GameManager.RollResult defense = GameManager.instance.Roll (pool, pool, false);
		Debug.LogFormat ("Defense pool: {0}\tSuccesses: {1}", pool, defense.successes);

		_overwatchScore += defense.successes;

		if (attack.successes > defense.successes)
			target.FormatSelf ();
		else
			Debug.Log ("Format Device failed!");
	}

	public void RebootDevice() {
	}
	public void SpoofCommand() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void Garbage() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void CrackFile() {
		if (isBusy)
			throw new DeviceException ("Device is busy");


	}
	public void DisarmDataBomb() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void SetDataBomb() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void Snoop() {
		if (isBusy)
			throw new DeviceException ("Device is busy");


	}
	public void TraceIcon() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void BruteForceAttack() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void BruteForceGridHop() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void EraseMark() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void HackOnTheFly() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void HackOnTheFlyGridHop() {
		if (isBusy)
			throw new DeviceException ("Device is busy");


	}
	public void CrashProgram() {
		if (isBusy)
			throw new DeviceException ("Device is busy");


	}
	public void DataSpike() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void Hide() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void JamSignals() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}
	public void TrackBack() {
		if (isBusy)
			throw new DeviceException ("Device is busy");

	}

	#endregion

	public override int GetAttribute(string attribute) {
		switch (attribute) {
		case "Attack":
			return Attack;
		case "Sleaze":
			return Sleaze;
		default:
			return base.GetAttribute (attribute);
		}
	}

	public override void SetAttribute (string attribute, int value)
	{
		switch (attribute) {
		case "Attack":
			Attack = value;
			break;
		case "Sleaze":
			Sleaze = value;
			break;
		default:
			base.SetAttribute (attribute, value);
			break;
		}
	}


	#region UI
	public Toggle ProgramListItemPrefab;
	#endregion

	#region Initializers
	public override void Start() {
		base.Start ();
		InitializeUI ();
	}

	public void InitializeUI() {
		InitializeProgramListUI ();
	}

	void InitializeProgramListUI() {
		Program [] programs = GetComponents<Program> ();
		GameObject list = GameObject.Find ("ProgramListGrid");

		if (list == null)
			return;

		foreach (Program p in programs) {
			InstantiateProgramListItem (p, list.transform);
		}
	}
	#endregion

	#region Helpers
	Toggle InstantiateProgramListItem(Program p, Transform parent) {
		Toggle t = Instantiate<Toggle>(ProgramListItemPrefab, parent);

		t.isOn = p.isLoaded;

		t.GetComponentInChildren<Text> ().GetComponent<GhostWriter>().SetText(p.ProgramName);

		t.onValueChanged.AddListener (delegate {
			if (t.isOn)
				LoadProgram(p);
			else
				UnloadProgram(p);
			
			t.isOn = p.isLoaded;	
		});
		return t;
	}

	#endregion
}
