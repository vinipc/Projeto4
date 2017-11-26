using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ActivitiesProgression { Collecting, Stepping, Bottling }

public class GameMaster : Singleton<GameMaster> 
{
	public static string MAIN_MENU_NAME = "Main Menu";
	public static bool isCounting = true;
	public static ActivitiesProgression currentProgressionState;

	public Image timerGauge;
	public GameObject gameOverOverlay;
	public Text descriptionDisplay;
	public float maxTime;

	public Color timerFullColor = Color.green; // Timer color when it's full
	public Color timerEmptyColor = Color.red; // Timer color when it's empty

	public Transform activitiesParent;
	public GrapesActivity collectActivity;
	public DanceMatActivity danceMatActivity;
	public MicrophoneActivity bottlingActivity;
	public GameObject pauseMenu;

	private float currentTime;
	private bool isPaused = false;

	private void Awake()
	{
		gameOverOverlay.SetActive(false);
		currentTime = maxTime;
		timerGauge.transform.localScale = Vector3.one;
		isCounting = true;
		currentProgressionState = ActivitiesProgression.Collecting;

		danceMatActivity.gameObject.SetActive(false);
		bottlingActivity.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (isCounting)
		{
			if (currentTime < 0f)
				GameOver();
			else
				DecreaseTimer();
		}

		if (!bottlingActivity.gameObject.activeSelf && ResourcesMaster.GetResourceAmount("juice") >= ResourcesMaster.GetResourceData("bottle").requiredToGeneratedRatio)
		{
			bottlingActivity.gameObject.SetActive(true);
			currentProgressionState = ActivitiesProgression.Bottling;
		}

		if (Input.GetButtonDown("Reset"))
		{
			System.GC.Collect();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		else if (Input.GetButtonDown("Pause"))
		{
			if (isPaused)
				ClosePauseMenu();
			else
				OpenPauseMenu();
		}
	}

	private void OpenPauseMenu()
	{
		isPaused = true;
		isCounting = false;

		pauseMenu.SetActive(true);
	}

	public void ClosePauseMenu()
	{
		Debug.Log("Close pause menu");
		isPaused = false;
		isCounting = true;

		pauseMenu.SetActive(false);
	}

	public void MenuButtonPressed()
	{
		Debug.Log("Load menu");
		SceneManager.LoadScene(MAIN_MENU_NAME);
	}

	private void GameOver()
	{
		isCounting = false;
		gameOverOverlay.SetActive(true);

		string wineDescription = "Parabéns!\n\n Vocês produziram ";
		wineDescription = string.Concat(wineDescription, ResourcesMaster.instance.bottlesColors.Count, 
			" garrafas de um ", ResourcesMaster.GetColorDescription(),
			", ", ResourcesMaster.GetDeviationDescription(), 
			" e ", ResourcesMaster.GetQuantityDescription(), ".");

		descriptionDisplay.text = wineDescription;
	}

	private void DecreaseTimer()
	{
		currentTime -= Time.deltaTime;
		timerGauge.fillAmount = currentTime / maxTime;
		timerGauge.color = Color.Lerp(timerEmptyColor, timerFullColor, currentTime / maxTime);
	}

	public static void FirstGrapeCollected()
	{
		if (currentProgressionState == ActivitiesProgression.Collecting)
		{
			currentProgressionState = ActivitiesProgression.Stepping;
		}
	}
}
