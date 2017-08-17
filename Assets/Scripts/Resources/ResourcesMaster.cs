using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesMaster : Singleton<ResourcesMaster>
{
	public List<Resource> resources;

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
	}

	public static void AddResource(string name, float amount)
	{
		Debug.Log("Adding resource: " + name);
		Resource resource = instance.resources.Find(res => res.uniqueName == name);
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
		Resource resource = instance.resources.Find(res => res.uniqueName == name);
		RemoveResource(resource.requiredResourceName, amount);
	}

	public static void RemoveResource(string name, float amount)
	{
		Resource resource = instance.resources.Find(res => res.uniqueName == name);
		resourcePools[resource.uniqueName] = Mathf.Max(0f, resourcePools[resource.uniqueName] - amount);
		instance.UpdateDebug();
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
public class Resource
{
	public string uniqueName;
	public string requiredResourceName;
	[Tooltip("required / generated ratio")]
	public float requiredToGeneratedRatio;
}