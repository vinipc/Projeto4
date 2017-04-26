using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleLiquidResourcePool : ResourcePool
{
	public Transform purpleLiquid;
	public float resourcePerHeight = 10;

	public override void AddResource(int amount)
	{
		base.AddResource(amount);
		UpdatePurpleLiquidScale();
	}

	public override void RemoveResource(int amount)
	{
		base.RemoveResource(amount);
		UpdatePurpleLiquidScale();
	}

	private void UpdatePurpleLiquidScale()
	{
		Vector3 localScale = purpleLiquid.localScale;
		localScale.y = (float) resourceAmount / (float) resourcePerHeight;
		purpleLiquid.localScale = localScale;
	}
}
