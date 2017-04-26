using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlesResourcePool : ResourcePool
{
	public GameObject bottlePrefab;
	public Transform bottlesParent;
	public float resourcesPerBottle = 10f;

	private List<GameObject> bottles = new List<GameObject>();

	public override void AddResource(float amountDelta)
	{
		base.AddResource(amountDelta);

		if ((amount - resourcesPerBottle * bottles.Count) > resourcesPerBottle)
		{
			bottles.Add(Instantiate<GameObject>(bottlePrefab, bottlesParent));
		}
	}
}
