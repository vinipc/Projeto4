using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyInputReceiver : MonoBehaviour 
{
	public KeyCode input;
	public float generatedAmount;

	private Activity activity;

	private void Awake()
	{
		activity = GetComponent<Activity>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(input))
		{
			activity.GenerateResource(generatedAmount);
		}
	}
}