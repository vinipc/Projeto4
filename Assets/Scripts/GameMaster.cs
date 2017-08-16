using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour 
{
	public static bool isCounting = true;

	public Image timerGauge;
	public GameObject restartMessage;
	public float maxTime;

	public Color timerFullColor = Color.green; // Timer color when it's full
	public Color timerEmptyColor = Color.red; // Timer color when it's empty

	private float currentTime;

	private void Awake()
	{
		if (Application.isMobilePlatform)
		{
			SceneManager.LoadScene("Mobile Button");
			return;
		}

		restartMessage.SetActive(false);
		currentTime = maxTime;
		timerGauge.transform.localScale = Vector3.one;
		isCounting = true;
	}

	private void Start()
	{
		if (!Application.isMobilePlatform)
		{
			if (!DanceMatInputManager.isInitialized)
				SceneManager.LoadScene("DanceMat Calibration");
			else if (!MicrophoneActivity.isCalibrated)
				SceneManager.LoadScene("Microphone Calibration");
		}
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
			}
		}

		if (Input.GetButtonDown("Reset"))
		{
			System.GC.Collect();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
