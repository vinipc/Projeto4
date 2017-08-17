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

	public float displayedAmount = 0;
	public string displayedResource;

	private List<GameObject> grapes = new List<GameObject>();

	private void Update()
	{
		float currentAmount = ResourcesMaster.GetResourceAmount(displayedResource);
		if (currentAmount != displayedAmount)
		{
			displayedAmount = currentAmount;
			UpdateGrapesQuantity();
//
//			if (currentAmount > displayedAmount)
//				AddResource((int) (currentAmount - displayedAmount));
//			else
//				RemoveResource((int) (displayedAmount - currentAmount));

			displayedAmount = currentAmount;
		}
	}

	private void UpdateGrapesQuantity()
	{
		int expectedQuantity = Mathf.FloorToInt(displayedAmount / ResourcesMaster.instance.resourcePerGrape);

		if (grapes.Count > expectedQuantity)
		{
			while (grapes.Count > expectedQuantity)
			{
				Destroy(grapes[0]);
				grapes.RemoveAt(0);
			}
		}
		else
		{
			while (grapes.Count < expectedQuantity)
			{
				Vector3 position = spawnPoint.position;
				position.x += Random.Range(-spawnRadius, spawnRadius);
				position.y += Random.Range(-0.5f, 0.5f);
				grapes.Add(Instantiate<GameObject>(grapePrefab, position, grapePrefab.transform.rotation, grapesParent));
			}
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
