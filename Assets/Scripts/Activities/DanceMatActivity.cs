using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DanceMatActivity : Activity
{
	[Header("Activity config:")]
	public int resourcePerTap = 1;

	private DanceMatInput lastPressedInput;
	private Array danceMatInputsArray = System.Enum.GetValues(typeof(DanceMatInput));

	private void Update()
	{
		if (GameMaster.isCounting)
		{
			CheckInput();			
		}
	}

	private void CheckInput()
	{
		// If no button was pressed, do nothing.
		if (!DanceMatInputManager.GetAnyInput())
			return;
		
		// Gets which button was pressed on current frame
		DanceMatInput pressedButton = GetDanceMatKeyDown();

		// If the pressed button is different than the last pressed button, 
		//	increases the score
		if (pressedButton != lastPressedInput)
			GenerateResource(resourcePerTap);

		// Saves current button as last pressed button for next frame.
		lastPressedInput = pressedButton;
	}

	// Returns which mat button was pressed on the current frame.
	private DanceMatInput GetDanceMatKeyDown()
	{
		for (int i = 0; i < danceMatInputsArray.Length; i++)
		{
			DanceMatInput input = (DanceMatInput) danceMatInputsArray.GetValue(i);
			if (DanceMatInputManager.GetInput(input))
				return input;
		}

		return DanceMatInput.Up;
	}
}
