using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ActivitiesProgression { Collecting, Stepping, Bottling }

public class GameMaster : Singleton<GameMaster> 
{
	#region Twitter stuff
	string CONSUMER_KEY = "PfF09E6UP2D8dgWZyMAReCjpt";
	string CONSUMER_SECRET = "MToeBjBAfQum3153YKuyu5xZM9vetbHzkfCyRlbPqBIrn5KbNr";

	string VINICOLA_USER_TOKEN_SECRET = "6JOiinuqz933ArRTs2gs9Q2INOF2nFpRItyB5puvoCLXj";
	string VINICOLA_USER_TOKEN = "933838789381967873-kuSLhGPNWnewK0u4n3yvjHyDpSlOVqG";
	private string VINICOLA_USER_ID = "933838789381967873";
	private string VINICOLA_USER_SCREEN_NAME = "VinicoLouca";

	Twitter.RequestTokenResponse m_RequestTokenResponse;
	Twitter.AccessTokenResponse m_AccessTokenResponse;
	#endregion

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

		LoadTwitterUserInfo();
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

		string wineDescription = string.Concat(ResourcesMaster.instance.bottlesColors.Count, 
			" garrafas de um ", ResourcesMaster.GetColorDescription(),
			", ", ResourcesMaster.GetDeviationDescription(), 
			" e ", ResourcesMaster.GetQuantityDescription(), ".");

		descriptionDisplay.text = string.Concat("Parabéns!\n\nVocês produziram ", wineDescription);
		string tweet = string.Concat("Acabaram de produzir ", wineDescription);

		StartCoroutine(Twitter.API.PostTweet(tweet, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse, new Twitter.PostTweetCallback(this.OnPostTweet)));
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

	private void LoadTwitterUserInfo()
	{
		m_AccessTokenResponse = new Twitter.AccessTokenResponse();

		m_AccessTokenResponse.UserId = VINICOLA_USER_ID;
		m_AccessTokenResponse.ScreenName = VINICOLA_USER_SCREEN_NAME;
		m_AccessTokenResponse.Token = VINICOLA_USER_TOKEN;
		m_AccessTokenResponse.TokenSecret = VINICOLA_USER_TOKEN_SECRET;

		if (!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
			!string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret))
		{
			string log = "LoadTwitterUserInfo - succeeded";
			log += "\n    UserId : " + m_AccessTokenResponse.UserId;
			log += "\n    ScreenName : " + m_AccessTokenResponse.ScreenName;
			log += "\n    Token : " + m_AccessTokenResponse.Token;
			log += "\n    TokenSecret : " + m_AccessTokenResponse.TokenSecret;
			Debug.Log(log);
		}
	}

	private void OnPostTweet(bool success)
	{
		Debug.Log("OnPostTweet - " + (success ? "succedded." : "failed."));
	}
}
