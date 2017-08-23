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

	public Transform activitiesParent;
	public Activity[] activitiesPrefabs;

	public GameObject[] tutorials;

	private float currentTime;
	private Activity[] activities;

	private void Awake()
	{
		restartMessage.SetActive(false);
		currentTime = maxTime;
		timerGauge.transform.localScale = Vector3.one;
		isCounting = true;

		activities = new Activity[activitiesPrefabs.Length];
		for (int i = 0; i < activitiesPrefabs.Length; i++)
		{
			activities[i] = Instantiate<Activity>(activitiesPrefabs[i], activitiesParent);
			activities[i].gameObject.SetActive(false);
			tutorials[i].SetActive(false);
		}

		tutorials[0].SetActive(true);
		activities[0].gameObject.SetActive(true);
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

		if (ResourcesMaster.GetResourceAmount(activities[0].generatedResourceName) >= 50f && !activities[1].gameObject.activeSelf)
		{
			activities[1].gameObject.SetActive(true);
			tutorials[1].SetActive(true);
		}

		if (ResourcesMaster.GetResourceAmount(activities[1].generatedResourceName) >= 10f && !activities[2].gameObject.activeSelf)
		{
			activities[2].gameObject.SetActive(true);
			tutorials[2].SetActive(true);
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
		timerGauge.fillAmount = currentTime / maxTime;
		timerGauge.color = Color.Lerp(timerEmptyColor, timerFullColor, currentTime / maxTime);
	}
}
