using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleLiquidResourcePool : ResourcePool
{
	public Transform purpleLiquid;
	public float resourcePerHeight = 10f;

	public override void AddResource(float amount)
	{
		base.AddResource(amount);
		UpdatePurpleLiquidScale();
	}

	public override void RemoveResource(float amountDelta)
	{
		base.RemoveResource(amountDelta);
		UpdatePurpleLiquidScale();
	}

	private void UpdatePurpleLiquidScale()
	{
		Vector3 localScale = purpleLiquid.localScale;
		localScale.y = amount / resourcePerHeight;
		purpleLiquid.localScale = localScale;
	}
}
