using System;
using UnityEngine;
using System.Collections.Generic;

public class GamepadPlayerController : PlayerController
{
	public int index;
	public Gamepad gamepad = null;

	public GamepadPlayerController(int index)
	{
		this.index = index;
		title = "Gamepad " + (index+1);

		gamepad = GamepadManager.instance.GetGamepad(index);
	}

	override public void Update()
	{
		gamepad = GamepadManager.instance.GetGamepad(index);

		if(gamepad == null)
		{
			movementVector.x = 0;
			movementVector.y = 0;
		}
		else
		{
			movementVector = gamepad.direction;
		}
	}

	override public bool GetButtonDown(PlayerControllerButtonType buttonType)
	{
		if(gamepad == null) return false;

		if(buttonType == PlayerControllerButtonType.Ready)
		{
			if(gamepad.GetButtonDown(gamepad.buttonReady))
			{
				return true;
			}
		}
		else if(buttonType == PlayerControllerButtonType.Start || buttonType == PlayerControllerButtonType.Pause)
		{
			if(gamepad.GetButtonDown(gamepad.buttonStart))
			{
				return true;
			}
		}
		else if(buttonType == PlayerControllerButtonType.Reset)
		{
			if(gamepad.GetButtonDown(gamepad.buttonReset))
			{
				return true;
			}
		}

		return false;
	}

	override public bool CanBeUsed()
	{
		gamepad = GamepadManager.instance.GetGamepad(index);

		if(gamepad == null)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}