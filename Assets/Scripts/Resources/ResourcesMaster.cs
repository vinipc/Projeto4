using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesMaster : Singleton<ResourcesMaster>
{
	public List<Resource> resources;

	private static Dictionary<string, float> resourcePools;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public static void AddResource(string name, float amount)
	{
		Resource resource = instance.resources.Find(res => res.uniqueName == name);
		float requiredAmount = resource.requiredToGeneratedRatio * amount;

		if (resource == null)
		{
			Debug.Log(string.Concat("Added resource doesn't exist: ", name));
			return;
		}

		if (string.IsNullOrEmpty(resource.requiredResource) || requiredAmount <= resourcePools[resource.requiredResource])
			resourcePools[resource.requiredResource] -= requiredAmount;

		resourcePools[resource.uniqueName] += amount;
	}
}

[System.Serializable]
public class Resource
{
	public string uniqueName;
	public string requiredResource;
	[Tooltip("required / generated ratio")]
	public float requiredToGeneratedRatio;
}