using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapesResourcePool : MonoBehaviour
{
	[Header("Resource config:")]
	public GameObject grapePrefab;
	public Transform spawnPoint;
	public Transform grapesParent;
	public float spawnRadius = 1f;

	public float lastDisplayedAmount = 0;
	public string displayedResource;

	private List<GameObject> grapes = new List<GameObject>();

	private void Update()
	{
		float currentAmount = ResourcesMaster.GetResourceAmount(displayedResource);
		if (currentAmount != lastDisplayedAmount)
		{
			if (currentAmount > lastDisplayedAmount)
				AddResource((int) (currentAmount - lastDisplayedAmount));
			else
				RemoveResource((int) (lastDisplayedAmount - currentAmount));

			lastDisplayedAmount = currentAmount;
		}
	}

	public void AddResource(int amount)
	{
		// Creates grape objects and adds them to grapes list
		for (int i = 0; i < amount; i++)
		{
			Vector3 position = spawnPoint.position;
			position.x += Random.Range(-spawnRadius, spawnRadius);
			position.y += Random.Range(-0.5f, 0.5f);

			GameObject newGrape = Instantiate<GameObject>(grapePrefab, position, Quaternion.identity, grapesParent);
			grapes.Add(newGrape);
		}
	}

	public void RemoveResource(int amount)
	{
		// Destroys consumed grapes
		if (grapes.Count > amount)
		{
			for (int i = 0; i < amount; i++)
			{
				Destroy(grapes[0]);
				grapes.RemoveAt(0);
			}			
		}
	}
}
