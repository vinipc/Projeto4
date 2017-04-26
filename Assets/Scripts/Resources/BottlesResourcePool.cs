using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlesResourcePool : ResourcePool
{
	public GameObject bottlePrefab;
	public Transform bottlesParent;
	public int resourcesPerBottle = 10;

	private List<GameObject> bottles = new List<GameObject>();

	public override void AddResource(int amount)
	{
		base.AddResource(amount);

		if ((resourceAmount - resourcesPerBottle * bottles.Count) > resourcesPerBottle)
		{
			bottles.Add(Instantiate<GameObject>(bottlePrefab, bottlesParent));
		}
	}
}
