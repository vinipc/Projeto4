using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceMatActivity : Activity
{
	public static KeyCode NULL_KEYCODE = KeyCode.F15;
	public static KeyCode UP_KEYCODE, DOWN_KEYCODE, LEFT_KEYCODE, RIGHT_KEYCODE, CROSS_KEYCODE, SQUARE_KEYCODE, TRIANGLE_KEYCODE, CIRCLE_KEYCODE;

	[Header("Activity config:")]
	public int resourcePerTap = 1;

	private KeyCode lastPressedKey;
	private bool leftButtonDown = false;
	private bool rightButtonDown = false;
	private bool upButtonDown = false;
	private bool downButtonDown = false;

	private void Update()
	{
		if (GameMaster.isCounting)
		{
			CheckInput();			
		}
	}

	private void CheckInput()
	{
		// Gets which button was pressed on current frame
		KeyCode pressedButton = GetDanceMatKeyDown();

		// If no button was pressed, do nothing.
		if (pressedButton == NULL_KEYCODE)
			return;

		// If the pressed button is different than the last pressed button, 
		//	increases the score
		if (pressedButton != lastPressedKey)
			GenerateResource(resourcePerTap);

		// Saves current button as last pressed button for next frame.
		lastPressedKey = pressedButton;
	}

	// Returns which mat button was pressed on the current frame.
	private KeyCode GetDanceMatKeyDown()
	{
		if (GetUpButtonDown())
			return KeyCode.UpArrow;

		if (GetDownButtonDown())
			return KeyCode.DownArrow;

		if (GetLeftButtonDown())
			return KeyCode.LeftArrow;

		if (GetRightButtonDown())
			return KeyCode.RightArrow;

		foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
		{
			if(Input.GetKeyDown(key))
			{
				return key;
			}
		}

		return NULL_KEYCODE;
	}

	// True if Up button was pressed on current frame
	private bool GetUpButtonDown()
	{
		bool isUpButtonPressed = Input.GetAxisRaw("Vertical") > 0f;
		bool wasButtonPressedThisFrame = !upButtonDown && isUpButtonPressed;

		upButtonDown = isUpButtonPressed;

		return wasButtonPressedThisFrame;	
	}

	// True if Down button was pressed on current frame
	private bool GetDownButtonDown()
	{
		bool isDownButtonPressed = Input.GetAxisRaw("Vertical") < 0f;
		bool wasButtonPressedThisFrame = !downButtonDown && isDownButtonPressed;

		downButtonDown = isDownButtonPressed;

		return wasButtonPressedThisFrame;
	}

	// True if Left button was pressed on current frame
	private bool GetLeftButtonDown()
	{
		bool isLeftButtonPressed = Input.GetAxisRaw("Horizontal") < 0f;
		bool wasButtonPressedThisFrame = !leftButtonDown && isLeftButtonPressed;

		leftButtonDown = isLeftButtonPressed;

		return wasButtonPressedThisFrame;
	}

	// True if Right button was pressed on current frame
	private bool GetRightButtonDown()
	{
		bool isRightButtonPressed = Input.GetAxisRaw("Horizontal") > 0f;
		bool wasButtonPressedThisFrame = !rightButtonDown && isRightButtonPressed;

		rightButtonDown = isRightButtonPressed;

		return wasButtonPressedThisFrame;
	}
}
