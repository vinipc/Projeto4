using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

[System.Serializable]
public struct DanceMatActivityProperties
{
	public float resourcesPerTap;
	public float hitRadius;
	public float beatsInterval;
	public float grapesSpeed;
}

public class DanceMatActivity : Activity
{
	public Transform spawnPoint;
	public Transform grapePrefab;
	public Transform target;
	public SpriteRenderer feedbackPanel;
	public Collider2D grapesKiller;
	public Transform footTransform;
	public AudioClip grapeSquashing;

	private List<Transform> fallingGrapes = new List<Transform>();
	private DanceMatInput lastPressedInput;
	private Array danceMatInputsArray = System.Enum.GetValues(typeof(DanceMatInput));
	private Countdown beatCountdown;
	private Vector3 startingFootPosition;
	private Vector3 startingFootRotation;
	private AudioSource audioSource;

	protected override void Awake()
	{
		base.Awake();
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		float beatsInterval = ResourcesMaster.instance.danceMatProperties.beatsInterval;
		beatCountdown = Countdown.New(beatsInterval, SpawnGrape, UpdateGrapesPosition, true);

		startingFootPosition = footTransform.position;
		startingFootRotation = footTransform.eulerAngles;
	}

	private void Update()
	{
		if (GameMaster.isCounting)
		{
			CheckInput();
		}

		float beatsInterval = ResourcesMaster.instance.danceMatProperties.beatsInterval;
		beatCountdown.totalTime = beatsInterval;
	}

	private void UpdateGrapesPosition()
	{
		List<GameObject> flaggedGrapes = new List<GameObject>();
		for (int i = 0; i < fallingGrapes.Count; i++)
		{
			float grapesSpeed = ResourcesMaster.instance.danceMatProperties.grapesSpeed;
			fallingGrapes[i].transform.position += (target.position - spawnPoint.position).normalized * grapesSpeed * Time.deltaTime;
			if (grapesKiller.bounds.Contains(fallingGrapes[i].transform.position))
			{
				flaggedGrapes.Add(fallingGrapes[i].gameObject);
			}
		}

		for (int i = 0; i < flaggedGrapes.Count; i++)
		{
			fallingGrapes.Remove(flaggedGrapes[i].transform);
			Destroy(flaggedGrapes[i]);
		}
	}

	private void SpawnGrape()
	{
		float requiredAmount = ResourcesMaster.instance.danceMatProperties.resourcesPerTap * generatedResource.requiredToGeneratedRatio;
		if (ResourcesMaster.GetResourceAmount(requiredResource.uniqueName) < requiredAmount)
			return;

		Transform newGrape = Instantiate<Transform>(grapePrefab, spawnPoint.position, Quaternion.identity);
		newGrape.SetParent(spawnPoint, true);
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
			DoFootStep();
			Transform closestGrape = GetClosestGrape();
			if (closestGrape)
			{
				float hitRadius = ResourcesMaster.instance.danceMatProperties.hitRadius;
				if ((closestGrape.position - target.position).sqrMagnitude < hitRadius * hitRadius)
				{
					audioSource.PlayOneShot(grapeSquashing);
					fallingGrapes.Remove(closestGrape);
					Destroy(closestGrape.gameObject);
					float resourcePerTap = ResourcesMaster.instance.danceMatProperties.resourcesPerTap;
					ResourcesMaster.AddResource(generatedResourceName, resourcePerTap);
				}
				else
				{
					feedbackPanel.SetAlpha(0f);
					feedbackPanel.DOFade(0.5f, 0.5f).From();
					float resourcePerTap = ResourcesMaster.instance.danceMatProperties.resourcesPerTap;
					float usedResources = ResourcesMaster.GetResourceData(generatedResourceName).requiredToGeneratedRatio * resourcePerTap;
					ResourcesMaster.RemoveRequiredResource(generatedResourceName, usedResources);
				}				
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
		if (fallingGrapes.Count == 0)
			return null;

		Transform closestGrape = fallingGrapes[0];
		for (int i = 0; i < fallingGrapes.Count; i++)
		{
			if ((fallingGrapes[i].position - target.position).sqrMagnitude < (closestGrape.position - target.position).sqrMagnitude)
			{
				closestGrape = fallingGrapes[i];
			}
		}

		return closestGrape;
	}

	private void DoFootStep()
	{
		DOTween.Kill("FootStep");

		footTransform.position = startingFootPosition;
		footTransform.rotation = Quaternion.Euler(startingFootRotation + new Vector3(0f, 0f, UnityEngine.Random.Range(-8f, 8f)));

		Sequence sequence = DOTween.Sequence();

		sequence.Append(footTransform.DOMove(target.position, 0.1f));
		sequence.Append(footTransform.DOMove(startingFootPosition, 1f));
		sequence.SetId("FootStep");
	}

	private void OnDrawGizmosSelected()
	{
		if (ResourcesMaster.instance != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(target.position, ResourcesMaster.instance.danceMatProperties.hitRadius);			
		}
	}
}