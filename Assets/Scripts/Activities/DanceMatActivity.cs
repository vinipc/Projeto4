using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class DanceMatActivity : Activity
{
	private float SPAWN_DISTANCE = 5f;
	public string generatedResourceName;

	public Transform grapePrefab;
	public Transform target;
	public float hitRadius;

	public float beatsInterval;
	public float grapesSpeed;

	public SpriteRenderer feedbackPanel;

	[Header("Activity config:")]
	public int resourcePerTap = 1;

	private List<Transform> fallingGrapes = new List<Transform>();
	private DanceMatInput lastPressedInput;
	private Array danceMatInputsArray = System.Enum.GetValues(typeof(DanceMatInput));
	private Countdown beatCountdown;

	private void Start()
	{
		beatCountdown = Countdown.New(beatsInterval, SpawnGrape, UpdateGrapesPosition, true);
	}

	private void Update()
	{
		if (GameMaster.isCounting)
		{
			CheckInput();
		}

		beatCountdown.totalTime = beatsInterval;
	}

	private void UpdateGrapesPosition()
	{		
		for (int i = 0; i < fallingGrapes.Count; i++)
		{
			fallingGrapes[i].transform.position += Vector3.down * grapesSpeed * Time.deltaTime;
		}
	}

	private void SpawnGrape()
	{
		Transform newGrape = Instantiate<Transform>(grapePrefab, target.position + Vector3.up * SPAWN_DISTANCE, Quaternion.identity, transform);
		fallingGrapes.Add(newGrape);
	}

	private void CheckInput()
	{
		// If manager wasn't initialized or no button was pressed, do nothing.
		if (!DanceMatInputManager.isInitialized || !DanceMatInputManager.GetAnyInput())
			return;
		
		// Gets which button was pressed on current frame
		DanceMatInput pressedButton = GetDanceMatKeyDown();

		// If the pressed button is different than the last pressed button, 
		//	increases the score
		if (pressedButton != lastPressedInput)
		{
			Transform closestGrape = GetClosestGrape();
			if (Mathf.Abs(closestGrape.position.y - target.position.y) < hitRadius)
			{
				fallingGrapes.Remove(closestGrape);
				Destroy(closestGrape.gameObject);
				GenerateResource(resourcePerTap);
				ResourcesMaster.AddResource(generatedResourceName, (float) resourcePerTap);
			}
			else
			{
				feedbackPanel.SetAlpha(0f);
				feedbackPanel.DOFade(0.5f, 0.5f).From();
				requiredResource.RemoveResource((int) (requirementToGenerationRatio * resourcePerTap));
				ResourcesMaster.RemoveRequiredResource(generatedResourceName, requirementToGenerationRatio * resourcePerTap);
			}
		}

		// Saves current button as last pressed button for next frame.
		lastPressedInput = pressedButton;
	}

	// Returns which mat button was pressed on the current frame.
	private DanceMatInput GetDanceMatKeyDown()
	{
		for (int i = 0; i < danceMatInputsArray.Length; i++)
		{
			DanceMatInput input = (DanceMatInput) danceMatInputsArray.GetValue(i);
			if (DanceMatInputManager.GetInput(input))
				return input;
		}

		return DanceMatInput.Up;
	}

	private Transform GetClosestGrape()
	{
		Transform closestGrape = fallingGrapes[0];
		for (int i = 0; i < fallingGrapes.Count; i++)
		{
			if (Mathf.Abs(fallingGrapes[i].position.y - target.position.y) < Mathf.Abs(closestGrape.position.y - target.position.y))
			{
				closestGrape = fallingGrapes[i];
			}
		}

		return closestGrape;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(target.position, hitRadius);
	}
}
