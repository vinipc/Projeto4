using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapesResourcePool : ResourcePool
{
	[Header("Resource config:")]
	public GameObject grapePrefab;
	public Transform spawnPoint;
	public Transform grapesParent;
	public float spawnRadius = 1f;

	private List<GameObject> grapes = new List<GameObject>();

	public override void AddResource(int amount)
	{
		base.AddResource(amount);

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

	public override void RemoveResource(int amount)
	{
		base.RemoveResource(amount);

		// Destroys consumed grapes
		for (int i = 0; i < amount; i++)
		{
			Destroy(grapes[0]);
			grapes.RemoveAt(0);
		}
	}
}
