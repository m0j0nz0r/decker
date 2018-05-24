using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Device : Icon {

	#region Stats
	public int DeviceRating;
	public int ConditionMonitor;
	#endregion

	#region Actions
	public void ChangeIcon() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
	}
	public void InviteMark(){
		if (isBusy)
			throw new DeviceException ("Device is busy");
	}
	private int _formatProgress = 0;
	public void FormatSelf() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
		_formatProgress = 12;
	}
	public void FormatStep(int successes) {
		_formatProgress = Mathf.Clamp (_formatProgress - successes, 0, _formatProgress);
	}

	public void RebootSelf() {
		if (isBusy)
			throw new DeviceException ("Device is busy");
	}
	#endregion

	public virtual bool isBusy {
		get {
			return _formatProgress > 0;
		}
	}
	public override int GetAttribute (string attribute)
	{
		switch (attribute) {
		case "DeviceRating":
			return DeviceRating;
		case "ConditionMonitor":
			return ConditionMonitor;
		default:
			return base.GetAttribute (attribute);
		}
	}
	public override void SetAttribute (string attribute, int value)
	{
		switch (attribute) {
		case "DeviceRating":
			DeviceRating = value;
			break;
		case "ConditionMonitor":
			ConditionMonitor = value;
			break;
		default:
			base.SetAttribute (attribute, value);
			break;
		}
	}

	#region Exceptions
	public class DeviceException: Exception {
		public DeviceException(){}
		public DeviceException(string message) : base(message) {}
		public DeviceException(string message, Exception inner) : base(message, inner) {}
	}
	#endregion
}
