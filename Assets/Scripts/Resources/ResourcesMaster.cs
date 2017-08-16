using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ResourceConfig
{
	public string uniqueName;
	public string requiredResource;
	[Tooltip("required / generated ratio")]
	public float requiredToGeneratedRatio;
}

public class ResourcesMaster : Singleton<ResourcesMaster>
{
	public List<ResourceConfig> resources;

	private static Dictionary<string, float> resourcePools;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public static void AddResource(string name, float amount)
	{
		ResourceConfig? resource = instance.resources.Find(res => res.uniqueName == name);
		float requiredAmount = ((ResourceConfig) resource).requiredToGeneratedRatio * amount;

		if (resource == null)
		{
			Debug.Log(string.Concat("Added resource doesn't exist: ", name));
			return;
		}

		if (string.IsNullOrEmpty(((ResourceConfig) resource).requiredResource) || requiredAmount <= resourcePools[((ResourceConfig) resource).requiredResource])
			resourcePools[((ResourceConfig) resource).requiredResource] -= requiredAmount;

		resourcePools[((ResourceConfig) resource).uniqueName] += amount;
	}
}