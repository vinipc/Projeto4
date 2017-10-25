﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : Singleton<GameMaster> 
{
	public static bool isCounting = true;

	public Image timerGauge;
	public GameObject restartMessage;
	public float maxTime;

	public Color timerFullColor = Color.green; // Timer color when it's full
	public Color timerEmptyColor = Color.red; // Timer color when it's empty

	public Transform activitiesParent;
	public GrapesActivity collectActivity;
	public DanceMatActivity danceMatActivity;
	public MicrophoneActivity bottlingActivity;

	private float currentTime;

	private void Awake()
	{
		restartMessage.SetActive(false);
		currentTime = maxTime;
		timerGauge.transform.localScale = Vector3.one;
		isCounting = true;

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
			bottlingActivity.gameObject.SetActive(true);

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
		timerGauge.fillAmount = currentTime / maxTime;
		timerGauge.color = Color.Lerp(timerEmptyColor, timerFullColor, currentTime / maxTime);
	}
}
