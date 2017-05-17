using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceMatCalibration : MonoBehaviour
{
	public enum DanceMatButtons { Up, Down, Left, Right, Cross, Square, Triangle, Circle };
	public Dictionary<DanceMatButtons, DanceMatInputCode> buttonToCode = new Dictionary<DanceMatButtons, DanceMatInputCode>();

	public Text messageDisplay;

	private DanceMatButtons currentCalibratedButton;
	private DanceMatInputCode pressedCode;
	private int numberOfButtons;

	private float countdown = 3f;

	private void Awake()
	{
		pressedCode = null;
		countdown = 3f;
		currentCalibratedButton = DanceMatButtons.Up;
		numberOfButtons = System.Enum.GetValues(typeof(DanceMatButtons)).Length;
	}

	private void Update()
	{
		if (countdown > 0f)
		{
			messageDisplay.text = string.Concat("Não aperte nenhum botão. Calibragem começará em ", Mathf.CeilToInt(countdown).ToString("0"));
			countdown -= Time.deltaTime;
			if (countdown <= 0f)
			{
				messageDisplay.text = "Aperte a seta para cima";
				StartCoroutine(CalibrateKeys());
			}
		}
		else
		{			
			if ((int) currentCalibratedButton < numberOfButtons)
			{
				pressedCode = GetCurrentDanceMatInputCode();
			}
		}
	}

	private IEnumerator CalibrateKeys()
	{
		while ((int) currentCalibratedButton < numberOfButtons)
		{
			while (pressedCode == null || ButtonHasBeenRegistered())
			{
				yield return new WaitForEndOfFrame();
			}
			
			switch (currentCalibratedButton)
			{
			case DanceMatButtons.Up:
				messageDisplay.text = "Aperte a seta para baixo";
				break;
			case DanceMatButtons.Down:
				messageDisplay.text = "Aperte a seta para esquerda";
				break;
			case DanceMatButtons.Left:
				messageDisplay.text = "Aperte a seta para direita";
				break;
			case DanceMatButtons.Right:
				messageDisplay.text = "Aperte X";
				break;
			case DanceMatButtons.Cross:
				messageDisplay.text = "Aperte quadrado";
				break;
			case DanceMatButtons.Square:
				messageDisplay.text = "Aperte triângulo";
				break;
			case DanceMatButtons.Triangle:
				messageDisplay.text = "Apert círculo";
				break;
			case DanceMatButtons.Circle:
				messageDisplay.text = "Calibragem completa :)";
				break;
			}

			buttonToCode.Add(currentCalibratedButton, pressedCode);
			pressedCode = null;
			currentCalibratedButton = (DanceMatButtons) ((int) currentCalibratedButton + 1);
		}

		DebugDancematKeys();
	}

	private bool ButtonHasBeenRegistered()
	{
		foreach(KeyValuePair<DanceMatButtons, DanceMatInputCode> entry in buttonToCode)
		{
			if (pressedCode.type == DanceMatInputCode.InputType.Key && 
				entry.Value.type == DanceMatInputCode.InputType.Key &&
				entry.Value.keycode == pressedCode.keycode)
				return true;
			if (pressedCode.type == DanceMatInputCode.InputType.Axis && 
				entry.Value.type == DanceMatInputCode.InputType.Axis &&
				entry.Value.axisName == pressedCode.axisName &&
				entry.Value.axisDirection == pressedCode.axisDirection)
				return true;
		}

		return false;
	}

	private DanceMatInputCode GetCurrentDanceMatInputCode()
	{
		DanceMatInputCode inputCode = null;
		KeyCode currentKeycode = GetCurrentKeycode();

		if (currentKeycode != DanceMatInputCode.NULL_KEYCODE)
		{
			inputCode = new DanceMatInputCode();
			inputCode.type = DanceMatInputCode.InputType.Key;
			inputCode.keycode = currentKeycode;
		}
		else
		{
			float horizontalInput = Input.GetAxisRaw("Horizontal");
			float verticalInput = Input.GetAxisRaw("Vertical");

			if (horizontalInput != 0f)
			{
				inputCode = new DanceMatInputCode();
				inputCode.type = DanceMatInputCode.InputType.Axis;
				inputCode.axisName = "Horizontal";
				inputCode.axisDirection = horizontalInput > 0f ? 1 : -1;
			}
			else if (verticalInput != 0f)
			{
				inputCode = new DanceMatInputCode();
				inputCode.type = DanceMatInputCode.InputType.Axis;
				inputCode.axisName = "Vertical";
				inputCode.axisDirection = verticalInput > 0f ? 1 : -1;
			}
		}

		return inputCode;
	}

	private KeyCode GetCurrentKeycode()
	{
		foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
		{
			if(Input.GetKey(key))
			{
				return key;
			}
		}

		return DanceMatActivity.NULL_KEYCODE;
	}

	private void DebugDancematKeys()
	{
		string message = "";
		foreach (KeyValuePair<DanceMatButtons, DanceMatInputCode> entry in buttonToCode)
		{
			message = string.Concat(message, entry.Key.ToString(), ": ", entry.Value.ToString(), "\n");
		}

		Debug.Log(message);
	}
}

public class DanceMatInputCode
{
	public const KeyCode NULL_KEYCODE = KeyCode.F15;

	public enum InputType { Key, Axis, Undefined };
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