using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapesResourcePool : ResourcePool
{
	public GameObject grapePrefab;
	public Transform spawnPoint;
	public Transform grapesParent;
	public float spawnRadius = 1f;

	private List<GameObject> grapes = new List<GameObject>();

	public override void AddResource(float amountDelta)
	{
		base.AddResource(amountDelta);
		int iAmount = (int) amountDelta;

		for (int i = 0; i < iAmount; i++)
		{
			Vector3 position = spawnPoint.position + Vector3.left * Random.Range(-spawnRadius, spawnRadius);
			GameObject newGrape = Instantiate<GameObject>(grapePrefab, position, Quaternion.identity, grapesParent);
			grapes.Add(newGrape);
		}
	}

	public override void RemoveResource(float amountDelta)
	{
		int iAmount = (int) amountDelta;
		iAmount = Mathf.Max(iAmount, (int) amountDelta);

		for (int i = 0; i < iAmount; i++)
		{
			Destroy(grapes[0]);
			grapes.RemoveAt(0);
		}

		base.RemoveResource(amountDelta);
	}
}
