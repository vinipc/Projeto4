using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceMatCalibration : MonoBehaviour
{
	private enum DanceMatButtons { Up, Down, Left, Right, Cross, Square, Triangle, Circle };

	public Text messageDisplay;

	private DanceMatButtons currentCalibratedButton;
	private KeyCode pressedKey = DanceMatActivity.NULL_KEYCODE;
	private int numberOfButtons;

	private float countdown = 3f;

	private void Awake()
	{
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
				pressedKey = GetCurrentKeycode();
			}
		}
	}

	private IEnumerator CalibrateKeys()
	{
		while ((int) currentCalibratedButton < numberOfButtons)
		{
			while (pressedKey == DanceMatActivity.NULL_KEYCODE || ButtonHasBeenRegistered())
			{
				yield return new WaitForEndOfFrame();
			}
			
			switch (currentCalibratedButton)
			{
			case DanceMatButtons.Up:
				DanceMatActivity.UP_KEYCODE = pressedKey;
				messageDisplay.text = "Aperte a seta para baixo";
				break;
			case DanceMatButtons.Down:
				DanceMatActivity.DOWN_KEYCODE = pressedKey;
				messageDisplay.text = "Aperte a seta para esquerda";
				break;
			case DanceMatButtons.Left:
				DanceMatActivity.LEFT_KEYCODE = pressedKey;
				messageDisplay.text = "Aperte a seta para direita";
				break;
			case DanceMatButtons.Right:
				DanceMatActivity.RIGHT_KEYCODE = pressedKey;
				messageDisplay.text = "Aperte X";
				break;
			case DanceMatButtons.Cross:
				DanceMatActivity.CROSS_KEYCODE = pressedKey;
				messageDisplay.text = "Aperte quadrado";
				break;
			case DanceMatButtons.Square:
				DanceMatActivity.SQUARE_KEYCODE = pressedKey;
				messageDisplay.text = "Aperte triângulo";
				break;
			case DanceMatButtons.Triangle:
				DanceMatActivity.TRIANGLE_KEYCODE = pressedKey;
				messageDisplay.text = "Apert círculo";
				break;
			case DanceMatButtons.Circle:
				DanceMatActivity.CIRCLE_KEYCODE = pressedKey;
				messageDisplay.text = "Calibragem completa :)";
				break;
			}

			pressedKey = DanceMatActivity.NULL_KEYCODE;
			currentCalibratedButton = (DanceMatButtons) ((int) currentCalibratedButton + 1);			
		}

		DebugDancematKeys();
	}

	private bool ButtonHasBeenRegistered()
	{
		if (DanceMatActivity.UP_KEYCODE == pressedKey ||
			DanceMatActivity.DOWN_KEYCODE == pressedKey ||
			DanceMatActivity.LEFT_KEYCODE == pressedKey ||
			DanceMatActivity.RIGHT_KEYCODE == pressedKey ||
			DanceMatActivity.CROSS_KEYCODE == pressedKey ||
			DanceMatActivity.SQUARE_KEYCODE == pressedKey ||
			DanceMatActivity.TRIANGLE_KEYCODE == pressedKey ||
			DanceMatActivity.CIRCLE_KEYCODE == pressedKey)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private KeyCode GetCurrentKeycode()
	{
		foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
		{
			if(Input.GetKeyDown(key))
			{
				return key;
			}
		}

		return DanceMatActivity.NULL_KEYCODE;
	}

	private void DebugDancematKeys()
	{
		string message = string.Concat("Up: ", DanceMatActivity.UP_KEYCODE, "\nDown: ", DanceMatActivity.DOWN_KEYCODE, "\nLeft: ", DanceMatActivity.LEFT_KEYCODE, "\nRight: ", DanceMatActivity.RIGHT_KEYCODE);
		message = string.Concat(message, "\nCross: ", DanceMatActivity.CROSS_KEYCODE, "\nSquare: ", DanceMatActivity.SQUARE_KEYCODE, "\nTriangle: ", DanceMatActivity.TRIANGLE_KEYCODE, "\nCircle: ", DanceMatActivity.CIRCLE_KEYCODE);
		Debug.Log(message);
	}
}
