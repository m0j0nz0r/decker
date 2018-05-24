using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persona : Device {
	#region Stats
	public int Intuition;
	public int Logic;
	public int Willpower;
	public int Reaction;
	public int Edge;

	public int Computer;
	public int Cybercombat;
	public int ElectronicWarfare;
	public int Hacking;
	public int Hardware;
	public int Software;
	#endregion

	#region Actions
	public void JackOut() {
	}
	public void SendMessage() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		
	}
	public void SwitchInterfaceMode() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		
	}
	public void EditFile() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		
	}
	public void MatrixPerception() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		
	}
	public void EnterHost(){
		if (isBusy)
			throw new DeviceException ("Device is busy");
		
	}
	public void ExitHost() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		
	}
	public void GridHop() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		
	}
	public void JumpIntoRiggedDevice() { // TODO: Check device needed for this, move this as appropiate.
		if (isBusy)
			throw new DeviceException ("Device is busy");
		
	} 
	public void ControlDevice() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
	}
	public void MatrixSearch() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
	}

	#endregion

	public void FixFormattedDeviceStep(Device d){
		GameManager.RollResult roll = GameManager.instance.Roll (GetAttribute ("Software") + GetAttribute ("Logic"), GetLimit("Mental"), false);
		d.FormatStep (roll.successes);
	}

	public int GetLimit(string limit) {
		switch (limit) {
		case "Mental": 
			return Mathf.CeilToInt ((GetAttribute ("Logic") * 2 + GetAttribute ("Intuition") + GetAttribute ("Willpower")) / 3);
		default:
			return 0;
		}
	}
	public override int GetAttribute (string attribute)	{
		switch (attribute) {
		case "Intuition":
			return Intuition;
		case "Logic":
			return Logic;
		case "Willpower":
			return Willpower;
		case "Reaction":
			return Reaction;
		case "Edge":
			return Edge;
		case "Computer":
			return Computer;
		case "Cybercombat":
			return Cybercombat;
		case "ElectronicWarfare":
			return ElectronicWarfare;
		case "Hacking":
			return Hacking;
		case "Hardware":
			return Hardware;
		case "Software":
			return Software;
		default:
			return base.GetAttribute(attribute);
		}
	}
	public override void SetAttribute (string attribute, int value)	{
		switch (attribute) {
		case "Intuition":
			Intuition = value;
			break;
		case "Logic":
			Logic = value;
			break;
		case "Willpower":
			Willpower = value;
			break;
		case "Reaction":
			Reaction = value;
			break;
		case "Edge":
			Edge = value;
			break;
		case "Computer":
			Computer = value;
			break;
		case "Cybercombat":
			Cybercombat = value;
			break;
		case "ElectronicWarfare":
			ElectronicWarfare = value;
			break;
		case "Hacking":
			Hacking = value;
			break;
		case "Hardware":
			Hardware = value;
			break;
		case "Software":
			Software = value;
			break;
		default:
			base.SetAttribute (attribute, value);
			break;
		}
	}
}
