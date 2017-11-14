using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

[System.Serializable]
public class DanceMatActivity : Activity
{
	public Transform spawnPoint;
	public Grape grapePrefab;
	public Transform target;
	public SpriteRenderer feedbackPanel;
	public Collider2D grapesKiller;
	public Transform footTransform;
	public AudioClip grapeSquashing;

	private List<Grape> fallingGrapes = new List<Grape>();
	private List<float> steppedGrapesColors = new List<float>();
	private DanceMatInput lastPressedInput;
	private Array danceMatInputsArray = System.Enum.GetValues(typeof(DanceMatInput));
	private Countdown beatCountdown;
	private Vector3 startingFootPosition;
	private Vector3 startingFootRotation;
	private AudioSource audioSource;
	private List<float> grapeColorsPool = new List<float>();

	protected override void Awake()
	{
		base.Awake();
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		float beatsInterval = ResourcesMaster.instance.beatsInterval;
		beatCountdown = Countdown.New(beatsInterval, SpawnGrape);

		startingFootPosition = footTransform.position;
		startingFootRotation = footTransform.eulerAngles;
	}

	private void Update()
	{
		if (GameMaster.isCounting)
		{
			CheckInput();
		}

		float beatsInterval = ResourcesMaster.instance.beatsInterval;
		beatCountdown.totalTime = beatsInterval;
		UpdateGrapesPosition();
	}

	private void UpdateGrapesPosition()
	{
		List<Grape> flaggedGrapes = new List<Grape>();
		for (int i = 0; i < fallingGrapes.Count; i++)
		{
			float grapesSpeed = ResourcesMaster.instance.grapesSpeed;
			fallingGrapes[i].transform.position += (target.position - spawnPoint.position).normalized * grapesSpeed * Time.deltaTime;
			if (grapesKiller.bounds.Contains(fallingGrapes[i].transform.position))
			{
				flaggedGrapes.Add(fallingGrapes[i]);
			}
		}

		for (int i = 0; i < flaggedGrapes.Count; i++)
		{
			fallingGrapes.Remove(flaggedGrapes[i]);
			Destroy(flaggedGrapes[i].gameObject);
			grapeColorsPool.Add(flaggedGrapes[i].colorSpectrumValue);
		}
	}

	private void SpawnGrape()
	{
		if (grapeColorsPool.Count == 0)
			return;

		float grapeColor = grapeColorsPool.GetRandom();
		Grape newGrape = Instantiate<Grape>(grapePrefab, spawnPoint.position, Quaternion.identity);
		newGrape.transform.SetParent(spawnPoint, true);
		newGrape.SetColor(grapeColor);

		fallingGrapes.Add(newGrape);
		grapeColorsPool.Remove(grapeColor);

		float interval = ResourcesMaster.instance.beatsInterval * ResourcesMaster.instance.grapeStepMultiplier;
		Countdown.New(interval, SpawnGrape);
	}

	public void AddCollectedGrape(Grape grape)
	{
		if (!gameObject.activeSelf)
		{
			GameMaster.FirstGrapeCollected();
			gameObject.SetActive(true);
		}
		
		grapeColorsPool.Add(grape.colorSpectrumValue);
	}

	private void StepOnGrape(Grape steppedGrape)
	{
		GameMaster.instance.collectActivity.RemoveGrapeByColor(steppedGrape.colorSpectrumValue);
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
			Grape closestGrape = GetClosestGrape();
			if (closestGrape)
			{
				float hitRadius = ResourcesMaster.instance.hitRadius;
				if ((closestGrape.transform.position - target.position).sqrMagnitude < hitRadius * hitRadius)
				{
					audioSource.PlayOneShot(grapeSquashing);
					fallingGrapes.Remove(closestGrape);
					Destroy(closestGrape.gameObject);
					float resourcePerTap = ResourcesMaster.instance.resourcesPerTap;
					ResourcesMaster.AddResource(generatedResourceName, resourcePerTap);

					steppedGrapesColors.Add(closestGrape.colorSpectrumValue);
					if (steppedGrapesColors.Count > 10)
						steppedGrapesColors.RemoveAt(0);
				}
				else
				{
					feedbackPanel.SetAlpha(0f);
					feedbackPanel.DOFade(0.5f, 0.5f).From();
					//float resourcePerTap = ResourcesMaster.instance.danceMatProperties.resourcesPerTap;
					//float usedResources = ResourcesMaster.GetResourceData(generatedResourceName).requiredToGeneratedRatio * resourcePerTap;
					//ResourcesMaster.RemoveRequiredResource(generatedResourceName, usedResources);
				}				

				fallingGrapes.Remove(closestGrape);
				Destroy(closestGrape.gameObject);
				GameMaster.instance.collectActivity.RemoveGrapeByColor(closestGrape.colorSpectrumValue);
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

	private Grape GetClosestGrape()
	{
		if (fallingGrapes.Count == 0)
			return null;

		Grape closestGrape = fallingGrapes[0];
		for (int i = 0; i < fallingGrapes.Count; i++)
		{
			if ((fallingGrapes[i].transform.position - target.position).sqrMagnitude < (closestGrape.transform.position - target.position).sqrMagnitude)
			{
				closestGrape = fallingGrapes[i];
			}
		}

		return closestGrape;
	}

	public float GetAverageColor()
	{
		float averageColor = 0f;
		for (int i = 0; i < steppedGrapesColors.Count; i++)
		{
			averageColor += steppedGrapesColors[i]/ (float) steppedGrapesColors.Count;
		}

		return averageColor;
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
			Gizmos.DrawWireSphere(target.position, ResourcesMaster.instance.hitRadius);
		}
	}
}