using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour 
{
	public const int SCORE_PER_TAP = 1;
	public const KeyCode NULL_KEYCODE = KeyCode.F15;

	public Text scoreDisplay;
	public Image timerGauge;
	public GameObject restartMessage;
	public float maxTime;

	public Color timerFullColor = Color.green; // Timer color when it's full
	public Color timerEmptyColor = Color.red; // Timer color when it's empty

	private float currentTime;

	public KeyCode lastPressedKey;

	private int currentScore = 0;
	public static bool isCounting = true;

	private bool leftButtonDown = false;
	private bool rightButtonDown = false;
	private bool upButtonDown = false;
	private bool downButtonDown = false;

	private void Awake()
	{
		restartMessage.SetActive(false);
		lastPressedKey = NULL_KEYCODE;
		currentTime = maxTime;
		currentScore = 0;
		scoreDisplay.text = currentScore.ToString();
		timerGauge.transform.localScale = Vector3.one;
		isCounting = true;
	}

	private void Update()
	{
		if (isCounting)
		{
			if (currentTime < 0f)
			{
				GameOver();
			}
			else
			{
				DecreaseTimer();
				CheckInput();
			}
		}
		else
		{
			if (Input.GetButtonDown("Reset"))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
	}

	// Incresed current score and updates display
	private void AddScore(int score)
	{
		GameObject[] grapesArray = GameObject.FindGameObjectsWithTag ("Grape");

		if (grapesArray.Length < 3)
			return;
		
		currentScore += score;
		scoreDisplay.text = currentScore.ToString();

		for (int i = 0; i < 3; i++)
		{
			Destroy (grapesArray [i]);
		}
	}

	private void GameOver()
	{
		isCounting = false;
		restartMessage.SetActive(true);
	}

	private void DecreaseTimer()
	{
		currentTime -= Time.deltaTime;

		Vector3 newTimerScale = timerGauge.transform.localScale;
		newTimerScale.x = currentTime / maxTime;
		timerGauge.transform.localScale = newTimerScale;

		timerGauge.color = Color.Lerp(timerEmptyColor, timerFullColor, currentTime / maxTime);
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
			AddScore(SCORE_PER_TAP);

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

		foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
		{
			if(Input.GetKeyDown(vKey))
			{
				return vKey;
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
