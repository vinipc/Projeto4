using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickAnalogActivity : MonoBehaviour
{
	private readonly string VERTICAL_INPUT = "JoystickVertical";
	private readonly string HORIZONTAL_INPUT = "JoystickHorizontal";

	[Header("Activity config:")]
	public int resourcePerSpin = 1;
	public string generatedResource;

	private enum JoystickPosition { Up, Left, Down, Right, None };
	private List<JoystickPosition> latestDirections = new List<JoystickPosition>(); // Keeps the last 4 joystick directions

	private void Awake()
	{
		// Initializes lastPositions
		latestDirections.Add(JoystickPosition.None);
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
		JoystickPosition currentDirection = GetCurrentJostickPosition();
		JoystickPosition lastDirection = latestDirections.GetLast();

		// Waits for direction to change so it doesn't accumulate a bunch of equal directions
		if (currentDirection != lastDirection && currentDirection != JoystickPosition.None)
		{
			latestDirections.Add(currentDirection);

			// We are only interested in the latest 4 directions.
			if (latestDirections.Count > 4)
			{
				latestDirections.RemoveAt(0);
			}

			// Generates resources whenever player completed a full spin
			if (CounterClockwiseCheck() || ClockwiseCheck())
			{
				ResourcesMaster.AddResource(generatedResource, resourcePerSpin);
			}
		}
	}

	// Checks if latest directions completed a counterclockwise spin
	private bool CounterClockwiseCheck()
	{
		if (latestDirections.Count < 4)
			return false;

		for (int i = 0; i < 4; i++)
		{
			if ((int) latestDirections[i] != i)
				return false;
		}

		return true;
	}

	// Checks if latest directions completed a clockwise spin
	private bool ClockwiseCheck()
	{
		if (latestDirections.Count < 4)
			return false;
		
		for (int i = 0; i < 4; i++)
		{
			if ((int) latestDirections[i] != 3 - i)
				return false;
		}

		return true;
	}

	// Simplifies the Vector2 input from the joystick to 4 directions
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