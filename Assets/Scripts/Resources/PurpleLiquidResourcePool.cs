using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleLiquidResourcePool : MonoBehaviour
{
	[Header("Resource config:")]
	public Transform purpleLiquid;

	public string displayedResource;
	private float lastDisplayedAmount;

	private void Update()
	{
		float currentAmount = ResourcesMaster.GetResourceAmount(displayedResource);
		if (currentAmount != lastDisplayedAmount)
		{
			lastDisplayedAmount = currentAmount;
			UpdatePurpleLiquidScale();
		}
	}

	// Updates purple liquid scale to be proportional to resourceAmount
	private void UpdatePurpleLiquidScale()
	{
		float resourcePerHeight = ResourcesMaster.instance.resourcePerJuiceHeight;
		Vector3 localScale = purpleLiquid.localScale;
		localScale.y = (float) lastDisplayedAmount / (float) resourcePerHeight;
		purpleLiquid.localScale = localScale;
	}
}
