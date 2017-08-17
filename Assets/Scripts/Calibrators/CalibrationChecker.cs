using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalibrationChecker : MonoBehaviour
{
	private void Start()
	{
		if (Application.isMobilePlatform)
		{
			SceneManager.LoadScene("Mobile Button");
		}
		else if (!Application.isMobilePlatform)
		{
			if (!DanceMatInputManager.isInitialized)
				SceneManager.LoadScene("DanceMat Calibration");
			else if (!MicrophoneActivity.isCalibrated)
				SceneManager.LoadScene("Microphone Calibration");
			else
				SceneManager.LoadScene("Main");
		}
	}
}
