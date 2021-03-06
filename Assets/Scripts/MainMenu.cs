﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public const string CALIBRATION_SCENE_NAME = "CalibrationChecker";
	public const string CREDITS_SCENE_NAME = "Credits";

	public void PlayButtonPressed()
	{
		SceneManager.LoadScene(CALIBRATION_SCENE_NAME);
	}

	public void CalibrateButtonPressed()
	{
		DanceMatInputManager.isInitialized = false;
		MicrophoneActivity.isCalibrated = false;
		SceneManager.LoadScene(CALIBRATION_SCENE_NAME);
	}

	public void CreditsButtonPressed()
	{
		SceneManager.LoadScene(CREDITS_SCENE_NAME);
	}

	public void QuitButtonPressed()
	{
		Application.Quit();
	}
}
