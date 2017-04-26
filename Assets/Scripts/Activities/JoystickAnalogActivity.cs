using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickAnalogActivity : Activity
{
	private readonly string VERTICAL_INPUT = "JoystickVertical";
	private readonly string HORIZONTAL_INPUT = "JoystickHorizontal";

	public float resourcePerSpin = 1f;

	private enum JoystickPosition { Up, Left, Down, Right, None };
	private List<JoystickPosition> lastPositions = new List<JoystickPosition>();

	private void Awake()
	{
		lastPositions.Add(JoystickPosition.None);
	}

	private void Update()
	{
		if (GameMaster.isCounting)
		{			
			CheckInput();
		}
	}

	private void CheckInput()
	{
		JoystickPosition joystickPosition = GetCurrentJostickPosition();
		JoystickPosition lastPosition = lastPositions.GetLast();

		if (joystickPosition != lastPosition && joystickPosition != JoystickPosition.None)
		{
			lastPositions.Add(joystickPosition);
			if (lastPositions.Count > 4)
			{
				lastPositions.RemoveAt(0);
			}

			if (CounterClockwiseCheck() || ClockwiseCheck())
			{
				GenerateResource(resourcePerSpin);
			}
		}
	}

	private bool CounterClockwiseCheck()
	{
		if (lastPositions.Count < 4)
			return false;

		for (int i = 0; i < 4; i++)
		{
			if ((int) lastPositions[i] != i)
				return false;
		}

		return true;
	}

	private bool ClockwiseCheck()
	{
		if (lastPositions.Count < 4)
			return false;
		
		for (int i = 0; i < 4; i++)
		{
			if ((int) lastPositions[i] != 3 - i)
				return false;
		}
		return true;
	}

	private JoystickPosition GetCurrentJostickPosition()
	{
		float vertical = Input.GetAxis(VERTICAL_INPUT);
		float horizontal = Input.GetAxis(HORIZONTAL_INPUT);

		if (vertical > 0f && vertical > Mathf.Abs(horizontal))
		{
			return JoystickPosition.Up;
		}

		if (vertical < 0f && Mathf.Abs(vertical) > Mathf.Abs(horizontal))
		{
			return JoystickPosition.Down;
		}

		if (horizontal > 0f && horizontal > Mathf.Abs(vertical))
		{
			return JoystickPosition.Right;
		}

		if (horizontal < 0f && Mathf.Abs(horizontal) > Mathf.Abs(vertical))
		{
			return JoystickPosition.Left;
		}

		return JoystickPosition.None;
	}
}