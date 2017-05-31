using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Countdown : MonoBehaviour
{
	private static Transform parent = null;
	private static List<Countdown> countdowns = new List<Countdown>();

	public float time;
	public float totalTime;

	private bool isRepeated;
	private bool isPaused;

	private event Action OnCompleteCallback;
	private event Action OnUpdateCallback;

	public static Countdown New(float time, Action onCompleteCallback, bool repeat, string name = "Countdown")
	{
		return New(time, onCompleteCallback, null, repeat, name);
	}

	public static Countdown New(float time, Action onCompleteCallback, Action onUpdateCallback = null, bool repeat = false, string name = "Countdown")
	{
		GameObject newGameObject = new GameObject(name, typeof(Countdown));
		Countdown newCountdown = newGameObject.GetComponent<Countdown>();
		newCountdown.totalTime = time;
		newCountdown.time = time;
		newCountdown.OnCompleteCallback = onCompleteCallback;
		newCountdown.OnUpdateCallback = onUpdateCallback;
		newCountdown.isRepeated = repeat;

		countdowns.Add(newCountdown);

		if (parent == null)
		{
			parent = new GameObject("Countdowns parent").transform;	
		}

		newGameObject.transform.SetParent(parent);

		return newCountdown;
	}

	public static void PauseAll()
	{
		for (int i = 0; i < countdowns.Count; i++)
		{
			countdowns[i].Pause();
		}
	}

	public static void ResumeAll()
	{
		for (int i = 0; i < countdowns.Count; i++)
		{
			countdowns[i].Resume();
		}
	}

	private void OnDestroy()
	{
		countdowns.Remove(this);
	}

	public void Update()
	{
		if (isPaused)
			return;

		if (OnUpdateCallback != null)
			OnUpdateCallback();

		time -= Time.deltaTime;
		if (time <= 0f)
		{
			if (OnCompleteCallback != null)
				OnCompleteCallback();
			if (isRepeated)
			{
				time = totalTime;				
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	[ContextMenu("Pause")]
	public void Pause()
	{
		isPaused = true;
	}

	[ContextMenu("Resume")]
	public void Resume()
	{
		isPaused = false;
	}
}