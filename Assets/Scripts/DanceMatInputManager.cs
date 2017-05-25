using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum DanceMatInput { Up, Down, Left, Right, Cross, Square, Triangle, Circle };
public enum InputType { Key, Axis, Undefined };

public class DanceMatInputManager : Singleton<DanceMatInputManager>
{
	public static Dictionary<DanceMatInput, DanceMatInputCode> inputToCode = new Dictionary<DanceMatInput, DanceMatInputCode>();
	public static bool isInitialized = false;

	private static Dictionary<DanceMatInput, bool> getInput = new Dictionary<DanceMatInput, bool>();
	private static Dictionary<DanceMatInput, bool> getInputDown = new Dictionary<DanceMatInput, bool>();
	public static Array danceMatInputsArray = System.Enum.GetValues(typeof(DanceMatInput));

	public static bool GetInputDown(DanceMatInput input)
	{
		return getInputDown[input];
	}

	public static bool GetInput(DanceMatInput input)
	{
		return getInput[input];
	}

	public static bool GetAnyInput()
	{
		for (int i = 0; i < danceMatInputsArray.Length; i++)
		{
			DanceMatInput input = (DanceMatInput) danceMatInputsArray.GetValue(i);
			if (getInput[input])
				return true;
		}

		return false;
	}

	private void Awake()
	{
		if (instance != this)
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		getInput = new Dictionary<DanceMatInput, bool>();
		getInputDown = new Dictionary<DanceMatInput, bool>();

		for (int i = 0; i < danceMatInputsArray.Length; i++)
		{
			DanceMatInput input = (DanceMatInput) danceMatInputsArray.GetValue(i);
			getInput.Add(input, false);
			getInputDown.Add(input, false);
		}

		isInitialized = false;
	}

	private void Update()
	{
		if (!isInitialized)
			return;

		for (int i = 0; i < danceMatInputsArray.Length; i++)
		{
			DanceMatInput input = (DanceMatInput) danceMatInputsArray.GetValue(i);
			bool isInputPressed = IsInputPressed(input);
			getInputDown[input] = getInputDown[input] == false && isInputPressed;
			getInput[input] = isInputPressed;
		}
	}

	private bool IsInputPressed(DanceMatInput input)
	{
		DanceMatInputCode code = inputToCode[input];
		if (code.type == InputType.Axis)
		{
			return Input.GetAxisRaw(code.axisName) == code.axisDirection;
		}
		else if (code.type == InputType.Key)
		{
			return Input.GetKey(code.keycode);
		}

		Debug.Log(string.Concat(input.ToString(), " is undefined."));
		return false;
	}
}

public class DanceMatInputCode
{
	public const KeyCode NULL_KEYCODE = KeyCode.F15;

	public InputType type;
	public KeyCode keycode;
	public string axisName;
	public int axisDirection;

	public DanceMatInputCode()
	{
		type = InputType.Undefined;
		keycode = NULL_KEYCODE;
		axisName = string.Empty;
		axisDirection = 0;
	}

	public override string ToString ()
	{
		if (type == InputType.Key)
			return keycode.ToString();
		else if (type == InputType.Axis)
			return string.Concat(axisName, axisDirection == 1 ? " Positive" : " Negative");
		else
			return "Undefined";
	}
}