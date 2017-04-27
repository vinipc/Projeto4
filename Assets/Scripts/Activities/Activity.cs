using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class that interfaces with ResourcePool to get inputs, consume and generate resources
public class Activity : MonoBehaviour
{
	public ResourcePool requiredResource;
	public ResourcePool generatedResource;

	[Tooltip("required / generated ratio")]
	public float requirementToGenerationRatio = 1f;

	public virtual void GenerateResource(int generatedAmount)
	{
		int requiredAmount = (int) requirementToGenerationRatio * generatedAmount;

		// If there is not a required resource OR the current amount is over the required amount,
		//	then decreases required resource and increased generated one.
		if (requiredResource == null || requiredAmount <= requiredResource.resourceAmount)
		{
			if (requiredResource != null)
				requiredResource.RemoveResource(requiredAmount);

			generatedResource.AddResource(generatedAmount);
		}
	}
}