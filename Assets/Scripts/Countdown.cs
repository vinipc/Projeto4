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

	public event Action Callback;

	/// <summary>
	/// Creates a new countdown with the specified time, callback and name that destroys itself when finished
	/// </summary>
	public static Countdown New(float time, Action callback, string name = "Countdown")
	{
		return New(time, callback, false, name);
	}

	/// <summary>
	/// Creates a new countdown with the specified time, callback and name.
	/// </summary>
	/// <param name="repeat">If set to <c>true</c> repeats indefinitely. Else, destroys itself when finished</param>
	public static Countdown New(float time, Action callback, bool repeat = false, string name = "Countdown")
	{
		GameObject newGameObject = new GameObject(name, typeof(Countdown));
		Countdown newCountdown = newGameObject.GetComponent<Countdown>();
		newCountdown.totalTime = time;
		newCountdown.time = time;
		newCountdown.Callback = callback;
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
		
		time -= Time.deltaTime;
		if (time <= 0f)
		{
			Callback();
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