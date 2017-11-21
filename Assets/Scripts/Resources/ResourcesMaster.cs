using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourcesState { TooLittleGrapes, JustRight, TooManyGrapes }

public class ResourcesMaster : Singleton<ResourcesMaster>
{
	public List<ResourceData> resources;

	public ResourcesState currentState;
	public Vector2 statesThresholds;
	public float stateCheckInterval = 4f;

	[Header("Grape collection")]
	public Vector2 bunchLifetimeRange;
	public Vector2 bunchSpawnIntervalRange;
	public float defaultGrapesSpawnInterval = 0.2f;
	public float grapeSpawnMultiplier = 1f;
	public float bunchSpawnMultiplier = 1f;

	[Header("Grape stepping")]
	public float resourcesPerTap;
	public float hitRadius;
	public float beatsInterval;
	public float grapesSpeed;
	public float grapeStepMultiplier = 1f;

	[Header("Wine qualities")]
	public List<float> bottlesColors = new List<float>();
	public float averageColor;
	public float deviation;

	[Header("Resource generation variables")]
	public float resourcePerMicThreshold;

	[Header("Resource display variables")]
	public Gradient grapeColorGradient;
	public int grapesPerBunch;
	public float resourcePerJuiceHeight;
	public float resourcePerBottle;

	private static Dictionary<string, float> resourcePools;

	[TextArea(2, 10)]
	public string debug;

	private void Awake()
	{
		resourcePools = new Dictionary<string, float>();
		for (int i = 0; i < resources.Count; i++)
		{
			resourcePools.Add(resources[i].uniqueName, 0f);
		}

		StartCoroutine(UpdateState());
	}

	public static ResourceData GetResourceData(string name)
	{
		return instance.resources.Find(res => res.uniqueName == name);
	}

	public static float GetResourceAmount(string name)
	{
		return resourcePools[name];
	}

	public static void AddResource(string name, float amount)
	{
		ResourceData resource = instance.resources.Find(res => res.uniqueName == name);
		float requiredAmount = resource.requiredToGeneratedRatio * amount;

		if (string.IsNullOrEmpty(resource.requiredResourceName) || requiredAmount <= resourcePools[resource.requiredResourceName])
		{
			if (!string.IsNullOrEmpty(resource.requiredResourceName))
				RemoveResource(resource.requiredResourceName, requiredAmount);
			
			resourcePools[resource.uniqueName] += amount;
			instance.UpdateDebug();
		}
	}

	/// <summary>
	/// Removes the required resource of the named resource.
	/// </summary>
	public static void RemoveRequiredResource(string name, float amount)
	{
		ResourceData resource = instance.resources.Find(res => res.uniqueName == name);
		RemoveResource(resource.requiredResourceName, amount);
	}

	public static void RemoveResource(string name, float amount)
	{
		ResourceData resource = instance.resources.Find(res => res.uniqueName == name);
		resourcePools[resource.uniqueName] = Mathf.Max(0f, resourcePools[resource.uniqueName] - amount);
		instance.UpdateDebug();
	}

	public static void AddBottle(float color)
	{
		instance.averageColor = 0f;
		instance.deviation = 0f;

		instance.bottlesColors.Add(color);

		for (int i = 0; i < instance.bottlesColors.Count; i++)
		{
			instance.averageColor += instance.bottlesColors[i] / (float) instance.bottlesColors.Count;
		}

		float accumulatedDeviation = 0f;
		for (int i = 0; i < instance.bottlesColors.Count; i++)
		{
			accumulatedDeviation += Mathf.Pow(instance.bottlesColors[i] - instance.averageColor, 2f);
		}
		accumulatedDeviation /= (float) instance.bottlesColors.Count;
		instance.deviation = Mathf.Sqrt(accumulatedDeviation);
	}

	private IEnumerator UpdateState()
	{
		if (GameMaster.instance.collectActivity.collectedGrapes.Count <= statesThresholds.x && (int) GameMaster.currentProgressionState >= (int) ActivitiesProgression.Stepping)
		{
			SetTooLittleGrapesState();
		}
		else if (GameMaster.instance.collectActivity.collectedGrapes.Count <= statesThresholds.y)
		{
			SetJustRightState();
		}
		else if ((int) GameMaster.currentProgressionState >= (int) ActivitiesProgression.Stepping)
		{
			SetTooManyGrapesState();
		}

		yield return new WaitForSeconds(stateCheckInterval);
		StartCoroutine(UpdateState());
	}

	private void SetTooLittleGrapesState()
	{
		currentState = ResourcesState.TooLittleGrapes;

		grapeSpawnMultiplier = 0.5f;
		bunchSpawnMultiplier = 0.5f;

		// Set default value for step multiplier
		grapeStepMultiplier = 1f;
	}

	private void SetJustRightState()
	{
		currentState = ResourcesState.JustRight;

		grapeSpawnMultiplier = 1f;
		bunchSpawnMultiplier = 1f;

		// Set default value for step multiplier
		grapeStepMultiplier = 1f;
	}

	private void SetTooManyGrapesState()
	{
		currentState = ResourcesState.TooManyGrapes;

		// Set default value for spawns multipliers
		grapeSpawnMultiplier = 1f;
		bunchSpawnMultiplier = 1f;

		grapeStepMultiplier = 0.5f;
	}

	private void UpdateDebug()
	{
		debug = string.Empty;
		foreach(KeyValuePair<string, float> entry in resourcePools)
		{
			debug = string.Concat(debug, entry.Key, ": ", entry.Value.ToString(), "\n");
		}
	}
}

[System.Serializable]
public class ResourceData
{
	public string uniqueName;
	public string requiredResourceName;
	[Tooltip("required / generated ratio")]
	public float requiredToGeneratedRatio;
}