using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleLiquidResourcePool : ResourcePool
{
	[Header("Resource config:")]
	public Transform purpleLiquid;
	public float resourcePerHeight = 10f;

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

	// Updates purple liquid scale to be proportional to resourceAmount
	private void UpdatePurpleLiquidScale()
	{
		Vector3 localScale = purpleLiquid.localScale;
		localScale.y = (float) resourceAmount / (float) resourcePerHeight;
		purpleLiquid.localScale = localScale;
	}
}
