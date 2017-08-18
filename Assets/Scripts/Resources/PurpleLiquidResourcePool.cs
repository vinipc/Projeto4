using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleLiquidResourcePool : MonoBehaviour
{
	[Header("Resource config:")]
	public Transform purpleLiquid;
	public float resourcePerHeight = 10f;

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
		Vector3 localScale = purpleLiquid.localScale;
		localScale.y = (float) lastDisplayedAmount / (float) resourcePerHeight;
		purpleLiquid.localScale = localScale;
	}
}
