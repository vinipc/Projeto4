using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DanceMatCalibration : MonoBehaviour
{
	public Text messageDisplay;

	private DanceMatInput currentCalibratedButton;
	private DanceMatInputCode pressedCode;
	private int numberOfButtons;

	private float countdown = 3f;

	private void Awake()
	{
		pressedCode = null;
		countdown = 3f;
		currentCalibratedButton = DanceMatInput.Up;
		numberOfButtons = System.Enum.GetValues(typeof(DanceMatInput)).Length;
		DanceMatInputManager.inputToCode.Clear();
	}

	private void Update()
	{
		if (countdown > 0f)
		{
			messageDisplay.text = string.Concat("Não aperte nenhum botão. Calibragem começará em ", Mathf.CeilToInt(countdown).ToString("0"));
			countdown -= Time.deltaTime;
			if (countdown <= 0f)
			{
				messageDisplay.text = "Aperte a seta para cima no tapete de dança";
				StartCoroutine(CalibrateKeys());
			}
		}
		else
		{			
			if ((int) currentCalibratedButton < numberOfButtons)
			{
				pressedCode = GetCurrentDanceMatInputCode();
			}
			else if (Input.anyKeyDown)
			{
				SceneManager.LoadScene("CalibrationChecker");
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
			case DanceMatInput.Up:
				messageDisplay.text = "Agora aperte a seta para baixo";
				break;
			case DanceMatInput.Down:
				messageDisplay.text = "Aperte para esquerda";
				break;
			case DanceMatInput.Left:
				messageDisplay.text = "E para direita";
				break;
			case DanceMatInput.Right:
				messageDisplay.text = "Aperte X";
				break;
			case DanceMatInput.Cross:
				messageDisplay.text = "Aperte quadrado";
				break;
			case DanceMatInput.Square:
				messageDisplay.text = "Aperte triângulo";
				break;
			case DanceMatInput.Triangle:
				messageDisplay.text = "Aperte círculo";
				break;
			case DanceMatInput.Circle:
				messageDisplay.text = "Calibragem completa.\nAperte qualquer botão para começar";
				break;
			}

			DanceMatInputManager.inputToCode.Add(currentCalibratedButton, pressedCode);
			pressedCode = null;
			currentCalibratedButton = (DanceMatInput) ((int) currentCalibratedButton + 1);
		}

		DanceMatInputManager.isInitialized = true;
	}

	private bool ButtonHasBeenRegistered()
	{
		foreach(KeyValuePair<DanceMatInput, DanceMatInputCode> entry in DanceMatInputManager.inputToCode)
		{
			if (pressedCode.type == InputType.Key && 
				entry.Value.type == InputType.Key &&
				entry.Value.keycode == pressedCode.keycode)
				return true;
			if (pressedCode.type == InputType.Axis && 
				entry.Value.type == InputType.Axis &&
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
			inputCode.type = InputType.Key;
			inputCode.keycode = currentKeycode;
		}
		else
		{
			float horizontalInput = Input.GetAxisRaw("Horizontal");
			float verticalInput = Input.GetAxisRaw("Vertical");

			if (horizontalInput != 0f)
			{
				inputCode = new DanceMatInputCode();
				inputCode.type = InputType.Axis;
				inputCode.axisName = "Horizontal";
				inputCode.axisDirection = horizontalInput > 0f ? 1 : -1;
			}
			else if (verticalInput != 0f)
			{
				inputCode = new DanceMatInputCode();
				inputCode.type = InputType.Axis;
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

		return DanceMatInputCode.NULL_KEYCODE;
	}

	private void DebugDancematKeys()
	{
		string message = "";
		foreach (KeyValuePair<DanceMatInput, DanceMatInputCode> entry in DanceMatInputManager.inputToCode)
		{
			message = string.Concat(message, entry.Key.ToString(), ": ", entry.Value.ToString(), "\n");
		}

		Debug.Log(message);
	}
}