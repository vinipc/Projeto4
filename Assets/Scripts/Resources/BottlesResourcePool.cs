using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlesResourcePool : ResourcePool
{
	[Header("Resource config:")]
	public GameObject bottlePrefab;
	public Transform bottlesParent;
	public int resourcesPerBottle = 10;

	private List<GameObject> bottles = new List<GameObject>();

	public override void AddResource(int amount)
	{
		base.AddResource(amount);
		CreateNewBottles();
	}

	// Creates new bottles if enough resource was generated
	private void CreateNewBottles()
	{
		if ((resourceAmount - resourcesPerBottle * bottles.Count) > resourcesPerBottle)
		{
			bottles.Add(Instantiate<GameObject>(bottlePrefab, bottlesParent));
		}		
	}
}
